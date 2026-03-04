using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media.Control;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace OBS_StreamMusicViewer
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private string _outputFilePath;
        private GlobalSystemMediaTransportControlsSessionManager _sessionManager;

        // System tray icon
        private NotifyIcon _notifyIcon;
        private bool _isClosing = false; // true only when "Quitter" is clicked

        public MainWindow()
        {
            InitializeComponent();
            _outputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "current_song.json");

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            InitializeTrayIcon();
            InitializeMediaManager();
        }

        // ─── System Tray ────────────────────────────────────────────────────────

        private void InitializeTrayIcon()
        {
            _notifyIcon = new NotifyIcon();

            // Charger l'icône depuis le fichier .ico embarqué
            string icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OSMV_logo.ico");
            if (File.Exists(icoPath))
                _notifyIcon.Icon = new Icon(icoPath);
            else
                _notifyIcon.Icon = SystemIcons.Application;

            _notifyIcon.Text = "OBS Stream Music Viewer";
            _notifyIcon.Visible = true;

            // Double-clic → restaurer la fenêtre
            _notifyIcon.DoubleClick += (s, e) => RestoreWindow();

            // Menu contextuel
            var contextMenu = new ContextMenuStrip();

            var menuShow = new ToolStripMenuItem("Afficher");
            menuShow.Font = new Font(menuShow.Font, System.Drawing.FontStyle.Bold);
            menuShow.Click += (s, e) => RestoreWindow();

            var menuQuit = new ToolStripMenuItem("Quitter");
            menuQuit.Click += (s, e) =>
            {
                _isClosing = true;
                System.Windows.Application.Current.Shutdown();
            };

            contextMenu.Items.Add(menuShow);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(menuQuit);

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void RestoreWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            Activate();
        }

        // Quand la fenêtre est minimisée → la cacher dans le tray
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                ShowInTaskbar = false;
                _notifyIcon.ShowBalloonTip(
                    2000,
                    "OBS Stream Music Viewer",
                    "L'application tourne en arrière-plan.",
                    ToolTipIcon.Info);
            }
        }

        // Interception de la fermeture : minimiser au lieu de quitter (sauf si "Quitter" cliqué)
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isClosing)
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
                return;
            }

            // Fermeture réelle
            _timer?.Stop();
            _notifyIcon?.Dispose();
            try { WriteJsonDump(null, null, null, "closed", null); } catch { }
        }

        // ─── Media Manager ───────────────────────────────────────────────────────

        private async void InitializeMediaManager()
        {
            try
            {
                _sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                _timer.Start();
                UpdateUI("Manager initialized, awaiting track...", "---", "", null);
            }
            catch (Exception ex)
            {
                ErrorText.Text = "Failed to access Windows Media API: " + ex.Message;
            }
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_sessionManager == null) return;

                var session = _sessionManager.GetCurrentSession();

                if (session == null)
                {
                    WriteJsonDump(null, null, null, "closed", null);
                    UpdateUI("No active media", "---", "closed", null);
                    return;
                }

                var mediaProps = await session.TryGetMediaPropertiesAsync();
                if (mediaProps == null)
                {
                    WriteJsonDump(null, null, null, "closed", null);
                    UpdateUI("No media properties", "---", "closed", null);
                    return;
                }

                string title = mediaProps.Title ?? "Unknown Title";
                string artist = mediaProps.Artist ?? "Unknown Artist";
                string album = mediaProps.AlbumTitle ?? "";

                var playbackInfo = session.GetPlaybackInfo();
                string status = "unknown";
                if (playbackInfo != null)
                {
                    switch (playbackInfo.PlaybackStatus)
                    {
                        case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Closed:   status = "closed";   break;
                        case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Opened:   status = "opened";   break;
                        case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Changing: status = "changing"; break;
                        case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped:  status = "stopped";  break;
                        case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing:  status = "playing";  break;
                        case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused:   status = "paused";   break;
                    }
                }

                string base64Image = null;
                BitmapImage bitmapImage = null;

                if (mediaProps.Thumbnail != null)
                {
                    try
                    {
                        using (var stream = await mediaProps.Thumbnail.OpenReadAsync())
                        {
                            byte[] buffer = new byte[stream.Size];
                            await stream.ReadAsync(buffer.AsBuffer(), (uint)stream.Size, InputStreamOptions.None);
                            base64Image = Convert.ToBase64String(buffer);

                            bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = new MemoryStream(buffer);
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();
                        }
                    }
                    catch (Exception) { /* Handle thumbnail error gracefully */ }
                }

                UpdateUI(title, artist, status, bitmapImage);
                WriteJsonDump(title, artist, album, status, base64Image);
            }
            catch (Exception ex)
            {
                ErrorText.Text = "Error during tick: " + ex.Message;
            }
        }

        private void UpdateUI(string title, string artist, string status, BitmapImage image)
        {
            TitleText.Text = title;
            ArtistText.Text = artist;
            StatusText.Text = "Status: " + status;

            if (image != null)
                AlbumArtBrush.ImageSource = image;
            else
                AlbumArtBrush.ImageSource = null;
        }

        private void WriteJsonDump(string title, string artist, string album, string status, string thumbnailB64)
        {
            try
            {
                object data;

                if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(artist))
                {
                    data = null; // No song playing
                }
                else
                {
                    data = new
                    {
                        title = title,
                        artist = artist,
                        album = album,
                        thumbnail = thumbnailB64,
                        status = status,
                        timestamp = DateTime.Now.ToString("o")
                    };
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(_outputFilePath, jsonString);
            }
            catch (Exception ex)
            {
                ErrorText.Text = "JSON save error: " + ex.Message;
            }
        }
    }
}

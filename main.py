"""
Now Playing Widget - Backend Script (C# Helper Version)
Récupère les informations de lecture depuis n'importe quelle application média Windows
et les exporte dans un fichier JSON pour l'affichage OBS.

Cette version utilise un helper C# (MediaInfoGetter.exe) qui accède directement aux Windows Runtime APIs,
compatible avec Python 3.14 et l'application Apple Music UWP moderne.
"""

import json
import subprocess
import base64
import os
import sys
import time
from pathlib import Path
from typing import Optional, Dict
from datetime import datetime


class NowPlayingTracker:
    """Tracker pour les informations de lecture via PowerShell WinRT."""
    
    def __init__(self, output_file: str = "current_song.json"):
        self.output_file = Path(output_file)
        self.current_song: Optional[Dict] = None
        self.last_update = None
        self.temp_artwork_path = Path(output_file).parent / "temp_artwork.jpg"
        self.exe_path = Path(__file__).parent / "MediaInfoGetter.exe"
        
        # Vérifier que l'exécutable C# existe
        if not self.exe_path.exists():
            print(f"❌ Exécutable MediaInfoGetter.exe introuvable: {self.exe_path}")
            print(f"   Compilez-le avec: dotnet publish MediaInfoGetter.csproj -c Release -o .")
            sys.exit(1)
        
    def get_current_track_info(self) -> Optional[Dict]:
        """Récupère les informations via l'exécutable C# MediaInfoGetter."""
        try:
            result = subprocess.run(
                [
                    str(self.exe_path.absolute()),
                    str(self.temp_artwork_path.absolute())
                ],
                capture_output=True,
                text=True,
                encoding='utf-8',
                errors='replace',
                timeout=5
            )
            output = result.stdout.strip() if result.stdout else ""
            
            if not output or output == "NO_TRACK":
                return None
                
            if output.startswith("ERROR:"):
                error_msg = output[6:]
                if "Cannot find type" in error_msg or "NotFound" in error_msg:
                    # Erreur silencieuse pour les problèmes de type WinRT
                    return None
                print(f"⚠️  {error_msg}")
                return None
            
            # Parser la sortie (format: title|artist|album|status|hasArtwork)
            parts = output.split('|')
            if len(parts) < 5:
                return None
                
            title, artist, album, status_code, has_artwork = parts
            
            # Mapper le statut
            status_map = {
                "0": "closed",
                "1": "opened",
                "2": "changing",
                "3": "stopped",
                "4": "playing",
                "5": "paused"
            }
            status = status_map.get(status_code.strip(), "unknown")
            
            # Lire l'artwork si disponible
            artwork_b64 = None
            if has_artwork.strip() == "True" and self.temp_artwork_path.exists():
                try:
                    with open(self.temp_artwork_path, 'rb') as f:
                        artwork_b64 = base64.b64encode(f.read()).decode('utf-8')
                    self.temp_artwork_path.unlink()
                except Exception as e:
                    print(f"⚠️  Erreur lecture artwork: {e}")
            
            return {
                "title": title.strip() or "Unknown Title",
                "artist": artist.strip() or "Unknown Artist",
                "album": album.strip() or "",
                "thumbnail": artwork_b64,
                "status": status,
                "timestamp": datetime.now().isoformat()
            }
            
        except subprocess.TimeoutExpired:
            return None
        except FileNotFoundError:
            print("❌ PowerShell introuvable. Vérifiez votre installation Windows.")
            return None
        except Exception as e:
            print(f"❌ Erreur: {e}")
            return None
    
    def update_loop(self, interval: float = 1.0):
        """Boucle principale de mise à jour."""
        print(f"🎵 Now Playing Tracker démarré (WinRT via PowerShell)")
        print(f"📁 Fichier de sortie: {self.output_file.absolute()}")
        print(f"⏱️  Intervalle de mise à jour: {interval}s")
        print(f"🎧 Compatible avec: Apple Music, Spotify, Chrome, Edge, etc.")
        print("-" * 60)
        
        first_run = True
        
        while True:
            try:
                track_info = self.get_current_track_info()
                
                if track_info:
                    # Vérifier si la chanson a changé
                    song_changed = (
                        self.current_song is None or
                        self.current_song.get("title") != track_info.get("title") or
                        self.current_song.get("artist") != track_info.get("artist")
                    )
                    
                    if song_changed:
                        if first_run:
                            first_run = False
                        print(f"\n🎵 Nouvelle piste détectée!")
                        print(f"   Titre: {track_info['title']}")
                        print(f"   Artiste: {track_info['artist']}")
                        if track_info['album']:
                            print(f"   Album: {track_info['album']}")
                        print(f"   Statut: {track_info['status']}")
                        print(f"   Pochette: {'✓' if track_info['thumbnail'] else '✗'}")
                    
                    self.current_song = track_info
                    self.save_current_song()
                    
                else:
                    # Aucune lecture en cours
                    if self.current_song is not None:
                        print("\n⏸️  Aucune lecture en cours")
                        self.current_song = None
                        self.save_current_song()
                
                time.sleep(interval)
                
            except KeyboardInterrupt:
                print("\n\n👋 Arrêt du tracker...")
                break
            except Exception as e:
                print(f"❌ Erreur dans la boucle principale: {e}")
                time.sleep(interval)
        
        # Nettoyage
        if self.temp_artwork_path.exists():
            try:
                self.temp_artwork_path.unlink()
            except:
                pass
    
    def save_current_song(self):
        """Sauvegarde les informations de la chanson actuelle dans un fichier JSON."""
        try:
            with open(self.output_file, 'w', encoding='utf-8') as f:
                json.dump(self.current_song, f, ensure_ascii=False, indent=2)
            self.last_update = datetime.now()
        except Exception as e:
            print(f"❌ Erreur lors de la sauvegarde: {e}")


def main():
    """Point d'entrée principal."""
    if sys.platform != "win32":
        print("⚠️  Ce script est conçu pour Windows uniquement.")
        return
    
    print("=" * 60)
    print("  Now Playing Widget - Apple Music & Plus")
    print("=" * 60)
    print()
    
    tracker = NowPlayingTracker("current_song.json")
    tracker.update_loop(interval=1.0)


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("\n👋 Programme terminé")

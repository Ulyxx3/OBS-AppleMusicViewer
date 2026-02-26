# 🔍 Guide de Dépannage (Troubleshooting)

## ✅ Vérification des Fichiers Source

Si vous avez cloné ou téléchargé le code source pour le compiler vous-même, vérifiez que vous avez ces fichiers principaux :

```
OBS-StreamMusicViewer/
├── .gitignore
├── App.xaml                  ← Interface base
├── App.xaml.cs
├── MainWindow.xaml           ← Design de la fenêtre
├── MainWindow.xaml.cs        ← Logique de récupération de musique
├── OBS-StreamMusicViewer.csproj ← CRITIQUE - fichier projet
├── README.md
├── compile.bat               ← Script de compilation
├── index.html                ← Affichage pour OBS
└── style.css                 ← Styles pour OBS
```

## 🐛 Problèmes Fréquents & Solutions

### 1. La façon la plus simple (Pas besoin de compiler !)
Si vous rencontrez des erreurs de compilation, abandonnez la ligne de commande et téléchargez simplement la **Release**.
1. Allez dans l'onglet **Releases** de GitHub.
2. Téléchargez l'exécutable `OBS-StreamMusicViewer.exe` ou le fichier ZIP contenant la release.
3. Lancez le fichier `.exe` généré. Aucun outil de développement ou ligne de commande n'est requis !

### 2. Le script "compile.bat" affiche une erreur de namespace / projet manquant
**Cause** : Le fichier `.csproj` n'est pas trouvé par la commande `dotnet` ou le clone a mal fonctionné.
**Solution** : Assurez-vous d'être bien dans le bon dossier. Vous pouvez aussi télécharger le ZIP du code source (`Code → Download ZIP`) depuis Github pour être sûr d'avoir tous les fichiers intacts.

### 3. "dotnet n'est pas reconnu en tant que commande"
**Cause** : Le .NET SDK n'est pas installé sur votre ordinateur.
**Solution** : 
1. Installez-le depuis https://dotnet.microsoft.com/download/dotnet
2. **Redémarrez** absolument votre terminal ou votre PC pour que la variable d'environnement soit prise en compte, puis relancez `compile.bat`.

### 4. Le widget OBS affiche "Waiting for music..." mais une musique joue
**Cause** : Le navigateur web de base (OBS) ou le programme (`OBS-StreamMusicViewer.exe`) a un problème de permissions, ou l'application musicale ne diffuse pas l'info à Windows.
**Solution** :
- Vérifiez que la fenêtre `OBS-StreamMusicViewer.exe` détecte bien la musique. Si oui, le problème vient d'OBS.
- Assurez-vous que le fichier `index.html` ouvert dans OBS est bien situé **dans le même dossier** que `current_song.json`.
- Si c'est un navigateur source (ex: Chrome/Youtube) qui joue la musique, vérifiez que les "Contrôles multimédias globaux" ne sont pas désactivés dans les paramètres du navigateur.

## 💡 Support

Si votre problème persiste malgré la version pré-compilée, ouvrez une **Issue** sur GitHub en précisant :
- Le comportement observé et l'application musicale utilisée (Spotify, Apple Music, Navigateur...)
- Votre version de Windows (10 ou 11)

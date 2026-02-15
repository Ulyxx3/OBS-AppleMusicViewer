# 🔍 Guide de Vérification après Téléchargement

Si vous rencontrez l'erreur `Le fichier projet n'existe pas`, suivez ces étapes :

## ✅ Vérification des Fichiers

Après avoir cloné ou téléchargé le projet, vérifiez que vous avez **tous ces fichiers** :

```
OBS-StreamMusicViewer/
├── .gitignore
├── LICENSE
├── MediaInfoGetter.cs          ← IMPORTANT
├── MediaInfoGetter.csproj      ← CRITIQUE - fichier projet C#
├── README.md
├── compile.bat
├── index.html
├── main.py
└── style.css
```

## 🐛 Solutions si MediaInfoGetter.csproj est manquant

### Option 1 : Re-cloner le Repository

```bash
# Supprimer le dossier téléchargé
# Cloner à nouveau
git clone https://github.com/Ulyxx3/OBS-StreamMusicViewer.git
cd OBS-StreamMusicViewer
```

### Option 2 : Télécharger le ZIP depuis GitHub

1. Allez sur https://github.com/Ulyxx3/OBS-StreamMusicViewer
2. Cliquez sur **Code** → **Download ZIP**
3. Extrayez **TOUT** le contenu (pas seulement certains fichiers)
4. Vérifiez que `MediaInfoGetter.csproj` est bien présent

### Option 3 : Utiliser la Release (plus simple)

Au lieu de compiler, téléchargez la **Release** qui contient l'exécutable pré-compilé :
1. Allez dans l'onglet **Releases**
2. Téléchargez `OBS-StreamMusicViewer-v1.0.0-precompiled.zip`
3. Pas besoin de compiler ! Lancez directement `python main.py`

## ⚠️ Erreurs Courantes

### Erreur : "Le fichier projet n'existe pas"
**Cause** : Le fichier `.csproj` n'a pas été téléchargé
**Solution** : Re-télécharger le projet complet (voir Option 1 ou 2)

### Erreur : "dotnet command not found"
**Cause** : .NET SDK n'est pas installé
**Solution** : Installer depuis https://dotnet.microsoft.com/download/dotnet

### Erreur : "Access denied" lors de la compilation
**Cause** : Permissions insuffisantes
**Solution** : Lancer PowerShell en administrateur

## 💡 Support

Si le problème persiste, ouvrez une **Issue** sur GitHub avec :
- Le message d'erreur complet
- La liste des fichiers présents dans votre dossier
- Votre version de Windows et .NET SDK (`dotnet --version`)

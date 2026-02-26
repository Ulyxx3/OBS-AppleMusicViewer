# 📦 Guide pour publier une nouvelle Release (Version)

Ce guide t'explique pas à pas comment publier une nouvelle version (Release) de l'application prête à l'emploi sur le GitHub de ton projet. Ainsi, les utilisateurs n'auront plus besoin d'installer le .NET SDK ou de compiler eux-mêmes l'application.

## 📁 Étape 1 : Compiler l'application
Puisque tu vas fournir un exécutable prêt à l'emploi (Self-Contained), tu dois t'assurer de compiler avec la dernière version du code.

1. Ouvre le dossier du projet : `C:\Users\Ulysse\Documents\GitHub\OBS-StreamMusicViewer`
2. Double-clique sur le script **`compile.bat`**.
3. Attends que le message de succès s'affiche.
4. Cela va générer un exécutable tout neuf appelé **`OBS-StreamMusicViewer.exe`**. Teste-le pour vérifier d'abord que l'application marche correctement sur ton ordinateur.

## 🗜️ Étape 2 : Préparer l'archive (.zip)
Il est préférable de fournir un seul dossier compressé comportant l'application métier et les fichiers web d'OBS, afin que l'utilisateur ait la structure de dossiers prête instantanément.

1. Sélectionne ensemble les fichiers suivants :
   - `OBS-StreamMusicViewer.exe` (Le programme principal)
   - `index.html` (L'interface visuelle pour OBS)
   - `style.css` (Les styles pour OBS)
   - *(Optionnel mais recommandé)* `README.md` et `TROUBLESHOOTING.md`
2. Fais un **clic droit** sur les éléments sélectionnés > **Compresser dans un fichier ZIP**.
3. Nomme le ficher ZIP de manière évidente, par exemple : `OBS-StreamMusicViewer-v1.0.0.zip`.

## 🌐 Étape 3 : Créer la Release sur GitHub
Maintenant que le fichier ZIP prêt à l'emploi est là, il faut le poster sur GitHub.

1. Rends-toi sur la page de ton répertoire GitHub : **`https://github.com/Ulyxx3/OBS-StreamMusicViewer`**
2. Côté droit, dans la section "Releases", clique sur **[Create a new release]** (ou sur *"Draft a new release"* si une existe déjà).
3. **Choose a tag** : Clique et tape un nouveau numéro de version, exemple : `v1.0.0`, puis clique sur "+ Create new tag: v1.0.0 on publish".
4. **Release title** : Donne un nom à ta mise à jour (ex: `Version 1.0.0 - Standalone C# Update !`).
5. **Description** : Décris les nouveautés, par exemple :
   ```markdown
   🌟 Première Release Publique du Widget OBS.
   - Ne nécessite plus aucune installation Python ou dotnet de la part des utilisateurs !
   - Téléchargez, extrayez, et double-cliquez sur le .exe.
   - Nouvelle interface transparente et sans bordure.
   ```
6. **Attach binaries by dropping them here** : Prends ton fichier `OBS-StreamMusicViewer-v1.0.0.zip` créé précédemment et glisse-le dans cette zone encadrée au bas de l'éditeur Github. Attends que le chargement (upload) finisse.
7. Clique enfin sur le gros bouton vert **[Publish release]** en bas de la page.

✅ **C'est fini !** Les gens peuvent désormais aller dans ton onglet "Releases" et télécharger ton widget en un clic !

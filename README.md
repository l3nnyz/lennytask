# 🚀 lennytask (ProcessKiller)

**lennytask** est un utilitaire système en ligne de commande développé en **C# (.NET 9)**. Ce projet est une réimplémentation optimisée et structurée de l'outil `taskkill`, conçue pour offrir des performances natives et une gestion avancée des processus Windows.

## 🧠 Philosophie du Projet
L'objectif de **lennytask** est de démontrer qu'il est possible d'allier la puissance du framework .NET moderne à la rigueur du développement système bas niveau. 

Ma démarche repose sur trois piliers :
1. **L'Architecture Propre** : Utilisation de patterns de conception (Command & Service) pour un code maintenable et testable.
2. **La Performance Native** : Recours à la compilation **Native AOT** pour un binaire ultra-léger et sans dépendances.
3. **L'Interopérabilité Système** : Utilisation directe de l'API Win32 pour dépasser les limitations des bibliothèques standards.

## 🛠️ Architecture Technique

### Pattern Command & Découplage
Le projet implémente un `CommandInvoker` qui gère dynamiquement l'exécution des commandes via une interface `ICommand`. 
- **ListProcessesCommand** : Gère l'affichage et le filtrage des processus actifs.
- **KillProcessCommand** : Gère la logique de terminaison selon les paramètres saisis.

### Gestion Bas Niveau (Win32 P/Invoke)
Pour la fonctionnalité de terminaison d'arbre de processus (`KillProcessTree`), j'utilise `kernel32.dll` via **P/Invoke**. 
Le code fait appel à `CreateToolhelp32Snapshot` et `PROCESSENTRY32` pour naviguer dans la hiérarchie parent/enfant du système, une opération que la classe `System.Diagnostics.Process` ne permet pas de réaliser nativement de manière récursive.

### Optimisation Native AOT
Le projet est configuré pour la compilation **Ahead-Of-Time** (`PublishAot: true`). Cela transforme le code C# en code machine pur, garantissant :
- Un démarrage instantané.
- Une consommation RAM minimale.
- Un exécutable unique sans besoin d'installer le runtime .NET sur le poste client.

## ✨ Fonctionnalités
- 📊 **Listing Exhaustif** : Visualisation du PID, nom, utilisation mémoire (Working Set), chemin d'exécution et heure de lancement.
- 🎯 **Filtrage Intelligent** : Recherche par nom de processus ou par identifiant unique (PID).
- ⚔️ **Terminaison Avancée** :
    - **Mode Doux** : Demande de fermeture propre à l'application.
    - **Mode Forcé (`/f`)** : Terminaison immédiate du processus.
    - **Mode Récursif (`/t`)** : Nettoyage complet de l'arbre de processus (processus parents et enfants).

## 🚀 Installation et Utilisation

### Compilation
```bash
# Publication en mode Natif AOT
dotnet publish -c Release -r win-x64 --self-contained

# 🖥️ Art Asset Manager - Setup Guide

Kompletny przewodnik konfiguracji projektu dla Windows i Linux (Fedora).

---

## 📋 Wybierz System Operacyjny

- **[Windows Setup](#-windows-setup)**
- **[Linux (Fedora) Setup](#-linux-fedora-setup)**

---

# 🪟 Windows Setup

## 📋 Wymagania

- **Windows 10/11**
- **Internet connection**
- **Git** (opcjonalnie)

---

## 🚀 Szybki Start (Quick Commands)

```powershell
# 1. Pobierz i zainstaluj .NET SDK 8.0
# Przejdź do: https://dotnet.microsoft.com/download/dotnet/8.0
# Pobierz: "SDK 8.0.x (recommended)" dla Windows x64

# 2. Zainstaluj EF Core Tools
dotnet tool install --global dotnet-ef

# 3. Sklonuj repo (jeśli jeszcze nie masz)
git clone <twoje-repo-url>
cd art-asset-manager

# 4. Backend Setup
cd ArtAssetManager.Api
dotnet restore
dotnet build
dotnet ef database update

# 5. Frontend Setup
cd ..\ArtAssetManager.client
npm install

# 6. Uruchom (dwa terminale)
# Terminal 1:
cd ArtAssetManager.Api
dotnet watch run

# Terminal 2:
cd ArtAssetManager.client
npm run dev
```

---

## 📦 Krok 1: Instalacja .NET SDK 8.0

### GUI Installer (Rekomendowane)

1. Otwórz: https://dotnet.microsoft.com/download/dotnet/8.0
2. Pobierz: **SDK 8.0.x (recommended)** dla Windows x64
3. Uruchom installer
4. Zaznacz: "Add to PATH" (domyślnie zaznaczone)

### Weryfikacja

```powershell
# Otwórz PowerShell lub CMD
dotnet --version
# Oczekiwany output: 8.0.x
```

---

## 🛠️ Krok 2: Instalacja Entity Framework Core Tools

```powershell
dotnet tool install --global dotnet-ef

# Weryfikacja
dotnet ef --version
# Oczekiwany output: Entity Framework Core .NET Command-line Tools 8.0.x
```

### ⚠️ Problem: PATH nie zaktualizowany

```powershell
# Zamknij i otwórz ponownie terminal
# LUB dodaj ręcznie do PATH:
# %USERPROFILE%\.dotnet\tools
```

---

## 🖥️ Krok 3: Instalacja Node.js

1. Otwórz: https://nodejs.org/
2. Pobierz: **LTS version** (v18.x lub nowszy)
3. Uruchom installer
4. Zaznacz: "Automatically install necessary tools" (opcjonalnie)

### Weryfikacja

```powershell
node --version  # v18.x lub nowsze
npm --version   # 9.x lub nowsze
```

---

## 📂 Krok 4: Sklonowanie Projektu

### Z Git

```powershell
git clone <twoje-repo-url>
cd art-asset-manager
```

### Bez Git (ręcznie)

1. Pobierz ZIP z repozytorium
2. Rozpakuj do wybranego folderu
3. Otwórz PowerShell/CMD w tym folderze

---

## 🔧 Krok 5: Setup Backend

```powershell
# Przejdź do folderu API
cd ArtAssetManager.Api

# Restore pakietów
dotnet restore

# Build
dotnet build

# Migracja bazy
dotnet ef database update

# Uruchom
dotnet watch run

# Otwórz w przeglądarce:
# http://localhost:5244/swagger
```

---

## ⚛️ Krok 6: Setup Frontend

```powershell
# Przejdź do folderu client
cd ..\ArtAssetManager.client

# Instalacja zależności
npm install

# Uruchom dev server
npm run dev

# Otwórz w przeglądarce:
# http://localhost:5173
```

---

## 🎯 Codzienne Uruchamianie

**Terminal 1 (PowerShell) - Backend:**

```powershell
cd ArtAssetManager.Api
dotnet watch run
```

**Terminal 2 (PowerShell) - Frontend:**

```powershell
cd ArtAssetManager.client
npm run dev
```

**Zatrzymanie:** `Ctrl + C` w każdym terminalu

---

## 📚 Przydatne Komendy (Windows)

### .NET

```powershell
dotnet build                              # Build
dotnet clean                              # Clean
dotnet add package NazwaPakietu           # Dodaj pakiet
dotnet list package                       # Lista pakietów
```

### EF Core

```powershell
dotnet ef migrations add NazwaMigracji    # Nowa migracja
dotnet ef database update                 # Zastosuj migracje
dotnet ef migrations list                 # Lista migracji
```

### NPM

```powershell
npm install                               # Instalacja zależności
npm run dev                               # Dev server
npm run build                             # Build produkcyjny
```

### Git

```powershell
git status                                # Status
git add .                                 # Dodaj wszystko
git commit -m "wiadomość"                 # Commit
git push origin main                      # Push
git pull origin main                      # Pull
```

---

## 🐛 Troubleshooting (Windows)

### Problem: "dotnet: command not found"

- Zamknij i otwórz ponownie terminal
- Sprawdź instalację .NET SDK
- Sprawdź PATH: `echo $env:PATH`

### Problem: Port zajęty

```powershell
# Sprawdź który proces używa portu
netstat -ano | findstr :5244

# Zabij proces (zmień PID)
taskkill /PID <numer_pid> /F
```

### Problem: npm install fails

```powershell
# Wyczyść cache
npm cache clean --force

# Usuń node_modules
Remove-Item -Recurse -Force node_modules
Remove-Item package-lock.json

# Zainstaluj ponownie
npm install
```

---

## ✅ Weryfikacja (Windows)

```powershell
dotnet --version        # .NET SDK
dotnet ef --version     # EF Tools
node --version          # Node.js
npm --version           # npm

cd ArtAssetManager.Api
dotnet build            # Build backend

cd ..\ArtAssetManager.client
npm list --depth=0      # Sprawdź dependencies
```

---

# 🐧 Linux (Fedora) Setup

## 📋 Wymagania

- **Fedora 37+**
- **Internet connection**
- **Git**

---

## 🚀 Szybki Start (Quick Commands)

```bash
# 1. Zainstaluj .NET SDK
sudo dnf install dotnet-sdk-8.0

# 2. Zainstaluj EF Core Tools
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"

# 3. Sklonuj repo (jeśli jeszcze nie masz)
git clone <twoje-repo-url>
cd art-asset-manager

# 4. Backend Setup
cd ArtAssetManager.Api
dotnet restore
dotnet build
dotnet ef database update

# 5. Frontend Setup
cd ../ArtAssetManager.client
npm install

# 6. Uruchom (dwa terminale)
# Terminal 1:
cd ArtAssetManager.Api && dotnet watch run

# Terminal 2:
cd ArtAssetManager.client && npm run dev
```

---

## 📦 Krok 1: Instalacja .NET SDK 8.0

### Opcja A: Przez DNF (Rekomendowane)

```bash
# Zainstaluj .NET SDK
sudo dnf install dotnet-sdk-8.0

# Weryfikacja
dotnet --version
# Oczekiwany output: 8.0.x
```

### Opcja B: Oficjalny Skrypt (jeśli DNF nie działa)

```bash
# Pobierz skrypt instalacyjny
wget https://dot.net/v1/dotnet-install.sh

# Nadaj uprawnienia
chmod +x dotnet-install.sh

# Uruchom instalację
./dotnet-install.sh --channel 8.0

# Dodaj do PATH
export PATH="$PATH:$HOME/.dotnet"

# Dodaj permanentnie do .bashrc
echo 'export PATH="$PATH:$HOME/.dotnet"' >> ~/.bashrc
source ~/.bashrc

# Weryfikacja
dotnet --version
```

---

## 🛠️ Krok 2: Instalacja Entity Framework Core Tools

```bash
# Zainstaluj globalne narzędzie EF Core
dotnet tool install --global dotnet-ef

# Dodaj ścieżkę do PATH
export PATH="$PATH:$HOME/.dotnet/tools"

# Dodaj permanentnie do .bashrc
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.bashrc
source ~/.bashrc

# Weryfikacja
dotnet ef --version
# Oczekiwany output: Entity Framework Core .NET Command-line Tools 8.0.x
```

### ⚠️ Problem: "Permission denied"

```bash
chmod +x ~/.dotnet/tools/dotnet-ef
```

---

## 🖥️ Krok 3: Instalacja Node.js (dla Frontendu)

```bash
# Zainstaluj Node.js i npm
sudo dnf install nodejs npm

# Weryfikacja
node --version  # Powinno być v18.x lub nowsze
npm --version   # Powinno być 9.x lub nowsze
```

---

## 📂 Krok 4: Sklonowanie i Nawigacja

```bash
# Sklonuj repozytorium (jeśli jeszcze nie masz)
git clone <twoje-repo-url>

# Przejdź do głównego folderu projektu
cd art-asset-manager

# Sprawdź strukturę
ls -la
# Powinieneś zobaczyć:
# - art-asset-manager.sln
# - ArtAssetManager.Api/
# - ArtAssetManager.client/
# - .gitignore
```

---

## 🔧 Krok 5: Setup Backend (.NET API)

### 5.1 Restore Pakietów NuGet

```bash
# Przejdź do folderu API
cd ArtAssetManager.Api

# Przywróć wszystkie zależności
dotnet restore

# To pobierze pakiety:
# - Microsoft.EntityFrameworkCore.Design
# - Microsoft.EntityFrameworkCore.Sqlite
# - AutoMapper.Extensions.Microsoft.DependencyInjection
# - Swashbuckle.AspNetCore
```

### 5.2 Build Projektu

```bash
# Zbuduj projekt
dotnet build

# Oczekiwany output:
# Build succeeded.
#     0 Warning(s)
#     0 Error(s)
```

#### ⚠️ Jeśli są błędy kompilacji:

- Sprawdź czy wszystkie pliki zostały spullowane z Git
- Sprawdź błędy w AutoMapperProfile.cs (duplikaty CreateMap)
- Sprawdź namespace w plikach DTOs (konsystencja)

### 5.3 Migracja Bazy Danych

```bash
# Sprawdź istniejące migracje
dotnet ef migrations list

# Zastosuj migracje (stworzy assets.db)
dotnet ef database update

# Weryfikacja - sprawdź czy plik bazy istnieje
ls -la | grep assets.db
```

### 5.4 Uruchomienie Backend

```bash
# Opcja 1: Normalne uruchomienie
dotnet run

# Opcja 2: Z hot reload (lepsze dla development)
dotnet watch run

# Oczekiwany output:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: https://localhost:7270
#       Now listening on: http://localhost:5244

# Sprawdź w przeglądarce:
# http://localhost:5244/swagger
```

---

## ⚛️ Krok 6: Setup Frontend (React + Vite)

```bash
# Wróć do głównego folderu (jeśli jesteś w ArtAssetManager.Api)
cd ..

# Przejdź do folderu client
cd ArtAssetManager.client

# Zainstaluj wszystkie zależności npm
npm install

# To zainstaluje:
# - react, react-dom
# - vite
# - @tailwindcss/vite
# - typescript
# - eslint i inne dev dependencies
```

### 6.1 Uruchomienie Frontend

```bash
# Development server
npm run dev

# Oczekiwany output:
#   VITE v7.x.x  ready in xxx ms
#   ➜  Local:   http://localhost:5173/
#   ➜  Network: use --host to expose

# Otwórz w przeglądarce:
# http://localhost:5173
```

---

## 🎯 Codzienne Uruchamianie Projektu

### Dwa Terminale - Równolegle

**Terminal 1 - Backend:**

```bash
cd ArtAssetManager.Api
dotnet watch run
```

**Terminal 2 - Frontend:**

```bash
cd ArtAssetManager.client
npm run dev
```

### Zatrzymanie Serwerów

- Wciśnij `Ctrl + C` w każdym terminalu

---

## 📚 Przydatne Komendy

### .NET Build i Clean

```bash
# Build projektu
dotnet build

# Clean (usuń bin/obj)
dotnet clean

# Rebuild
dotnet clean && dotnet build

# Uruchom testy (gdy będą)
dotnet test
```

### NuGet Package Management

```bash
# Dodaj pakiet
dotnet add package NazwaPakietu

# Usuń pakiet
dotnet remove package NazwaPakietu

# Lista zainstalowanych pakietów
dotnet list package

# Aktualizuj pakiety
dotnet restore
```

### Entity Framework Core

```bash
# Stwórz nową migrację
dotnet ef migrations add NazwaMigracji

# Zastosuj migracje
dotnet ef database update

# Lista migracji
dotnet ef migrations list

# Usuń ostatnią migrację (jeśli nie była applied)
dotnet ef migrations remove

# Rollback do konkretnej migracji
dotnet ef database update NazwaPoprzednejMigracji

# Drop bazy (UWAGA: usuwa wszystkie dane!)
dotnet ef database drop
```

### NPM Commands

```bash
# Instalacja zależności
npm install

# Development server
npm run dev

# Build produkcyjny
npm run build

# Preview produkcyjnego buildu
npm run preview

# Linting
npm run lint
```

### Git Workflow

```bash
# Sprawdź status
git status

# Dodaj wszystkie zmiany
git add .

# Commit
git commit -m "feat: opis zmian"

# Push do remote
git push origin main

# Pull najnowszych zmian
git pull origin main

# Sprawdź brancha
git branch

# Stwórz nowy branch
git checkout -b feature/nazwa-feature
```

---

## 🐛 Troubleshooting (Fedora)

### Problem: "dotnet: command not found"

```bash
export PATH="$PATH:$HOME/.dotnet"
echo 'export PATH="$PATH:$HOME/.dotnet"' >> ~/.bashrc
source ~/.bashrc
```

### Problem: "dotnet-ef: command not found"

```bash
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.bashrc
source ~/.bashrc
chmod +x ~/.dotnet/tools/dotnet-ef
```

### Problem: Build errors w AutoMapper

- Usuń duplikat `CreateMap<Asset, AssetDto>` w AutoMapperProfile.cs
- Dodaj brakujące mapowania dla AssetDetailsDto i ScanFolder

### Problem: Port już zajęty

```bash
# Backend: zmień w Properties/launchSettings.json
# Frontend: Vite automatycznie użyje następnego wolnego portu
```

### Problem: SQLite błędy

```bash
sudo dnf install sqlite
rm assets.db-shm assets.db-wal
```

### Problem: npm install fails

```bash
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

---

## ✅ Weryfikacja (Fedora)

```bash
dotnet --version        # .NET SDK
dotnet ef --version     # EF Tools
node --version          # Node.js
npm --version           # npm

cd ArtAssetManager.Api
dotnet build            # Build backend

cd ../ArtAssetManager.client
npm list --depth=0      # Sprawdź dependencies
```

---

## 📁 Struktura Projektu

```
art-asset-manager/
├── art-asset-manager.sln
├── ArtAssetManager.Api/
│   ├── Program.cs
│   ├── assets.db              ← Baza SQLite
│   ├── Entities/              ← Modele bazy danych
│   ├── Data/                  ← DbContext
│   ├── DTOs/                  ← Data Transfer Objects
│   └── Migrations/            ← Migracje EF Core
└── ArtAssetManager.client/
    ├── package.json
    ├── vite.config.ts
    ├── src/
    └── node_modules/
```

---

## ✅ Checklist Setup

- [ ] .NET SDK zainstalowany (`dotnet --version`)
- [ ] EF Tools zainstalowany (`dotnet ef --version`)
- [ ] Node.js zainstalowany (`node --version`)
- [ ] Repo sklonowane
- [ ] Backend dependencies (`dotnet restore`)
- [ ] Frontend dependencies (`npm install`)
- [ ] Baza danych utworzona (`assets.db` istnieje)
- [ ] Backend działa (`http://localhost:5244/swagger`)
- [ ] Frontend działa (`http://localhost:5173`)

---

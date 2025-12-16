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
- **Bun Runtime** (wymagane do Frontendu)

---

## 🚀 Szybki Start (Quick Commands)

```powershell
# 1. Pobierz i zainstaluj .NET SDK 8.0
# Przejdź do: [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
# Pobierz: "SDK 8.0.x (recommended)" dla Windows x64

# 2. Zainstaluj EF Core Tools
dotnet tool install --global dotnet-ef

# 3. Zainstaluj Bun (jeśli nie masz)
# Przejdź do: [https://bun.com/docs/installation](https://bun.com/docs/installation) lub w PowerShell:
powershell -c "irm bun.sh/install.ps1 | iex"

# 4. Sklonuj repo (jeśli jeszcze nie masz)
git clone <twoje-repo-url>
cd art-asset-manager

# 5. Backend Setup
cd ArtAssetManager.Api
dotnet restore
dotnet build
dotnet ef database update

# 6. Frontend Setup
cd ..\ArtAssetManager.client
bun install

# 7. Uruchom (dwa terminale)
# Terminal 1 (Backend):
cd ArtAssetManager.Api
dotnet watch run

# Terminal 2 (Frontend):
cd ArtAssetManager.client
bun run dev
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

## 🥟 Krok 3: Instalacja Bun Runtime

Frontend wymaga środowiska Bun.

1. Otwórz PowerShell jako Administrator (opcjonalnie, ale zalecane).
2. Uruchom komendę instalacyjną:
   ```powershell
   powershell -c "irm bun.sh/install.ps1 | iex"
   ```
3. Alternatywnie pobierz instalator ze strony: https://bun.com/docs/installation

### Weryfikacja

```powershell
bun --version  # v1.x lub nowsze
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
# http://localhost:5270/swagger
```

---

## ⚛️ Krok 6: Setup Frontend

```powershell
# Przejdź do folderu client
cd ..\ArtAssetManager.client

# Instalacja zależności przez Bun
bun install

# Uruchom dev server
bun run dev

# Otwórz w przeglądarce:
# http://localhost:5173
```

---

## 🎯 Codzienne Uruchamianie

**Terminal 1 (PowerShell) - Backend:**

```powershell
cd ArtAssetManager.Api
dotnet watch run
# Dostępne pod: http://localhost:5270
```

**Terminal 2 (PowerShell) - Frontend:**

```powershell
cd ArtAssetManager.client
bun run dev
```

**Zatrzymanie:** `Ctrl + C` w każdym terminalu

---
---

# 🐧 Linux (Fedora) Setup

## 📋 Wymagania

- **Fedora 37+**
- **Internet connection**
- **Git**
- **Bun Runtime**

---

## 🚀 Szybki Start (Quick Commands)

```bash
# 1. Zainstaluj .NET SDK
sudo dnf install dotnet-sdk-8.0

# 2. Zainstaluj EF Core Tools
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"

# 3. Zainstaluj Bun
curl -fsSL [https://bun.sh/install](https://bun.sh/install) | bash
source ~/.bashrc

# 4. Sklonuj repo (jeśli jeszcze nie masz)
git clone <twoje-repo-url>
cd art-asset-manager

# 5. Backend Setup
cd ArtAssetManager.Api
dotnet restore
dotnet build
dotnet ef database update

# 6. Frontend Setup
cd ../ArtAssetManager.client
bun install

# 7. Uruchom (dwa terminale)
# Terminal 1:
cd ArtAssetManager.Api && dotnet watch run

# Terminal 2:
cd ArtAssetManager.client && bun run dev
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
wget [https://dot.net/v1/dotnet-install.sh](https://dot.net/v1/dotnet-install.sh)

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

## 🥟 Krok 3: Instalacja Bun Runtime (Zamiast Node.js)

Projekt wykorzystuje **Bun** jako menedżer pakietów i runtime dla frontendu.

```bash
# Zainstaluj Bun
curl -fsSL [https://bun.sh/install](https://bun.sh/install) | bash

# Dodaj do konfiguracji shella (jeśli instalator nie zrobił tego automatycznie)
# Dla Bash:
echo 'export BUN_INSTALL="$HOME/.bun"' >> ~/.bashrc
echo 'export PATH="$BUN_INSTALL/bin:$PATH"' >> ~/.bashrc
source ~/.bashrc

# Weryfikacja
bun --version
# Oczekiwany output: 1.x.x
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
```

---

## 🔧 Krok 5: Setup Backend (.NET API)

### 5.1 Restore Pakietów NuGet

```bash
# Przejdź do folderu API
cd ArtAssetManager.Api

# Przywróć wszystkie zależności
dotnet restore
```

### 5.2 Build Projektu

```bash
# Zbuduj projekt
dotnet build
```

### 5.3 Migracja Bazy Danych

```bash
# Zastosuj migracje (stworzy assets.db)
dotnet ef database update
```

### 5.4 Uruchomienie Backend

```bash
# Uruchom z hot reload
dotnet watch run

# Oczekiwany output:
# info: Microsoft.Hosting.Lifetime[14]
#        Now listening on: http://localhost:5270

# Sprawdź w przeglądarce:
# http://localhost:5270/swagger
```

---

## ⚛️ Krok 6: Setup Frontend (React + Vite)

```bash
# Wróć do głównego folderu (jeśli jesteś w ArtAssetManager.Api)
cd ..

# Przejdź do folderu client
cd ArtAssetManager.client

# Zainstaluj zależności używając Bun
bun install
```

### 6.1 Uruchomienie Frontend

```bash
# Development server via Bun
bun run dev

# Oczekiwany output:
#   VITE v7.x.x  ready in xxx ms
#   ➜  Local:   http://localhost:5173/

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
# Adres: http://localhost:5270
```

**Terminal 2 - Frontend:**

```bash
cd ArtAssetManager.client
bun run dev
```

### Zatrzymanie Serwerów

- Wciśnij `Ctrl + C` w każdym terminalu

---

## 📚 Przydatne Komendy

### Bun Commands (Frontend)

```bash
# Instalacja zależności
bun install

# Development server
bun run dev

# Build produkcyjny
bun run build

# Dodawanie pakietów
bun add nazwa-pakietu
```

### .NET & EF Core (Backend)

```bash
# Build
dotnet build

# Migracja bazy
dotnet ef database update

# Drop bazy
dotnet ef database drop
```

---

## 🐛 Troubleshooting

### Problem: "bun: command not found"

```bash
export BUN_INSTALL="$HOME/.bun"
export PATH="$BUN_INSTALL/bin:$PATH"
source ~/.bashrc
```

### Problem: Port 5270 zajęty

Sprawdź, czy nie masz uruchomionej innej instancji backendu. Jeśli chcesz zmienić port, edytuj `ArtAssetManager.Api/Properties/launchSettings.json`.

---

## ✅ Weryfikacja (Fedora)

```bash
dotnet --version        # .NET SDK
dotnet ef --version     # EF Tools
bun --version           # Bun Runtime

cd ArtAssetManager.Api
dotnet build            # Build backend

cd ../ArtAssetManager.client
bun pm ls               # Sprawdź dependencies (bun package manager list)
```

---

## ✅ Checklist Setup

- [ ] .NET SDK zainstalowany (`dotnet --version`)
- [ ] EF Tools zainstalowany (`dotnet ef --version`)
- [ ] Bun Runtime zainstalowany (`bun --version`)
- [ ] Repo sklonowane
- [ ] Backend dependencies (`dotnet restore`)
- [ ] Frontend dependencies (`bun install`)
- [ ] Baza danych utworzona (`assets.db` istnieje)
- [ ] Backend działa (`http://localhost:5270/swagger`)
- [ ] Frontend działa (`http://localhost:5173`)

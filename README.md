# Art Asset Manager

**Status:** Projekt w trakcie rozwoju (WIP).
**Typ:** Aplikacja self-hosted (Web) do indeksowania i zarządzania lokalnymi plikami graficznymi (3D/2D) bez ingerencji w strukturę folderów na dysku.

## 🛠 Stack Technologiczny

### Backend (`/ArtAssetManager.Api`)
* **Framework:** ASP.NET Core Web API (.NET 8)
* **Baza danych:** SQLite (plik lokalny) + Entity Framework Core
* **Wzorce:** Repository Pattern, Hosted Services (Background Worker)
* **Kluczowe mechanizmy:**
    * `ScannerService`: Usługa w tle skanująca system plików i wykrywająca zmiany.
    * `AssetRepository`: Warstwa dostępu do danych.

### Frontend (`/ArtAssetManager.client`)
* **Runtime:** Bun
* **Framework:** React + Vite
* **UI/Styling:** HeroUI (dawniej NextUI) + Tailwind CSS v4
* **Komunikacja:** Axios (REST API)

## 🚀 Instrukcja Uruchomienia

### Krok 1: Backend
Wymagane: .NET 8 SDK.

```bash
cd ArtAssetManager.Api

# 1. Pobranie zależności
dotnet restore

# 2. Utworzenie bazy danych SQLite (aplikacja migracji)
# To stworzy plik assets.db w folderze projektu
dotnet ef database update

# 3. Uruchomienie serwera
dotnet run
```
Domyślny adres API: https://localhost:7082 (lub http://localhost:5270)

### Krok 2: Frontend
Wymagane: Bun (lub Node.js, ale skrypty są skonfigurowane pod bun).

``bash

cd ArtAssetManager.client

# 1. Instalacja pakietów
bun install

# 2. Uruchomienie trybu deweloperskiego
bun run dev
```
Domyślny adres UI: http://localhost:5173

## 🗂 Struktura Projektu

* **`ArtAssetManager.Api/`**
    * `Controllers/` - Endpointy API (Assets, Tags, Scanner).
    * `Data/` - Kontekst bazy danych i implementacja repozytoriów.
    * `Entities/` - Modele domenowe (Asset, Tag, MaterialSet).
    * `Services/` - Logika biznesowa (skaner plików).
* **`ArtAssetManager.client/`**
    * `src/features/` - Komponenty podzielone funkcjonalnie (Gallery, Inspector).
    * `src/services/` - Logika zapytań do API.
    * `src/layouts/` - Główne elementy UI (Sidebar, Layout).

## 🧩 Zaimplementowane Funkcjonalności

1.  **File System Watcher:** Wykrywanie plików w wybranych folderach na dysku.
2.  **Galeria:** Wirtualizowana lista assetów z obsługą miniatur.
3.  **System Tagowania:** Relacja wiele-do-wielu (Asset <-> Tag).
4.  **Material Sets:** Grupowanie powiązanych plików (np. tekstury PBR) w jeden obiekt logiczny.
5.  **Inspektor:** Panel boczny ze szczegółami i edycją metadanych.

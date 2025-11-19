# 🛠️ Art Asset Manager - Backend Roadmap & Missing Features

Ten dokument definiuje brakujące funkcjonalności w API, które są niezbędne do obsługi zaprojektowanego Frontendu (UI/UX Guidelines v1.0).

## 🚨 Priorytet 1: MVP Critical (Must-Have dla działającego UI)

_Bez tych elementów Frontend będzie tylko statyczną makietą._

### 1. System Real-Time Feedback (SignalR)

Frontend ma pasek postępu ("Global Progress Indicator"). Musimy wysyłać te dane.

- [ ] **Instalacja SignalR:** Dodaj `builder.Services.AddSignalR()` i `app.MapHub<ScanHub>("/hubs/scan")`.
- [ ] **ScanHub:** Stwórz prosty hub, który pozwala klientowi dołączyć do grupy "scanners".
- [ ] **ScannerService Integration:** Wstrzyknij `IHubContext<ScanHub>` do `ScannerService`.
- [ ] **Broadcast Progress:** W pętli skanowania wysyłaj zdarzenie `ReceiveProgress` co np. 50 plików.
  - Payload: `{ totalItems: int, processedItems: int, currentFolder: string }`.

### 2. Kontrola Skanera (Manual Trigger & Schedule)

Obecnie `ScannerService` to pętla `while(true)`. Frontend ma przycisk "Scan Now".

- [ ] **Refactor ScannerService:** Zmień logikę pętli. Zamiast `Sleep(5min)`, użyj mechanizmu `Semaphore` lub `Channel`, aby czekać na sygnał.
- [ ] **Endpoint:** `POST /api/scanner/start` – wybudza skaner natychmiast (Manual Sync).
- [ ] **Endpoint:** `POST /api/scanner/stop` – (Opcjonalnie) przerywa skanowanie (Cancel Token).
- [ ] **Endpoint:** `GET /api/scanner/status` – Zwraca czy skaner aktualnie pracuje (`IsScanning`).

### 3. Walidacja Folderów (Folder Picker UX)

Frontend ma input tekstowy dla ścieżek. Musimy sprawdzić, czy wpisana ścieżka istnieje, zanim spróbujemy ją dodać.

- [ ] **Endpoint:** `POST /api/system/validate-path`
  - Input: `{ "path": "D:\\Assets" }`
  - Logic: `Directory.Exists(path)` + sprawdzenie uprawnień (try/catch).
  - Output: `{ "isValid": true, "message": "OK" }` lub błąd.

### 4. Obsługa "Smart Collections" (Zapisane Filtry)

Frontend ma sekcję "Saved Searches". Backend nie ma gdzie tego trzymać.

- [ ] **Nowa Encja:** `SavedSearch` (lub `SmartCollection`).
  - Pola: `Id`, `Name`, `FilterJson` (zserializowane `AssetQueryParameters`).
- [ ] **Repozytorium & Kontroler:** CRUD dla `SavedSearch`.

---

## 🟠 Priorytet 2: UX Polish (Wysoka wartość użytkowa)

_Te funkcje sprawiają, że aplikacja nie czuje się "tania"._

### 5. Otwieranie w Systemie (Integration)

Przycisk "Show in Explorer" w Prawym Panelu.

- [ ] **Endpoint:** `POST /api/system/open-explorer`
  - Input: `{ "path": "..." }`
  - Logic: `Process.Start("explorer.exe", "/select,\"" + path + "\"")` (Windows specific).

### 6. Dashboard Stats

Pusty stan prawego panelu ma wyświetlać statystyki ("Library Size: 120GB").

- [ ] **Endpoint:** `GET /api/stats/library`
  - Logic: Agregacja SQL (`Sum(FileSize)`, `Count()`).
  - Output: `{ "totalCount": 1240, "totalSize": 4500000000, "lastScan": "..." }`.

### 7. Color Palette Endpoint (Opcjonalne)

Dla filtrowania po kolorach.

### 7.5. System Logowania do Pliku (Serilog)

Backend wykonuje ciężkie operacje w tle. Musimy mieć historię błędów zapisaną na dysku, a nie tylko w konsoli.

- [ ] **Instalacja:** Dodaj pakiety `Serilog.AspNetCore` i `Serilog.Sinks.File`.
- [ ] **Konfiguracja:** W `Program.cs` podmień domyślny logger na Serilog (`host.UseSerilog`).
- [ ] **Appsettings:** Skonfiguruj sekcję `Serilog` -> `WriteTo` -> `File`.

  - Ścieżka: `logs/log-.txt` (z datą w nazwie).
  - RollingInterval: `Day` (codziennie nowy plik).
  - Retention: Np. trzymaj logi z ostatnich 7 dni.

- [ ] **Endpoint:** `GET /api/assets/colors` – Zwraca listę unikalnych `DominantColor` z bazy (zgrupowaną), aby Frontend wiedział, jakie kropki wyświetlić w filtrze.

---

## 🟡 Priorytet 3: Post-MVP (Planowane ulepszenia)

_To robimy, jak już podstawy będą śmigać._

### 8. Dynamiczne Rozszerzenia (Settings)

Obecnie rozszerzenia są w `appsettings.json` (Read-Only). Frontend Settings ma mieć edycję checkboxami.

- [ ] **Migracja Bazy:** Przenieś `AllowedExtensions` do nowej tabeli `SystemSettings` lub kolumny w bazie.
- [ ] **Logika Skanera:** Skaner musi pobierać listę rozszerzeń z Bazy (Repo), a nie z `IOptions<ScannerSettings>`.

### 9. File System Watcher

Automatyczne wykrywanie zmian.

- [ ] **Implementacja:** `FileSystemWatcher` w `ScannerService`.
- [ ] **Debouncing:** Logika opóźniająca skanowanie o X sekund po wykryciu zmiany, aby nie zabić bazy.

### 10. Duplicate Management

- [ ] **Endpoint:** `GET /api/assets/duplicates` – Znajduje assety z tym samym `FileHash`.

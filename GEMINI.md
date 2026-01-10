# Art Asset Manager

**Status:** Work In Progress (WIP)
**Type:** Self-hosted Web Application (Local Asset Management)
**Context:** This is a full-stack application designed to index, manage, and tag local 2D/3D art assets without modifying the underlying file structure.

## 🛠 Tech Stack

### Backend (`/ArtAssetManager.Api`)
*   **Framework:** ASP.NET Core Web API (.NET 8)
*   **Database:** SQLite (`assets.db`) managed via Entity Framework Core.
*   **Key Libraries:**
    *   `AutoMapper`: Object mapping.
    *   `FluentValidation`: Request validation.
    *   `Serilog`: Structured logging.
    *   `SignalR`: Real-time updates (scanning progress).
*   **Architecture:**
    *   **Controllers:** REST API endpoints (e.g., `AssetsController`).
    *   **Repositories:** Data access abstraction.
    *   **Services:** Background tasks (e.g., `ScannerService` for file watching).
    *   **Entities:** Rich domain models with performance indexes.

### Frontend (`/ArtAssetManager.client`)
*   **Runtime:** Bun (preferred) / Node.js
*   **Framework:** React 18 + Vite 6
*   **Language:** TypeScript
*   **Styling:** Tailwind CSS v4 + HeroUI (formerly NextUI)
*   **State Management:**
    *   `TanStack Query` (React Query): Server state & caching.
    *   `Zustand`: Client global state.
*   **Icons:** Lucide React.

## 🚀 Building & Running

### Prerequisites
*   **.NET 8 SDK**
*   **Bun** (or Node.js)
*   **EF Core Tools:** `dotnet tool install --global dotnet-ef`

### Backend
1.  Navigate to the API directory:
    ```bash
    cd ArtAssetManager.Api
    ```
2.  Apply database migrations (creates `assets.db`):
    ```bash
    dotnet ef database update
    ```
3.  Start the server (Hot Reload enabled):
    ```bash
    dotnet watch run
    ```
    *   **URL:** `http://localhost:5270`
    *   **Swagger:** `http://localhost:5270/swagger`

### Frontend
1.  Navigate to the client directory:
    ```bash
    cd ArtAssetManager.client
    ```
2.  Install dependencies:
    ```bash
    bun install
    ```
3.  Start the development server:
    ```bash
    bun run dev
    ```
    *   **URL:** `http://localhost:5173`

## 📂 Project Structure

### Backend (`ArtAssetManager.Api`)
*   **`Controllers/`**: API endpoints. `AssetsController` is the primary entry point for gallery data.
*   **`Data/`**: `AssetDbContext` configuration and Repositories (`AssetRepository`).
*   **`Entities/`**: Database models (`Asset`, `Tag`, `ScanFolder`, `MaterialSet`).
*   **`Services/`**: Background logic, specifically `ScannerService` for file system monitoring.
*   **`DTOs/`**: Data Transfer Objects for API communication.

### Frontend (`ArtAssetManager.client`)
*   **`src/features/`**: Domain-specific logic and UI.
    *   `gallery/`: Main grid view, asset cards.
    *   `inspector/`: Right sidebar for asset details and editing.
    *   `settings/`: Application configuration.
*   **`src/lib/`**: Core utilities (e.g., `axios` configuration, `react-query` setup).
*   **`src/types/`**: TypeScript definitions (e.g., `api.ts` mirroring backend DTOs).
*   **`src/layouts/`**: App shell components (`MainLayout`, Sidebar).

## 🧩 Key Features & Conventions
*   **File System Watcher:** The backend monitors configured folders for changes and updates the DB automatically.
*   **Soft Delete:** Assets are "soft deleted" (`IsDeleted = true`) before permanent removal.
*   **Material Sets:** Logic to group related textures (Albedo, Normal, Roughness) into a single logical "Material".
*   **Virtualization:** The frontend gallery handles large datasets efficiently.
*   **Absolute Imports:** The frontend uses `@/` alias for `src/`.
*   **Styling:** Utility-first CSS with Tailwind, supplemented by HeroUI components for complex interactive elements.

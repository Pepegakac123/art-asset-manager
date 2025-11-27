// ---------------------------------------------------------------------------
// CORE ENTITIES (Mapowanie 1:1 z Backend DTOs)
// Pamiętaj: C# PascalCase -> JSON camelCase
// ---------------------------------------------------------------------------

export interface Tag {
  id: number;
  name: string;
  count?: number;
  // Opcjonalne, jeśli backend zwraca kolor w DTO
  color?: string;
}

export interface ScanFolder {
  id: number;
  path: string; // W DTO: Path
  isActive: boolean; // W DTO: IsActive
  lastScanned?: string;
  isDeleted: boolean;
}

export interface Asset {
  id: number;
  fileName: string;
  fileExtension: string;
  filePath: string;
  fileType: string; // 'model', 'image', 'texture'
  fileSize: number;
  fileHash?: string | null;

  // Wizualne
  thumbnailUrl?: string | null;
  imageWidth?: number | null;
  imageHeight?: number | null;
  dominantColor?: string | null;

  // Metadane
  dateAdded: string; // DateTime w C# to string w JSON
  lastModified: string;
  lastScanned?: string;

  // User Data
  isFavorite: boolean;
  rating: number;
  description?: string | null;

  // Relacje
  scanFolderId?: number | null;
  tags: Tag[];

  // Parent/Child (Wersjonowanie)
  parentId?: number | null;

  // Flagi systemowe
  isDeleted: boolean;
}

export interface MaterialSet {
  id: number;
  name: string;
  description?: string | null;
  coverAssetId?: number | null;
  customCoverUrl?: string | null;
  dateAdded: string;
  lastModified: string;
  // Jeśli pobierasz szczegóły, backend może zwrócić listę assetów
  assets?: Asset[];
}

export interface SavedSearch {
  id: number;
  name: string;
  filterJson: string;
  dateAdded: string;
  lastUsed?: string;
}

export interface LibraryStats {
  totalAssets: number;
  totalSize: number;
  totalTags: number;
  lastScanDate?: string | null;
}

// ---------------------------------------------------------------------------
// API RESPONSES (Wrappery)
// ---------------------------------------------------------------------------

export interface PagedResponse<T> {
  items: T[];
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// ---------------------------------------------------------------------------
// API REQUESTS (Payloads & Query Params)
// WAŻNE: Tu były błędy. Teraz jest zgodnie z Twoimi plikami .cs
// ---------------------------------------------------------------------------

// GET /api/assets
export interface AssetQueryParams {
  // Paginacja
  pageNumber?: number;
  pageSize?: number;

  // Logika filtrowania
  matchAll?: boolean; // false = OR, true = AND (dla tagów)

  // Sortowanie
  sortBy?: string; // np. 'DateAdded', 'Rating'
  sortDesc?: boolean; // true = Malejąco

  // Filtry Tekstowe
  fileName?: string; // To jest Twoja "szukajka" wg C#

  // Filtry Listowe
  fileType?: string[]; // backend: List<string> FileType
  tags?: string[]; // backend: List<string> Tags
  dominantColors?: string[]; // backend: List<string> DominantColors

  // Zakresy (Ranges)
  fileSizeMin?: number; // backend: long?
  fileSizeMax?: number;
  ratingMin?: number;
  ratingMax?: number;
  dateFrom?: string; // DateTime?
  dateTo?: string;

  // Wymiary obrazu
  minWidth?: number;
  maxWidth?: number;
  minHeight?: number;
  maxHeight?: number;

  // Specjalne
  fileHash?: string;
  hasAlphaChannel?: boolean;
}
// PATCH /api/assets/{id} -> DTOs/PatchAssetRequest.cs
export interface UpdateAssetRequest {
  rating?: number;
  isFavorite?: boolean;
  description?: string;
}

// POST /api/assets/bulk-tag -> DTOs/BulkUpdateAssetTagsRequest.cs
export interface BulkUpdateAssetTagsRequest {
  assetIds: number[];
  tagsToAdd?: string[];
  tagsToRemove?: string[];
}

// POST /api/settings/folders -> DTOs/AddScanFolderRequest.cs
// 🔥 TUTAJ BYŁ BŁĄD 400. Backend ma public string FolderPath { get; set; }
export interface AddScanFolderRequest {
  folderPath: string;
}

// PATCH /api/settings/folders/{id} -> DTOs/UpdateFolderStatusRequest.cs
// Backend ma public bool IsActive { get; set; }
export interface UpdateScanFolderStatusRequest {
  isActive: boolean;
}

// POST /api/system/validate-path -> DTOs/ValidatePathRequest.cs
// Zakładam, że tam jest public string Path { get; set; }
export interface ValidatePathRequest {
  path: string;
}

// POST /api/material-sets -> DTOs/CreateMaterialSets.cs
export interface CreateMaterialSetRequest {
  name: string;
  description?: string;
  initialAssetIds?: number[];
}

export interface GetOrSetAllowedExtensions {
  extensions: string[];
}

export interface ScanStatus {
  status: "scanning" | "idle";
}

export interface SidebarStats {
  totalAssets: number;
  totalFavorites: number;
  totalUncategorized: number;
  totalTrashed: number;
}

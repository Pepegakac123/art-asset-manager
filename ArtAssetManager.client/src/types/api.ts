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
	thumbnailPath?: string | null;
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
	pageNumber: number;
	totalPages: number;
	totalCount: number;
	hasPreviousPage: boolean;
	hasNextPage: boolean;
}

// ---------------------------------------------------------------------------
// API REQUESTS (Payloads & Query Params)
// WAŻNE: Tu były błędy. Teraz jest zgodnie z Twoimi plikami .cs
// ---------------------------------------------------------------------------

// GET /api/assets
export interface AssetQueryParams {
	pageNumber?: number;
	pageSize?: number;
	searchTerm?: string;

	// Filtry
	fileTypes?: string[];
	tags?: string[];
	scanFolderIds?: number[];

	// Logiczne
	isFavorite?: boolean;
	isDeleted?: boolean;
	withoutTags?: boolean;

	// Zakresy
	minRating?: number;
	maxRating?: number;
	dateFrom?: string;
	dateTo?: string;

	orderBy?: string;
	materialSetId?: number;
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

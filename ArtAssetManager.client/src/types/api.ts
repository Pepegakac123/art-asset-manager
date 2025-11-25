// ---------------------------------------------------------------------------
// CORE ENTITIES (Odzwierciedlenie DTO z Backendu)
// ---------------------------------------------------------------------------

export interface Tag {
	id: number;
	name: string;
	count?: number;
	dateCreated?: string;
}

export interface ScanFolder {
	id: number;
	path: string;
	isActive: boolean;
	dateAdded: string;
	isDeleted: boolean;
}

export interface Asset {
	id: number;
	fileName: string;
	fileExtension: string; // .blend, .fbx
	filePath: string;
	fileType: string; // 'model', 'image', 'texture'
	fileSize: number;
	fileHash?: string | null;

	// Wizualne
	thumbnailPath?: string | null; // Relatywna ścieżka, np. "thumbnails/xyz.jpg"
	dominantColor?: string | null;
	imageWidth?: number | null;
	imageHeight?: number | null;

	// Metadane
	dateAdded: string; // ISO Date
	lastModified: string; // ISO Date
	lastScanned: string; // ISO Date

	// User Data
	isFavorite: boolean;
	rating: number;
	description?: string | null;

	// Relacje
	scanFolderId?: number | null;
	tags: Tag[];

	// Parent/Child (dla wersji)
	parentAssetId?: number | null;

	// Flagi systemowe
	isDeleted: boolean;
}

export interface MaterialSet {
	id: number;
	name: string;
	description?: string | null;
	coverAssetId?: number | null;
	customCoverUrl?: string | null; // Jeśli user ustawił własną okładkę
	dateAdded: string;
	lastModified: string;
	assets?: Asset[]; // Opcjonalnie, jeśli pobieramy szczegóły kolekcji
}

export interface SavedSearch {
	id: number;
	name: string;
	filterJson: string; // JSON string z zapisanymi filtrami
	dateAdded: string;
}

export interface LibraryStats {
	totalAssets: number;
	totalSize: number; // w bajtach
	totalTags: number;
	lastScanDate?: string;
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
// API REQUESTS (Query Params & Payloads)
// ---------------------------------------------------------------------------

// Parametry wyszukiwania (GET /api/assets)
export interface AssetQueryParams {
	pageNumber?: number;
	pageSize?: number;

	// Wyszukiwanie tekstowe
	searchTerm?: string;

	// Filtrowanie
	fileTypes?: string[]; // np. ['model', 'image']
	tags?: string[]; // lista nazw tagów lub ID
	scanFolderIds?: number[];

	// Filtry logiczne
	isFavorite?: boolean;
	isDeleted?: boolean; // Dla kosza
	withoutTags?: boolean; // Dla "Uncategorized"

	// Zakresy (dla filtrów technicznych)
	minRating?: number;
	maxRating?: number;
	dateFrom?: string;
	dateTo?: string;

	// Sortowanie
	orderBy?: string; // np. 'DateAdded_Desc', 'Name_Asc'

	// Kontekst Kolekcji
	materialSetId?: number; // Jeśli przeglądamy konkretną kolekcję
}

// Payload do aktualizacji assetu (PATCH /api/assets/{id})
export interface UpdateAssetRequest {
	rating?: number;
	isFavorite?: boolean;
	description?: string;
}

// Payload do tworzenia folderu skanowania
export interface CreateScanFolderRequest {
	path: string;
}

export interface ValidatePathRequest {
	path: string;
}

export interface AddScanFolderRequest {
	path: string;
}
export interface UpdateScanFolderStatusRequest {
	isActive: boolean;
}

import { create } from "zustand";
import { devtools } from "zustand/middleware";

// Interfejs reprezentujący filtry dostępne w UI.
// Odpowiada polom w klasie C# `AssetQueryParameters`.
export interface GalleryFilters {
  searchQuery: string; // Odpowiednik C#: FileName
  tags: string[]; // Odpowiednik C#: Tags
  matchAllTags: boolean; // Odpowiednik C#: MatchAll (true = AND, false = OR)
  fileTypes: string[]; // Odpowiednik C#: FileType (image, model, texture)
  colors: string[]; // Odpowiednik C#: DominantColors

  ratingRange: [number, number]; // Slider UI zwraca tablicę [min, max]
  dateRange: {
    // Odpowiednik C#: DateFrom, DateTo
    from: string | null;
    to: string | null;
  };

  // Filtrowanie techniczne (wymiary, rozmiar pliku)
  widthRange: [number, number];
  heightRange: [number, number];
  fileSizeRange: [number, number]; // W Megabajtach
  hasAlpha: boolean | null; // null = wszystko, true = tylko z alpha, false = bez alpha
}

export type SortOption = "dateadded" | "filename" | "filesize" | "lastmodified";

// Główny stan galerii (Global State) zarządzany przez Zustand
interface GalleryState {
  // --- UI State (Wygląd) ---
  zoomLevel: number; // Wielkość kafelków (px)
  viewMode: "grid" | "masonry"; // Układ siatki
  pageSize: number; // Ilość elementów na stronę (do paginacji backendowej)

  // --- Data State (Dane) ---
  filters: GalleryFilters; // Aktualnie aktywne filtry
  sortOption: SortOption; // Po czym sortujemy
  sortDesc: boolean; // Kierunek sortowania (C#: SortDesc)

  // --- Selection State (Zaznaczanie) ---
  selectedAssetIds: Set<number>; // Zbiór ID zaznaczonych plików (Set dla wydajności O(1))
  lastSelectedAssetId: number | null; // Ostatnio kliknięty element (do obsługi Shift+Click)

  // --- Actions (Metody zmieniające stan) ---
  setZoomLevel: (zoom: number) => void;
  setViewMode: (mode: "grid" | "masonry") => void;
  setPageSize: (size: number) => void;

  // Partial pozwala aktualizować tylko wybrane pole filtra (np. tylko tagi, bez ruszania reszty)
  setFilters: (newFilters: Partial<GalleryFilters>) => void;

  // Sortowanie
  setSortOption: (option: SortOption) => void;
  toggleSortDirection: () => void;

  // Reset filtrów do wartości domyślnych
  resetFilters: () => void;

  // Obsługa zaznaczania (multi = czy trzymamy Ctrl/Shift)
  selectAsset: (id: number, multi: boolean) => void;
  setSelection: (ids: number[]) => void;
  clearSelection: () => void;
}

// Domyślne wartości filtrów (startowe)
const DEFAULT_FILTERS: GalleryFilters = {
  searchQuery: "",
  tags: [],
  matchAllTags: true, // Domyślnie szukamy plików zawierających WSZYSTKIE wybrane tagi
  fileTypes: [],
  colors: [],
  ratingRange: [0, 5],
  dateRange: { from: null, to: null },
  widthRange: [0, 8192],
  heightRange: [0, 8192],
  fileSizeRange: [0, 4096],
  hasAlpha: null,
};

// Utworzenie Store'a Zustand
export const useGalleryStore = create<GalleryState>()(
  devtools((set) => ({
    // Initial State
    zoomLevel: 250,
    viewMode: "grid",
    pageSize: 20,
    filters: DEFAULT_FILTERS,
    sortOption: "dateadded", // Domyślnie najnowsze
    sortDesc: true, // Domyślnie malejąco (najnowsze na górze)
    selectedAssetIds: new Set<number>(),
    lastSelectedAssetId: null,

    // Actions Implementation
    setZoomLevel: (zoom) => set({ zoomLevel: zoom }),
    setViewMode: (mode) => set({ viewMode: mode }),
    setPageSize: (size) => set({ pageSize: size }),

    // Scalanie starych filtrów z nowymi (Shallow Merge)
    setFilters: (newFilters) =>
      set((state) => ({
        filters: { ...state.filters, ...newFilters },
      })),

    setSortOption: (option) => set({ sortOption: option }),

    toggleSortDirection: () => set((state) => ({ sortDesc: !state.sortDesc })),

    resetFilters: () => set({ filters: DEFAULT_FILTERS }),

    // Logika zaznaczania
    selectAsset: (id, multi) => {
      if (multi) {
        // Jeśli multi (np. z Ctrl), dodajemy do istniejącego zbioru
        set((state) => ({
          selectedAssetIds: new Set([...state.selectedAssetIds, id]),
          lastSelectedAssetId: id,
        }));
      } else {
        // Jeśli pojedyncze kliknięcie, czyścimy resztę i zaznaczamy tylko ten jeden
        set({ selectedAssetIds: new Set([id]), lastSelectedAssetId: id });
      }
    },

    setSelection: (ids) =>
      set({
        selectedAssetIds: new Set(ids),
        lastSelectedAssetId: ids[0] || null,
      }),

    clearSelection: () =>
      set({ selectedAssetIds: new Set(), lastSelectedAssetId: null }),
  })),
);
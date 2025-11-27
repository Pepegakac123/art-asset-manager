import { create } from "zustand";
import { devtools } from "zustand/middleware";

export interface GalleryFilters {
  searchQuery: string; // C#: FileName
  tags: string[]; // C#: Tags
  matchAllTags: boolean; // C#: MatchAll
  fileTypes: string[]; // C#: FileType
  colors: string[]; // C#: DominantColors

  ratingRange: [number, number]; // C#: RatingMin, RatingMax (Slider UI zwraca tablicę)
  dateRange: {
    // C#: DateFrom, DateTo
    from: string | null;
    to: string | null;
  };

  // Wymiary (Opcjonalne)
  widthRange: [number, number]; // C#: MinWidth, MaxWidth
  heightRange: [number, number]; // C#: MinHeight, MaxHeight

  hasAlpha: boolean | null; // C#: HasAlphaChannel (null = wszystko, true = z, false = bez)
}

// 2. Definicja Sortowania (switch w C#)
export type SortOption = "dateadded" | "filename" | "filesize" | "lastmodified";

interface GalleryState {
  // --- UI State ---
  zoomLevel: number;
  viewMode: "grid" | "masonry";

  // --- Data State ---
  filters: GalleryFilters;
  sortOption: SortOption;
  sortDesc: boolean; // C#: SortDesc

  // --- Actions ---
  setZoomLevel: (zoom: number) => void;
  setViewMode: (mode: "grid" | "masonry") => void;

  // Update filtrów (Partial pozwala aktualizować tylko jedno pole np. tylko tags)
  setFilters: (newFilters: Partial<GalleryFilters>) => void;

  // Helpersy do sortowania
  setSortOption: (option: SortOption) => void;
  toggleSortDirection: () => void;

  // Reset
  resetFilters: () => void;
}

const DEFAULT_FILTERS: GalleryFilters = {
  searchQuery: "",
  tags: [],
  matchAllTags: true,
  fileTypes: [],
  colors: [],
  ratingRange: [0, 5],
  dateRange: { from: null, to: null },
  widthRange: [0, 8192],
  heightRange: [0, 8192],
  hasAlpha: null,
};

export const useGalleryStore = create<GalleryState>()(
  devtools((set) => ({
    // Initial State
    zoomLevel: 250,
    viewMode: "grid",

    filters: DEFAULT_FILTERS,
    sortOption: "dateadded", // Default z C#
    sortDesc: true, // Default z C# (OrderByDescending)

    // Actions
    setZoomLevel: (zoom) => set({ zoomLevel: zoom }),
    setViewMode: (mode) => set({ viewMode: mode }),

    setFilters: (newFilters) =>
      set((state) => ({
        filters: { ...state.filters, ...newFilters },
      })),

    setSortOption: (option) => set({ sortOption: option }),

    toggleSortDirection: () => set((state) => ({ sortDesc: !state.sortDesc })),

    resetFilters: () => set({ filters: DEFAULT_FILTERS }),
  })),
);

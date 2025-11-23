import { TOPBAR_SETTINGS } from "@/Enums";
import { create } from "zustand";

type viewModeType = "grid" | "masonry" | "list";

interface GalleryState {
	/*
TODO: [STATE] Refactor Selection for Multi-Select
- Zmienić `selectedAssetId: number | null` na `selectedAssetIds: Set<number>` (lub array).
- UI Guidelines (Sekcja 7.5): Potrzebujemy tego do "Floating Action Bar" (Taguj X assetów, Usuń X assetów).
- Dodać akcje: `toggleSelection(id)`, `clearSelection()`, `selectAll()`.

TODO: [FILTER] Add Technical Filters State
- UI Guidelines (Sekcja 6.4): Dodać stan dla filtrów technicznych:
  - ratingRange: [min, max]
  - fileTypes: string[] (np. ['model', 'image'])
  - dateRange: DateRange
*/

	// --- UI STATE ---
	zoomLevel: number;
	viewMode: viewModeType;
	sortOption: string;

	// --- FILTER STATE ---
	selectedAssetId: number | null;
	selectedTags: string[];
	isStrictMode: boolean;

	// --- ACTIONS ---
	setZoomLevel: (level: number) => void;
	setViewMode: (mode: viewModeType) => void;
	setSortOption: (option: string) => void;
	setSelectedAssetId: (id: number | null) => void;
	toggleTag: (tag: string) => void;
	removeTag: (tag: string) => void;
	setStrictMode: (isActive: boolean) => void;
}

export const useGalleryStore = create<GalleryState>((set) => ({
	zoomLevel: TOPBAR_SETTINGS.DEFAULT_ZOOM_LEVEL,
	viewMode: "grid",
	sortOption: "newest",
	selectedAssetId: null,
	selectedTags: [],
	isStrictMode: true,

	setZoomLevel: (level) => set({ zoomLevel: level }),
	setViewMode: (mode) => set({ viewMode: mode }),
	setSortOption: (option) => set({ sortOption: option }),
	setSelectedAssetId: (id) => set({ selectedAssetId: id }),

	toggleTag: (tag) =>
		set((state) => {
			const exists = state.selectedTags.includes(tag);
			return {
				selectedTags: exists
					? state.selectedTags.filter((t) => t !== tag)
					: [...state.selectedTags, tag],
			};
		}),

	removeTag: (tag) =>
		set((state) => ({
			selectedTags: state.selectedTags.filter((t) => t !== tag),
		})),

	setStrictMode: (isActive) => set({ isStrictMode: isActive }),
}));

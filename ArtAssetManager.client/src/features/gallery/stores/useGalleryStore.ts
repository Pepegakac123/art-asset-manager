import { create } from "zustand";

type viewModeType = "grid" | "masonry" | "list";

interface GalleryState {
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
	zoomLevel: 150,
	viewMode: "grid",
	sortOption: "newest",
	selectedAssetId: null,
	selectedTags: [],
	isStrictMode: false,

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

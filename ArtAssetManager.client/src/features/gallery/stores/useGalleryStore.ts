import { create } from "zustand";

type viewModeType = "grid" | "masonry" | "list";
interface GalleryState {
	zoomLevel: number;
	viewMode: viewModeType;
	sortOption: string;
	selectedAssetId: number | null;

	setZoomLevel: (level: number) => void;
	setViewMode: (mode: viewModeType) => void;
	setSortOption: (option: string) => void;
	setSelectedAssetId: (id: number | null) => void;
}

export const useGalleryStore = create<GalleryState>((set) => ({
	zoomLevel: 150,
	viewMode: "grid",
	sortOption: "newest",
	selectedAssetId: null,

	setZoomLevel: (level) => set({ zoomLevel: level }),
	setViewMode: (mode) => set({ viewMode: mode }),
	setSortOption: (option) => set({ sortOption: option }),
	setSelectedAssetId: (id) => set({ selectedAssetId: id }),
}));

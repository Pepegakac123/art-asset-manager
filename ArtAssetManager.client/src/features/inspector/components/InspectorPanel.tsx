import { Button } from "@heroui/button";
import { ScrollShadow } from "@heroui/scroll-shadow";
import { MousePointerClick, X } from "lucide-react"; // Dodałem X do zamykania (opcjonalnie)
import { useGalleryStore } from "../../gallery/stores/useGalleryStore";

export const InspectorPanel = () => {
	const selectedAssetId = useGalleryStore((state) => state.selectedAssetId);
	const setSelectedAssetId = useGalleryStore(
		(state) => state.setSelectedAssetId,
	);

	/*
TODO: [FEATURE] Multi-Select Inspector (Carousel Mode)
- Logic Switch:
  - IF selectedAssetIds.length === 1 -> Show Single Asset Details (Current view).
  - IF selectedAssetIds.length > 1 -> Show Multi-Edit Mode.
- Multi-Edit UI:
  - Header: "X items selected".
  - Navigation: Carousel/Pager to cycle through selected items (Prev/Next buttons).
  - Bulk Edit Form: Fields common to all assets (e.g. adding a Tag to all of them).
  - "Apply to All" button for changes.
*/

	/*
TODO: [FEATURE] Full Implementation (Guidelines Sec 8)
- Tab System: Zaimplementować zakładki: "Info", "File", "Versions".
- Data Fetching: Po zmianie `selectedAssetId`, pobrać pełne detale (GET /api/assets/{id}/details).
- Editable Fields: Pola "Rating", "Notes", "Tags" muszą być edycyjne (Auto-save on blur).

TODO: [PREVIEW] 3D Model Viewer
- Plan (Parking Lot / Future): Zostawić miejsce w DOM na integrację <Canvas> z Three.js dla plików .glb/.gltf.
*/

	// --- RENDER: EMPTY STATE ---
	if (selectedAssetId === null) {
		return (
			<div className="h-full w-full flex flex-col items-center justify-center gap-4 text-default-400 p-8 text-center">
				<div className="flex h-24 w-24 items-center justify-center rounded-full bg-default-100/50">
					<MousePointerClick size={48} strokeWidth={1} />
				</div>
				<div className="space-y-1">
					<p className="text-lg font-medium text-foreground">Select an asset</p>
					<p className="text-sm">
						Click on any item in the gallery to inspect details and metadata.
					</p>
				</div>
			</div>
		);
	}

	// --- RENDER: ACTIVE STATE (SZKIELET) ---
	return (
		<div className="flex h-full flex-col">
			{/* NAGŁÓWEK TYMCZASOWY */}
			<div className="flex h-16 shrink-0 items-center justify-between border-b border-default-200 px-4">
				<div>
					<p className="text-xs font-bold uppercase text-default-500">
						Selected Asset
					</p>
					<h2 className="text-xl font-bold text-primary">
						ID: {selectedAssetId}
					</h2>
				</div>
				<Button
					isIconOnly
					size="sm"
					variant="light"
					onPress={() => setSelectedAssetId(null)}
					aria-label="Close inspector"
				>
					<X size={20} />
				</Button>
			</div>

			{/* SCROLLOWALNA ZAWARTOŚĆ */}
			<ScrollShadow className="flex-1 p-4">
				<div className="rounded-lg border border-dashed border-default-300 bg-default-100/50 p-8 text-center text-sm text-default-500">
					Tu wylądują zakładki: Info, File, Versions.
					<br />
					(Implementacja w następnej fazie)
				</div>
			</ScrollShadow>
		</div>
	);
};

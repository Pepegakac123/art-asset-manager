import { useParams } from "react-router-dom";
import { useGalleryStore } from "../stores/useGalleryStore";
import { AssetCard } from "./AssetCard"; // Import nowej karty

// --- LEPSZY GENERATOR DANYCH ---
const generateItems = (count: number) => {
	return Array.from({ length: count }, (_, i) => ({
		id: i,
		title: `SciFi_Prop_v${i}.blend`,
		type: i % 4 === 0 ? "BLEND" : i % 3 === 0 ? "FBX" : "PNG",
		img: `https://picsum.photos/seed/${i + 120}/400/400`, // Random images
		isFavorite: i % 7 === 0, // Co 7 element jest ulubiony
	}));
};

/*
TODO: [API] Integration with React Query
- Zastąpić `generateItems` hookiem `useInfiniteQuery` z biblioteki @tanstack/react-query.
- Endpoint: GET /api/assets (z parametrami: page, pageSize, tags, filters).

TODO: [UX] Infinite Scroll / Load More
- UI Guidelines (Sekcja 7.4): Zaimplementować mechanizm "Load More" lub Infinite Scroll.
- Obecnie renderujemy wszystko naraz - przy 10k assetów przeglądarka wybuchnie bez wirtualizacji siatki (np. @tanstack/react-virtual) LUB paginacji.

TODO: [LAYOUT] Masonry Layout Support
- UI Guidelines (Sekcja 7.2): Dodać obsługę trybu `viewMode === 'masonry'`.
- Obecnie Grid jest sztywny (`aspect-square`). W Masonry kafelki mają różną wysokość.

TODO: [UX] Bulk Actions Bar (Floating)
- UI Guidelines (Sekcja 7.5): Dodać warunkowe renderowanie komponentu <FloatingActionBar /> na dole ekranu, gdy `selectedAssetIds.length > 0`.
*/

/*
TODO: [UX] Floating Bulk Actions Dock
- Requirements:
  - Show ONLY when selectedAssetIds.length > 1.
  - Fixed position: bottom-8, centered (z-index: 50).
  - Visuals: Glassmorphism, dark background, rounded-full.
  - Actions: "Select All", "Clear Selection", "Add to Collection", "Delete", "Tag Selected".
  - Animation: Slide-up entry animation (Framer Motion or Tailwind animate-in).
*/

type DisplayContentMode =
	| "default"
	| "favorites"
	| "uncategorized"
	| "trash"
	| "collection";
interface GalleryGridProps {
	mode: DisplayContentMode;
}

export const GalleryGrid = ({ mode }: GalleryGridProps) => {
	const params = useParams();

	const zoomLevel = useGalleryStore((state) => state.zoomLevel);

	// Generujemy 50 elementów
	const items = generateItems(50);

	return (
		<div className="h-full w-full">
			<div
				style={{ "--col-width": `${zoomLevel}px` } as React.CSSProperties}
				className="grid grid-cols-[repeat(auto-fill,minmax(var(--col-width),1fr))] gap-4 pb-20 p-4"
			>
				{items.map((item) => (
					<AssetCard
						key={item.id}
						id={item.id}
						title={item.title}
						type={item.type}
						thumbnailUrl={item.img}
						isFavorite={item.isFavorite}
					/>
				))}
			</div>
		</div>
	);
};

import { useParams } from "react-router-dom";
import { useGalleryStore } from "../stores/useGalleryStore";
import { AssetCard } from "./AssetCard";
import { Spinner } from "@heroui/spinner";
import type { UI_CONFIG } from "@/config/constants";
import { AssetQueryParams } from "@/types/api";
import { useMemo } from "react";
import { useAssets } from "../hooks/useAssets";
import { useShallow } from "zustand/react/shallow";
// --- LEPSZY GENERATOR DANYCH ---
// const generateItems = (count: number) => {
//   return Array.from({ length: count }, (_, i) => ({
//     id: i,
//     title: `SciFi_Prop_v${i}.blend`,
//     type: i % 4 === 0 ? "BLEND" : i % 3 === 0 ? "FBX" : "PNG",
//     img: `https://picsum.photos/seed/${i + 120}/400/400`, // Random images
//     isFavorite: i % 7 === 0, // Co 7 element jest ulubiony
//   }));
// };

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
  keyof typeof UI_CONFIG.GALLERY.AllowedDisplayContentModes;

interface GalleryGridProps {
  mode: DisplayContentMode;
}

export const GalleryGrid = ({ mode }: GalleryGridProps) => {
  const { collectionId } = useParams<{ collectionId: string }>();
  const parsedCollectionId = collectionId ? parseInt(collectionId) : undefined;

  const { zoomLevel, viewMode, filters, sortOption, sortDesc } =
    useGalleryStore(
      useShallow((state) => ({
        zoomLevel: state.zoomLevel,
        viewMode: state.viewMode,
        filters: state.filters,
        sortOption: state.sortOption,
        sortDesc: state.sortDesc,
      })),
    );

  const queryParams: AssetQueryParams = useMemo(() => {
    return {
      pageNumber: 1, // TODO: Paginacja w następnym kroku
      pageSize: 50, // Na start sztywno, potem dynamicznie

      // --- FILTRY ---
      fileName: filters.searchQuery || undefined,
      tags: filters.tags.length > 0 ? filters.tags : undefined,
      matchAll: filters.matchAllTags,

      fileType: filters.fileTypes.length > 0 ? filters.fileTypes : undefined,
      dominantColors: filters.colors.length > 0 ? filters.colors : undefined,

      ratingMin: filters.ratingRange[0],
      ratingMax: filters.ratingRange[1],

      dateFrom: filters.dateRange.from || undefined,
      dateTo: filters.dateRange.to || undefined,

      sortBy: sortOption,
      sortDesc: sortDesc,
    };
  }, [filters, sortOption, sortDesc]);

  const { data, isLoading, isError, error, openExplorer } = useAssets(
    mode,
    queryParams,
    parsedCollectionId,
  );

  // 5. Renderowanie Stanów
  if (isLoading) {
    return (
      <div className="flex h-full w-full items-center justify-center">
        <Spinner size="lg" label="Loading assets..." color="primary" />
      </div>
    );
  }

  if (isError) {
    return (
      <div className="flex h-full w-full flex-col items-center justify-center text-danger">
        <p className="text-xl font-bold">Błąd ładowania galerii</p>
        <p className="text-sm opacity-70">
          {(error as Error).message ||
            (error as any)?.response?.data?.message ||
            (error as any)?.response?.data?.error}
        </p>
      </div>
    );
  }

  const assets = data?.items || [];
  console.log(assets[0]);

  if (assets.length === 0) {
    return (
      <div className="flex h-full w-full flex-col items-center justify-center text-default-500">
        <p className="text-lg">Pusto tutaj...</p>
        <p className="text-sm">Brak assetów spełniających kryteria.</p>
      </div>
    );
  }

  return (
    <div className="h-full w-full">
      <div
        style={{ "--col-width": `${zoomLevel}px` } as React.CSSProperties}
        className="grid grid-cols-[repeat(auto-fill,minmax(var(--col-width),1fr))] gap-4 pb-20 p-4"
      >
        {assets.map((asset) => (
          <div key={asset.id} className="aspect-square">
            <AssetCard asset={asset} explorerfn={openExplorer} />
          </div>
        ))}
      </div>
    </div>
  );
};

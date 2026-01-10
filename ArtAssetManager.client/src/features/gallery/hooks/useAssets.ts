import {
  useQuery,
  keepPreviousData,
  useMutation,
  useInfiniteQuery,
} from "@tanstack/react-query";
import { assetService } from "@/services/assetService";
import { AssetQueryParams } from "@/types/api";
import { UI_CONFIG } from "@/config/constants";
import { addToast } from "@heroui/toast";

type GalleryMode = keyof typeof UI_CONFIG.GALLERY.AllowedDisplayContentModes;

// Hook do pobierania listy assetów z obsługą nieskończonego przewijania (Infinite Scroll)
export const useAssets = (
  mode: GalleryMode,
  params: AssetQueryParams,
  collectionId?: number,
) => {
  // useInfiniteQuery zarządza stronicowaniem danych
  const getAssetsQuery = useInfiniteQuery({
    // Klucz cache zależy od trybu, filtrów i kolekcji - zmiana któregokolwiek wymusi odświeżenie
    queryKey: ["assets", mode, params, collectionId],
    initialPageParam: 1,
    
    // Funkcja pobierająca dane dla konkretnej strony
    queryFn: async ({ pageParam = 1 }) => {
      const currentParams = { ...params, pageNumber: pageParam as number };
      
      // Wybór odpowiedniego endpointu w zależności od trybu galerii
      switch (mode) {
        case UI_CONFIG.GALLERY.AllowedDisplayContentModes.favorites:
          return assetService.getFavorites(currentParams);
        case UI_CONFIG.GALLERY.AllowedDisplayContentModes.trash:
          return assetService.getTrashed(currentParams);
        case UI_CONFIG.GALLERY.AllowedDisplayContentModes.collection:
          if (!collectionId) throw new Error("Brak ID kolekcji");
          return assetService.getAssetsForMaterialSet(
            collectionId,
            currentParams,
          );
        case UI_CONFIG.GALLERY.AllowedDisplayContentModes.uncategorized:
          return assetService.getUncategorizedAssets(currentParams);
        default:
          return assetService.getAll(currentParams);
      }
    },
    // Logika wyznaczania następnej strony (dla React Query)
    getNextPageParam: (lastPage) => {
      if (lastPage.hasNextPage) {
        return lastPage.currentPage + 1;
      }
      return undefined;
    },
    // Zachowaj poprzednie dane podczas ładowania nowych filtrów (lepsze UX)
    placeholderData: keepPreviousData,
    // Pobieraj tylko jeśli mamy ID kolekcji (dla trybu kolekcji) lub zawsze dla innych trybów
    enabled: mode === "collection" ? !!collectionId : true,
    staleTime: 1000 * 60 * 1, // Cache ważny przez 1 minutę
  });

  // Mutacja do otwierania folderu w eksploratorze systemowym
  const openExplorerMutation = useMutation({
    mutationFn: (filePath: string) => assetService.openInExplorer(filePath),
    onSuccess: () => {
      console.log("Explorer opened successfully");
    },
    onError: (error: any) => {
      addToast({
        title: "Błąd Systemu",
        description:
          error.response?.data?.message || "Nie udało się otworzyć folderu.",
        color: "danger",
      });
    },
  });

  return {
    ...getAssetsQuery,
    openExplorer: openExplorerMutation.mutate,
  };
};

// Hook do pobierania listy dostępnych kolorów dla filtrów
export const useColors = () => {
  const colorsQuery = useQuery({
    queryKey: ["colors"],
    queryFn: assetService.getColorsList,
  });

  return colorsQuery.data;
};
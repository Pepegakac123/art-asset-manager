import { useQuery, keepPreviousData, useMutation } from "@tanstack/react-query";
import { assetService } from "@/services/assetService";
import { AssetQueryParams } from "@/types/api";
import { UI_CONFIG } from "@/config/constants";
import { addToast } from "@heroui/toast";

type GalleryMode = keyof typeof UI_CONFIG.GALLERY.AllowedDisplayContentModes;

export const useAssets = (
  mode: GalleryMode,
  params: AssetQueryParams,
  collectionId?: number,
) => {
  const getAssetsQuery = useQuery({
    queryKey: ["assets", mode, params, collectionId],
    queryFn: async () => {
      switch (mode) {
        case "favorites":
          return assetService.getFavorites(params);
        case "trash":
          return assetService.getTrashed(params);
        case "collection":
          if (!collectionId) throw new Error("Brak ID kolekcji");
          return assetService.getAssetsForMaterialSet(collectionId, params);
        case "uncategorized":
          return assetService.getAll(params); // TODO: Dodać filtr uncategorized
        default:
          return assetService.getAll(params);
      }
    },
    placeholderData: keepPreviousData,
    enabled: mode === "collection" ? !!collectionId : true,
    staleTime: 1000 * 60 * 1,
  });

  // 2. AKCJA: OTWIERANIE FOLDERU (Mutation)
  const openExplorerMutation = useMutation({
    mutationFn: (filePath: string) => assetService.openInExplorer(filePath),
    onSuccess: () => {
      // Opcjonalnie: Toast sukcesu, ale zazwyczaj okno otwiera się po prostu
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

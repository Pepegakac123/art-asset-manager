import { useMutation, useQueryClient } from "@tanstack/react-query";
import { assetService } from "@/services/assetService";
import { Asset } from "@/types/api";
import { addToast } from "@heroui/toast";

// Hook grupujący szybkie akcje na assecie (Ulubione, Otwórz w Explorerze)
export const useAssetActions = (assetId: number) => {
  const queryClient = useQueryClient();

  // AKCJA 1: Przełącz Ulubione (z Optimistic UI)
  const favoriteMutation = useMutation({
    mutationFn: () => assetService.toggleFavorite(assetId),

    // Optymistyczna aktualizacja UI (zmiana koloru serduszka natychmiast)
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: ["asset", assetId] });

      const previousAsset = queryClient.getQueryData<Asset>(["asset", assetId]);
      if (previousAsset) {
        queryClient.setQueryData<Asset>(["asset", assetId], {
          ...previousAsset,
          isFavorite: !previousAsset.isFavorite,
        });
      }

      return { previousAsset };
    },

    onError: (_err, _vars, context) => {
      // Rollback w razie błędu
      if (context?.previousAsset) {
        queryClient.setQueryData(["asset", assetId], context.previousAsset);
      }
      addToast({
        title: "Error",
        description: "Failed to toggle favorite",
        color: "danger",
      });
    },

    onSettled: () => {
      // Odśwież wszystko co może zależeć od statusu ulubionego
      queryClient.invalidateQueries({ queryKey: ["asset", assetId] });
      queryClient.invalidateQueries({ queryKey: ["assets"] });
      queryClient.invalidateQueries({ queryKey: ["favorites"] });
      queryClient.invalidateQueries({ queryKey: ["sidebar-stats"] });
    },
  });

  // AKCJA 2: Otwórz folder zawierający plik
  const explorerMutation = useMutation({
    mutationFn: (path: string) => assetService.openInExplorer(path),
    onError: () => {
      addToast({
        title: "System Error",
        description: "Could not open explorer.",
        color: "danger",
      });
    },
  });

  // AKCJA 3: Otwórz plik w domyślnym programie
  const programMutation = useMutation({
    mutationFn: (path: string) => assetService.openInProgram(path),
    onError: () => {
      addToast({
        title: "System Error",
        description: "Could not open file.",
        color: "danger",
      });
    },
    onSuccess: () => {
      addToast({
        title: "Success",
        description: "File is opening it may take a second.",
        color: "success",
      });
    },
  });

  return {
    toggleFavorite: favoriteMutation.mutate,
    openInExplorer: explorerMutation.mutate,
    openInProgram: programMutation.mutate,
    isTogglingFav: favoriteMutation.isPending,
  };
};
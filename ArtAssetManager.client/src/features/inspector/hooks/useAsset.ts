import { useMutation, useQueryClient, useQuery } from "@tanstack/react-query";
import { assetService } from "@/services/assetService";
import { Asset } from "@/types/api";
import { addToast } from "@heroui/toast";

// Hook pobierający szczegóły pojedynczego assetu do Inspektora
export const useAsset = (assetId: number | null) => {
  return useQuery({
    queryKey: ["asset", assetId],
    queryFn: () => assetService.getById(assetId!),
    // Fetchuj tylko jak mamy ID (unikamy zapytań dla null)
    enabled: !!assetId,
    staleTime: 1000 * 60 * 5, // Cache ważny przez 5 minut
  });
};

// Hook do edycji metadanych assetu (PATCH) z optymistycznym update'em
export const useAssetMutation = (assetId: number) => {
  const queryClient = useQueryClient();

  const patchMutation = useMutation({
    mutationFn: (updates: Partial<Asset>) =>
      assetService.patch(assetId, updates),

    // OPTYMISTYCZNY UPDATE: Zaktualizuj UI zanim serwer odpowie
    onMutate: async (updates) => {
      // Anuluj trwające pobierania, żeby nie nadpisały naszej zmiany
      await queryClient.cancelQueries({ queryKey: ["asset", assetId] });
      
      // Zapisz stan poprzedni (do cofnięcia w razie błędu)
      const previousAsset = queryClient.getQueryData<Asset>(["asset", assetId]);

      // Ręcznie zaktualizuj cache React Query
      if (previousAsset) {
        queryClient.setQueryData<Asset>(["asset", assetId], {
          ...previousAsset,
          ...updates,
        });
      }

      return { previousAsset };
    },

    // W razie błędu przywróć poprzedni stan
    onError: (_err, _updates, context) => {
      if (context?.previousAsset) {
        queryClient.setQueryData(["asset", assetId], context.previousAsset);
      }
      addToast({
        title: "Update Failed",
        description: "Could not update asset properties.",
        color: "danger",
      });
    },

    // Po zakończeniu (sukces lub błąd) odśwież dane z serwera dla pewności
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ["asset", assetId] });
      queryClient.invalidateQueries({ queryKey: ["assets"] }); // Odśwież też listę w galerii
    },
  });

  return {
    patch: patchMutation.mutate,
    isUpdating: patchMutation.isPending,
  };
};
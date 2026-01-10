import { useMutation, useQueryClient } from "@tanstack/react-query";
import { assetService } from "@/services/assetService";
import { addToast } from "@heroui/toast";

// Hook do aktualizacji tagów assetu
export const useAssetTagsMutation = (assetId: number) => {
  const queryClient = useQueryClient();

  const tagsMutation = useMutation({
    mutationFn: (newTags: string[]) =>
      assetService.updateTags(assetId, newTags),

    onSuccess: () => {
      // Po sukcesie odświeżamy dane assetu, listę w galerii oraz globalną listę tagów
      queryClient.invalidateQueries({ queryKey: ["asset", assetId] });
      queryClient.invalidateQueries({ queryKey: ["assets"] });
      queryClient.invalidateQueries({ queryKey: ["tags"] });
    },

    onError: (error) => {
      console.error(error);
      addToast({
        title: "Error",
        description: "Failed to update tags.",
        color: "danger",
      });
    },
  });

  return {
    updateTags: tagsMutation.mutate,
    isUpdating: tagsMutation.isPending,
  };
};
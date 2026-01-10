import { assetService } from "@/services/assetService";
import { useQuery } from "@tanstack/react-query";

// Hook do pobierania statystyk paska bocznego (liczniki ulubionych, nieotagowanych itp.)
export const useAssetsStats = () => {
  const getSidebarStats = useQuery({
    queryKey: ["sidebar-stats"],
    queryFn: () => assetService.getSidebarStats(),
  });
  return {
    sidebarStats: getSidebarStats.data,
    isLoading: getSidebarStats.isLoading,
  };
};
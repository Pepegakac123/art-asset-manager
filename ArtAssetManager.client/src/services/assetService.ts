import apiReq from "@/lib/axios";
import {
  Asset,
  PagedResponse,
  AssetQueryParams,
  SidebarStats,
} from "@/types/api";

export const assetService = {
  getAll: async (params: AssetQueryParams): Promise<PagedResponse<Asset>> => {
    const response = await apiReq.get("/assets", { params });
    return response.data;
  },
  getFavorites: async (
    params: AssetQueryParams,
  ): Promise<PagedResponse<Asset>> => {
    const response = await apiReq.get("/assets/favorites", { params });
    return response.data;
  },
  getById: async (id: string): Promise<Asset> => {
    const response = await apiReq.get(`/assets/${id}`);
    return response.data;
  },
  getTrashed: async (
    params: AssetQueryParams,
  ): Promise<PagedResponse<Asset>> => {
    const response = await apiReq.get("/assets/deleted", { params });
    return response.data;
  },
  getUncategorizedAssets: async (
    params: AssetQueryParams,
  ): Promise<PagedResponse<Asset>> => {
    const response = await apiReq.get("/assets/uncategorized", { params });
    return response.data;
  },
  getAssetsForMaterialSet: async (
    setId: number,
    params: AssetQueryParams,
  ): Promise<PagedResponse<Asset>> => {
    // Endpoint: /api/materialsets/{id}/assets
    const response = await apiReq.get(`/materialsets/${setId}/assets`, {
      params,
    });
    return response.data;
  },
  getSidebarStats: async (): Promise<SidebarStats> => {
    const response = await apiReq.get("/stats/sidebar");
    return response.data;
  },
  openInExplorer: async (path: string) => {
    return apiReq.post("/system/open-in-explorer", { path });
  },
};

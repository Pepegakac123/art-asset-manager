import apiReq from "@/lib/axios";
import {
  Asset,
  PagedResponse,
  AssetQueryParams,
  SidebarStats,
} from "@/types/api";

// Serwis do komunikacji z API Assetów (odpowiednik AssetsController.cs)
export const assetService = {
  // Pobiera główną listę assetów z uwzględnieniem filtrów (params)
  getAll: async (params: AssetQueryParams): Promise<PagedResponse<Asset>> => {
    const response = await apiReq.get("/assets", { params });
    return response.data;
  },
  
  // Pobiera tylko ulubione (skrót do /assets/favorites)
  getFavorites: async (
    params: AssetQueryParams,
  ): Promise<PagedResponse<Asset>> => {
    const response = await apiReq.get("/assets/favorites", { params });
    return response.data;
  },
  
  // Szczegóły pojedynczego pliku
  getById: async (id: number): Promise<Asset> => {
    const response = await apiReq.get(`/assets/${id}`);
    return response.data;
  },
  
  // Pobiera kosz (pliki z flagą IsDeleted=true)
  getTrashed: async (
    params: AssetQueryParams,
  ): Promise<PagedResponse<Asset>> => {
    const response = await apiReq.get("/assets/deleted", { params });
    return response.data;
  },
  
  // Pobiera pliki bez tagów (do szybkiego tagowania)
  getUncategorizedAssets: async (
    params: AssetQueryParams,
  ): Promise<PagedResponse<Asset>> => {
    const response = await apiReq.get("/assets/uncategorized", { params });
    return response.data;
  },
  
  // Edycja metadanych (nazwa, opis, ocena) - metoda PATCH wysyła tylko zmienione pola
  patch: async (id: number, updates: Partial<Asset>): Promise<Asset> => {
    const response = await apiReq.patch<Asset>(`/assets/${id}`, updates);
    return response.data;
  },
  
  // Aktualizacja tagów dla konkretnego assetu
  updateTags: async (id: number, tagsNames: string[]): Promise<void> => {
    await apiReq.post(`/assets/${id}/tags`, { tagsNames });
  },
  
  // Pobiera assety należące do konkretnej kolekcji materiałów
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
  
  // Dodawanie assetu do kolekcji
  addAssetToMaterialSet: async (
    setId: number,
    assetId: number,
  ): Promise<void> => {
    await apiReq.post(`/materialsets/${setId}/assets/${assetId}`);
  },
  
  // Usuwanie assetu z kolekcji
  removeAssetFromMaterialSet: async (
    setId: number,
    assetId: string,
  ): Promise<void> => {
    await apiReq.delete(`/materialsets/${setId}/assets/${assetId}`);
  },
  
  // Statystyki dla paska bocznego (liczniki ulubionych, nieotagowanych itp.)
  getSidebarStats: async (): Promise<SidebarStats> => {
    const response = await apiReq.get("/stats/sidebar");
    return response.data;
  },
  
  // Lista unikalnych kolorów dominujących (do filtra kolorów)
  getColorsList: async (): Promise<string[]> => {
    const response = await apiReq.get("/assets/colors");
    return response.data;
  },
  
  // Szybkie przełączanie flagi ulubione (Heart icon)
  toggleFavorite: async (id: number): Promise<void> => {
    await apiReq.patch(`/assets/${id}/toggle-favorite`);
  },
  
  // Otwarcie lokalizacji pliku w systemowym eksploratorze (Windows/Linux/Mac)
  openInExplorer: async (path: string) => {
    return apiReq.post("/system/open-in-explorer", { path });
  },
  
  // Otwarcie pliku w domyślnym programie
  openInProgram: async (path: string) => {
    return apiReq.post("/system/open-in-program", { path });
  },
};
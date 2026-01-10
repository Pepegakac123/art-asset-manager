import apiReq from "@/lib/axios";
import { CreateMaterialSetRequest, MaterialSet } from "@/types/api";

// Serwis do zarządzania kolekcjami materiałów (MaterialSetsController.cs)
export const materialSetService = {
  // Pobiera listę wszystkich kolekcji
  getAll: async (): Promise<MaterialSet[]> => {
    const response = await apiReq.get("/materialsets");
    return response.data;
  },
  
  // Pobiera szczegóły jednej kolekcji
  getById: async (id: string): Promise<MaterialSet> => {
    const response = await apiReq.get(`/materialsets/${id}`);
    return response.data;
  },
  
  // Tworzy nową kolekcję
  create: async (
    materialSet: CreateMaterialSetRequest,
  ): Promise<MaterialSet> => {
    const response = await apiReq.post("/materialsets", materialSet);
    return response.data;
  },
  
  // Aktualizuje nazwę/opis kolekcji
  update: async (
    id: string,
    materialSet: MaterialSet,
  ): Promise<MaterialSet> => {
    const response = await apiReq.put(`/materialsets/${id}`, materialSet);
    return response.data;
  },
  
  // Usuwa kolekcję (ale nie usuwa plików z dysku!)
  delete: async (id: string): Promise<void> => {
    await apiReq.delete(`/materialsets/${id}`);
  },
};
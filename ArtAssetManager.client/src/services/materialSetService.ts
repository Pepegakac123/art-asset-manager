import apiReq from "@/lib/axios";
import { CreateMaterialSetRequest, MaterialSet } from "@/types/api";

export const materialSetService = {
  getAll: async (): Promise<MaterialSet[]> => {
    const response = await apiReq.get("/materialsets");
    return response.data;
  },
  getById: async (id: string): Promise<MaterialSet> => {
    const response = await apiReq.get(`/materialsets/${id}`);
    return response.data;
  },
  create: async (
    materialSet: CreateMaterialSetRequest,
  ): Promise<MaterialSet> => {
    const response = await apiReq.post("/materialsets", materialSet);
    return response.data;
  },
  update: async (
    id: string,
    materialSet: MaterialSet,
  ): Promise<MaterialSet> => {
    const response = await apiReq.put(`/materialsets/${id}`, materialSet);
    return response.data;
  },
  delete: async (id: string): Promise<void> => {
    await apiReq.delete(`/materialsets/${id}`);
  },
};

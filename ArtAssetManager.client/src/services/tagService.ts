import apiReq from "@/lib/axios";
import { Tag } from "@/types/api";

// Serwis do pobierania słownika tagów (używane w autouzupełnianiu)
export const tagService = {
  getAll: async (): Promise<Tag[]> => {
    const response = await apiReq.get("/tags");
    return response.data;
  },
};
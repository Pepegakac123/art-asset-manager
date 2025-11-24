import apiReq from "@/lib/axios";
import { Asset, PagedResponse, AssetQueryParams } from "@/types/api";

export const assetService = {
	getAll: async (params: AssetQueryParams): Promise<PagedResponse<Asset>> => {
		const response = await apiReq.get("/assets", { params });
		return response.data;
	},
};

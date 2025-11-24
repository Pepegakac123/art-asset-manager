import apiReq from "@/lib/axios";
import { AddScanFolderRequest, ScanFolder } from "@/types/api";

export const scannerService = {
	getFolders: async () => {
		const { data } = await apiReq.get<ScanFolder[]>("/scanner/folders");
		return data;
	},
	addFolder: async (path: string) => {
		return apiReq.post("/scanner/folders", { path } as AddScanFolderRequest);
	},
	deleteFolder: async (id: number) => {
		return apiReq.delete(`/scanner/folders/${id}`);
	},
	validatePath: async (path: string) => {
		const { data } = await apiReq.post<{ isValid: boolean }>(
			"/scanner/validate-path",
			{ path },
		);
		return data;
	},
	startScan: async () => {
		return apiReq.post("/scanner/start");
	},
};

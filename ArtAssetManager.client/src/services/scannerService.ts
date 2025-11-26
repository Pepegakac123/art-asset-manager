import apiReq from "@/lib/axios";
import {
	AddScanFolderRequest,
	ScanFolder,
	UpdateScanFolderStatusRequest,
} from "@/types/api";

export const scannerService = {
	getFolders: async () => {
		const { data } = await apiReq.get<ScanFolder[]>("/settings/folders");
		return data;
	},
	addFolder: async (path: string) => {
		return apiReq.post("/settings/folders", {
			folderPath: path,
		} as AddScanFolderRequest);
	},
	deleteFolder: async (id: number) => {
		return apiReq.delete(`/settings/folders/${id}`);
	},
	updateFolderStatus: async (id: number, isActive: boolean) => {
		return apiReq.patch<ScanFolder>(`/settings/folders/${id}`, {
			isActive,
		} as UpdateScanFolderStatusRequest);
	},
	validatePath: async (path: string) => {
		const { data } = await apiReq.post<{ isValid: boolean }>(
			"/system/validate-path",
			{ path },
		);
		return data;
	},
	startScan: async () => {
		return apiReq.post("/scanner/start");
	},
	getAllowedExtensions: async () => {
		const { data } = await apiReq.get<string[]>("/settings/extensions");
		return data;
	},
	updateAllowedExtensions: async (extensions: string[]) => {
		return apiReq.post("/settings/extensions", extensions);
	},
	openInExplorer: async (path: string) => {
		return apiReq.post("/system/open-in-explorer", { path });
	},
};

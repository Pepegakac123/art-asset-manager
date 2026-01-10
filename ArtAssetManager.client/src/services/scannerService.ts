import apiReq from "@/lib/axios";
import {
  AddScanFolderRequest,
  ScanFolder,
  UpdateScanFolderStatusRequest,
} from "@/types/api";

// Serwis do zarządzania ustawieniami skanera i folderami (SettingsController.cs & ScannerController.cs)
export const scannerService = {
  // Pobiera listę monitorowanych folderów
  getFolders: async () => {
    const { data } = await apiReq.get<ScanFolder[]>("/settings/folders");
    return data;
  },
  
  // Dodaje nowy folder do monitorowania
  addFolder: async (path: string) => {
    return apiReq.post("/settings/folders", {
      folderPath: path,
    } as AddScanFolderRequest);
  },
  
  // Usuwa folder z monitorowania
  deleteFolder: async (id: number) => {
    return apiReq.delete(`/settings/folders/${id}`);
  },
  
  // Włącza/wyłącza skanowanie konkretnego folderu
  updateFolderStatus: async (id: number, isActive: boolean) => {
    return apiReq.patch<ScanFolder>(`/settings/folders/${id}`, {
      isActive,
    } as UpdateScanFolderStatusRequest);
  },
  
  // Sprawdza czy ścieżka istnieje na serwerze (walidacja przed dodaniem)
  validatePath: async (path: string) => {
    const { data } = await apiReq.post<{ isValid: boolean }>(
      "/system/validate-path",
      { path },
    );
    return data;
  },
  
  // Ręczne wymuszenie skanowania ("Scan Now")
  startScan: async () => {
    return apiReq.post("/scanner/start");
  },
  
  // Pobiera listę obsługiwanych rozszerzeń plików (np. .jpg, .obj)
  getAllowedExtensions: async () => {
    const { data } = await apiReq.get<string[]>("/settings/extensions");
    return data;
  },
  
  // Zapisuje nową listę rozszerzeń
  updateAllowedExtensions: async (extensions: string[]) => {
    return apiReq.post("/settings/extensions", extensions);
  },
  
  // Otwarcie folderu w systemowym eksploratorze
  openInExplorer: async (path: string) => {
    return apiReq.post("/system/open-in-explorer", { path });
  },
};
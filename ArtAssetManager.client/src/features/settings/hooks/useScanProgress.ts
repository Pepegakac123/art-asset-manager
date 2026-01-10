import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import apiReq from "@/lib/axios";
import { useQueryClient } from "@tanstack/react-query";
import { API_BASE_URL } from "@/config/constants";

interface ScanState {
  isScanning: boolean;
  progress: number;
  message: string;
  total: number;
  current: number;
}

// Hook obsługujący połączenie WebSocket (SignalR) do odbierania postępu skanowania
export const useScanProgress = () => {
  const queryClient = useQueryClient();
  const [scanState, setScanState] = useState<ScanState>({
    isScanning: false, 
    progress: 0,
    message: "",
    total: 0,
    current: 0,
  });

  // 1. Sprawdzenie stanu początkowego (czy skaner już działa po odświeżeniu strony?)
  useEffect(() => {
    const fetchInitialStatus = async () => {
      try {
        const { data } = await apiReq.get<{ isScanning: boolean }>(
          "/scanner/status",
        );
        if (data.isScanning) {
          setScanState((prev) => ({
            ...prev,
            isScanning: true,
            message: "Resuming scanner connection...",
          }));
        }
      } catch (err) {
        console.error("Failed to fetch scanner status", err);
      }
    };

    fetchInitialStatus();
  }, []);

  // 2. Konfiguracja połączenia SIGNALR
  useEffect(() => {
    const cleanBaseUrl = API_BASE_URL.replace(/\/$/, "");
    const hubUrl = `${cleanBaseUrl}/hubs/scan`;
    
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .configureLogging(signalR.LogLevel.Error) // Mniej logów w konsoli
      .withAutomaticReconnect() // Automatyczne wznawianie połączenia
      .build();

    // HANDLER 1: Tylko zmiana stanu (Start/Stop)
    connection.on("ReceiveScanStatus", (status: string) => {
      const isNowScanning = status === "scanning";
      
      // Jeśli skanowanie się zakończyło, odświeżamy dane w aplikacji
      if (!isNowScanning) {
        console.log(
          "✅ Scan finished (Idle signal received). Refreshing data...",
        );
        queryClient.invalidateQueries({ queryKey: ["assets"] });
        queryClient.invalidateQueries({ queryKey: ["sidebar-stats"] });
        queryClient.invalidateQueries({ queryKey: ["colors"] });
      }
      
      setScanState((prev) => {
        return {
          ...prev,
          isScanning: isNowScanning,
          // Jeśli startujemy, resetujemy progress. Jeśli kończymy, zostawiamy 100% na chwilę dla efektu wizualnego
          progress: isNowScanning ? 0 : 100,
        };
      });
    });

    // HANDLER 2: Ciągła aktualizacja paska postępu (co X plików)
    connection.on(
      "ReceiveProgress",
      (msg: string, total: number, current: number) => {
        setScanState((prev) => ({
          ...prev,
          message: msg,
          total: total,
          current: current,
          progress: total > 0 ? (current / total) * 100 : 0,
        }));
      },
    );

    connection.start().catch((err) => console.error("SignalR Error:", err));

    return () => {
      connection.stop();
    };
  }, []);

  return scanState;
};
import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import apiReq from "@/lib/axios";

interface ScanState {
	isScanning: boolean;
	progress: number;
	message: string;
	total: number;
	current: number;
}

export const useScanProgress = () => {
	const [scanState, setScanState] = useState<ScanState>({
		isScanning: false, // Domyślnie false, ale zaraz to sprawdzimy
		progress: 0,
		message: "",
		total: 0,
		current: 0,
	});

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

	// 2. SIGNALR (Nasłuchiwanie)
	useEffect(() => {
		const connection = new signalR.HubConnectionBuilder()
			.withUrl("http://localhost:5244/hubs/scan")
			.configureLogging(signalR.LogLevel.Error) // Mniej logów w konsoli
			.withAutomaticReconnect()
			.build();

// HANDLER 1: Tylko zmiana stanu (Start/Stop)
    connection.on("ReceiveScanStatus", (status: string) => {
      const isNowScanning = status === "scanning";
      
      setScanState((prev) => {
        // Logika "Detect Edge": Jeśli wcześniej skanował, a teraz przestał -> SUKCES
        // if (prev.isScanning && !isNowScanning) {
        //    addToast({
        //      title: "Scan Finished",
        //      description: "Library has been updated.",
        //      color: "success",
        //    });
        // }
        return {
          ...prev,
          isScanning: isNowScanning,
          // Jeśli startujemy, resetujemy progress. Jeśli kończymy, zostawiamy 100% na chwilę
          progress: isNowScanning ? 0 : 100, 
        };
      });
    });

    // HANDLER 2: Tylko aktualizacja paska postępu
    connection.on("ReceiveProgress", (msg: string, total: number, current: number) => {
      setScanState((prev) => ({
        ...prev,
        message: msg,
        total: total,
        current: current,
        progress: total > 0 ? (current / total) * 100 : 0,
      }));
    });

		connection.start().catch((err) => console.error("SignalR Error:", err));

		return () => {
			connection.stop();
		};
	}, []);

	return scanState;
};

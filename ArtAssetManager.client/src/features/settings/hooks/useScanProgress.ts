import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import apiReq from "@/lib/axios"; // Twój instancja axios

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

		connection.on(
			"ReceiveProgress",
			(message: string, total: number, current: number) => {
				const percent = total > 0 ? Math.round((current / total) * 100) : 0;

				if (message === "Scan Finished") {
					console.log("🛑 Scan Finished Signal Received"); // Debug
					setTimeout(() => {
						setScanState((prev) => ({
							...prev,
							isScanning: false,
							progress: 0,
							message: "",
						}));
					}, 1000);
					return;
				}
				setScanState((prev) => {
					// Jeśli już skończyliśmy, a to nie jest nowy start (current > 0), ignoruj.
					if (
						!prev.isScanning &&
						current > 0 &&
						message !== "Initializing scan..."
					) {
						return prev;
					}

					return {
						isScanning: true, // Tutaj włączamy
						message,
						total,
						current,
						progress: percent,
					};
				});
			},
		);

		connection.start().catch((err) => console.error("SignalR Error:", err));

		return () => {
			connection.stop();
		};
	}, []);

	return scanState;
};

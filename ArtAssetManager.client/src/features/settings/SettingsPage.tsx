import React, { useState } from "react";
import { Card, CardBody, CardHeader } from "@heroui/card";
import { Button } from "@heroui/button";
import { Input } from "@heroui/input";
import { Spacer } from "@heroui/spacer";
import { Divider } from "@heroui/divider";
import { Chip } from "@heroui/chip";
import { Snippet } from "@heroui/snippet";
import { Progress } from "@heroui/progress";
import {
	FolderPlus,
	Trash2,
	Folder,
	Play,
	StopCircle,
	AlertCircle,
	CheckCircle2,
	Search,
} from "lucide-react";
// Tutaj później zaimportujesz swój hook: import { useScanFolders } from "./hooks/useScanFolders";

export default function SettingsPage() {
	// --- MIEJSCE NA TWOJĄ LOGIKĘ (Brain) ---
	// const { folders, addFolder, ... } = useScanFolders();
	const [pathInput, setPathInput] = useState("");

	// Mockowe dane (usuniesz je, gdy podepniesz hooka)
	const isScanning = false;
	const foldersMock = [
		{ id: 1, path: "D:\\3D_Assets\\Models", isActive: true },
		{ id: 2, path: "E:\\Textures\\Megascans", isActive: false },
	];

	return (
		<div className="w-full mx-auto p-6 space-y-8">
			{/* SEKCJA 1: HEADER & SCANNER CONTROL */}
			<div className="flex flex-col md:flex-row justify-between items-center gap-4">
				<div>
					<h1 className="text-3xl font-bold tracking-tight">
						Library Settings
					</h1>
					<p className="text-default-500">
						Manage your asset folders and scanner status.
					</p>
				</div>

				{/* Tu będzie przycisk START/STOP */}
				<Card className="w-full md:w-auto border-none bg-content2">
					<CardBody className="flex flex-row items-center gap-4 p-3">
						<div className="flex flex-col">
							<span className="text-xs font-semibold uppercase text-default-500">
								Scanner Status
							</span>
							<span
								className={`text-sm font-bold ${isScanning ? "text-success" : "text-warning"}`}
							>
								{isScanning ? "RUNNING" : "IDLE"}
							</span>
						</div>
						<Divider orientation="vertical" className="h-8" />
						<Button
							color={isScanning ? "danger" : "primary"}
							startContent={
								isScanning ? <StopCircle size={18} /> : <Play size={18} />
							}
						>
							{isScanning ? "Stop Scan" : "Scan Now"}
						</Button>
					</CardBody>
				</Card>
			</div>

			{/* SEKCJA 2: ADD NEW FOLDER (Hybrid Input) */}
			<Card className="w-full overflow-visible" shadow="sm">
				<CardHeader className="flex flex-col items-start px-6 pt-6 pb-0">
					<h4 className="text-large font-bold">Add Source Folder</h4>
					<p className="text-small text-default-500">
						Path must be accessible by the server.
					</p>
				</CardHeader>
				<CardBody className="px-6 py-6">
					<div className="flex gap-2 items-top">
						<Input
							value={pathInput}
							onChange={(e) => setPathInput(e.target.value)}
							placeholder="Paste absolute path (e.g. D:\Assets\SciFi)"
							startContent={
								<FolderPlus className="text-default-400" size={20} />
							}
							// Tutaj wejdzie logika walidacji (color="success" | "danger")
							description="Backend will validate if path exists on blur."
							className="flex-1"
							size="lg"
							variant="bordered"
							// endContent={... spinner albo checkmark ...}
						/>
						<Button
							size="lg"
							color="primary"
							isDisabled={!pathInput} // Tu dodasz warunek walidacji
							variant="solid"
						>
							Add Library
						</Button>
					</div>
				</CardBody>
			</Card>

			{/* SEKCJA 3: FOLDER LIST */}
			<div>
				<div className="flex justify-between items-center mb-4">
					<h3 className="text-xl font-semibold flex items-center gap-2">
						<Folder size={20} /> Linked Folders
						<Chip size="sm" variant="flat">
							{foldersMock.length}
						</Chip>
					</h3>
				</div>

				<div className="grid grid-cols-1 gap-4">
					{/* Tu zrobisz .map() po prawdziwych folderach */}
					{foldersMock.map((folder) => (
						<Card
							key={folder.id}
							isPressable
							className="hover:bg-content2 transition-colors"
						>
							<CardBody className="flex flex-row items-center justify-between p-4 gap-4">
								{/* IKONA + ŚCIEŻKA */}
								<div className="flex items-center gap-4 overflow-hidden flex-1">
									<div
										className={`p-3 rounded-xl ${folder.isActive ? "bg-primary/10 text-primary" : "bg-default-100 text-default-400"}`}
									>
										<Folder size={24} />
									</div>
									<div className="flex flex-col overflow-hidden">
										<Snippet
											symbol=""
											className="bg-transparent p-0 text-medium font-medium truncate w-full"
											codeString={folder.path}
										>
											{folder.path}
										</Snippet>
										<span className="text-tiny text-default-400">
											{folder.isActive
												? "Monitoring active"
												: "Monitoring paused"}
										</span>
									</div>
								</div>

								{/* AKCJE */}
								<div className="flex items-center gap-2">
									{/* Tutaj wejdzie Switch albo Checkbox do "isActive" */}
									<Button
										isIconOnly
										variant="light"
										color={folder.isActive ? "success" : "default"}
									>
										{folder.isActive ? <CheckCircle2 /> : <AlertCircle />}
									</Button>

									<Divider orientation="vertical" className="h-6" />

									<Button isIconOnly color="danger" variant="light">
										<Trash2 size={20} />
									</Button>
								</div>
							</CardBody>
						</Card>
					))}
				</div>
			</div>
		</div>
	);
}

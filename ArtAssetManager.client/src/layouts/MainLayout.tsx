import { Outlet } from "react-router-dom";
import { TopToolbar } from "@/features/gallery/components/TopToolbar";
import { InspectorPanel } from "@/features/inspector/components/InspectorPanel";
import { Sidebar } from "./sidebar/Sidebar";
import { useScanProgress } from "@/features/settings/hooks/useScanProgress";
import { Progress } from "@heroui/progress";

export const MainLayout = () => {
	// const { isScanning, progress, message } = useScanProgress();
	return (
		// GRID DEFINITION:
		// cols-[260px_1fr_320px] -> Lewy (sztywny), Środek (rozciągliwy), Prawy (sztywny)
		// rows-[64px_1fr] -> Góra (sztywna), Dół (rozciągliwy)
		<div
			className="grid h-screen w-screen overflow-hidden bg-background text-foreground 
                    grid-cols-[300px_1fr_320px] grid-rows-[64px_1fr]"
		>
			{/* OBSZAR 1: LEWY SIDEBAR (Rozciągnięty na wysokość - row-span-2) */}
			<div className="row-span-2 h-full">
				<Sidebar />
			</div>

			{/* OBSZAR 2: TOP BAR (Tylko górny rząd) */}
			<header className="col-start-2 col-end-3">
				<TopToolbar />
			</header>

			{/* OBSZAR 3: MAIN CONTENT (Środek - tutaj ląduje Outlet) */}
			<main className="relative col-start-2 col-end-3 overflow-hidden bg-background">
				{/* ScrollArea: To jest jedyne miejsce, które ma się scrollować! */}
				<div className="h-full w-full overflow-y-auto p-6 custom-scrollbar">
					<Outlet />
				</div>
				{/* {isScanning && (
					<div className="absolute bottom-6 left-1/2 -translate-x-1/2 w-[90%] max-w-2xl z-50">
						<div className="bg-content1/90 backdrop-blur-md border border-default-200 shadow-xl rounded-2xl p-4 flex flex-col gap-3 animate-slide-up-fade">
							<div className="flex justify-between items-center px-1">
								<div className="flex flex-col">
									<span className="text-sm font-bold text-foreground">
										Library Scan in Progress
									</span>
									<span className="text-xs text-default-500 font-mono truncate max-w-[300px]">
										{message || "Initializing..."}
									</span>
								</div>
								<span className="text-sm font-bold text-primary">
									{progress}%
								</span>
							</div>

							<Progress
								size="sm"
								value={progress}
								color="primary"
								isStriped
								isIndeterminate={progress === 0}
								aria-label="Scanning progress"
								className="max-w-full"
							/>
						</div>
					</div>
				)} */}
			</main>

			{/* OBSZAR 4: INSPECTOR (Prawy panel - też row-span-2) */}
			<aside className="row-span-2 col-start-3 col-end-4 border-t border-default-200 mt-[-1px]  ">
				<InspectorPanel />
			</aside>
		</div>
	);
};

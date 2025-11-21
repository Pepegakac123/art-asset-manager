import { Outlet } from "react-router-dom";
import { TopToolbar } from "@/features/gallery/components/TopToolbar";
import { InspectorPanel } from "@/features/inspector/components/InspectorPanel";
import { Sidebar } from "./sidebar/Sidebar";

export const MainLayout = () => {
	return (
		// GRID DEFINITION:
		// cols-[260px_1fr_320px] -> Lewy (sztywny), Środek (rozciągliwy), Prawy (sztywny)
		// rows-[64px_1fr] -> Góra (sztywna), Dół (rozciągliwy)
		<div
			className="grid h-screen w-screen overflow-hidden bg-background text-foreground 
                    grid-cols-[260px_1fr_320px] grid-rows-[64px_1fr]"
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
			</main>

			{/* OBSZAR 4: INSPECTOR (Prawy panel - też row-span-2) */}
			<aside className="row-span-2 col-start-3 col-end-4 border-t border-default-200 mt-[-1px]  ">
				<InspectorPanel />
			</aside>
		</div>
	);
};

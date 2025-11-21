import { Outlet } from "react-router-dom";
import { Sidebar } from "./Sidebar";

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
			<header className="col-start-2 col-end-3 h-full border-b border-default-200 bg-background/50 p-4 border-dashed border-2 border-blue-500/50 flex items-center justify-center text-blue-500">
				PLACEHOLDER: TopBar (64px)
			</header>

			{/* OBSZAR 3: MAIN CONTENT (Środek - tutaj ląduje Outlet) */}
			<main className="relative col-start-2 col-end-3 overflow-hidden bg-background">
				{/* ScrollArea: To jest jedyne miejsce, które ma się scrollować! */}
				<div className="h-full w-full overflow-y-auto p-6">
					<Outlet />
				</div>
			</main>

			{/* OBSZAR 4: INSPECTOR (Prawy panel - też row-span-2) */}
			<aside className="row-span-2 col-start-3 col-end-4 h-full border-l border-default-200 bg-content1/50 p-4 border-dashed border-2 border-green-500/50 flex items-center justify-center text-green-500">
				PLACEHOLDER: Inspector (320px)
			</aside>
		</div>
	);
};

import { Card, CardFooter } from "@heroui/card";
import { Image } from "@heroui/image";
import { useShallow } from "zustand/react/shallow";
import { useGalleryStore } from "../stores/useGalleryStore";

export const GalleryGrid = () => {
	const zoomLevel = useGalleryStore((state) => state.zoomLevel);
	// const selectedAssetId = useGalleryStore((state) => state.selectedAssetId);
	const setSelectedAssetId = useGalleryStore(
		(state) => state.setSelectedAssetId,
	);
	// Fejkowe dane
	const items = Array.from({ length: 50 }, (_, i) => ({
		id: i,
		title: `Asset_Final_v${i}.blend`,
		type: i % 3 === 0 ? "MODEL" : "IMG",
	}));

	return (
		<div className="h-full w-full">
			{/* MAGIA CSS GRID 🪄
          1. Definiujemy zmienną CSS inline: --col-width
          2. W Tailwindzie używamy tej zmiennej wewnątrz arbitrary value: []
          
          repeat(auto-fill, ...) -> Wypchaj wiersz tyloma kolumnami, ile wejdzie.
          minmax(var(...), 1fr) -> Każda kolumna ma MINIMUM naszą szerokość, 
                                   ale jak jest luz, to rozciągnij się (1fr) równo.
      */}
			<div
				style={
					{
						"--col-width": `${zoomLevel}px`,
					} as React.CSSProperties
				}
				className="grid grid-cols-[repeat(auto-fill,minmax(var(--col-width),1fr))] gap-4 pb-20"
			>
				{items.map((item) => (
					<Card
						key={item.id}
						shadow="sm"
						isPressable
						onPress={() => setSelectedAssetId(item.id)}
						className="aspect-square border-none bg-content2 hover:scale-[1.02] transition-transform"
					>
						{/* Placeholder obrazka */}
						<div className="h-full w-full flex items-center justify-center bg-gradient-to-br from-default-100 to-default-200 text-default-500">
							<span className="text-4xl font-bold opacity-20">{item.type}</span>
						</div>

						{/* Stopka pojawia się po najechaniu (group-hover to ficzer HeroUI/Tailwind, ale tu upraszczamy) */}
						<CardFooter className="absolute bottom-0 z-10 justify-between border-t-1 border-zinc-100/50 bg-black/40 text-tiny text-white shadow-small opacity-0 hover:opacity-100 transition-opacity">
							<p className="truncate">{item.title}</p>
						</CardFooter>
					</Card>
				))}
			</div>
		</div>
	);
};

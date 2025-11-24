import { Button, ButtonGroup } from "@heroui/button";
import {
	Dropdown,
	DropdownItem,
	DropdownMenu,
	DropdownTrigger,
} from "@heroui/dropdown";
import { Input } from "@heroui/input";
import { Slider } from "@heroui/slider";
import {
	Grid3X3,
	LayoutList,
	Rows,
	Search,
	ChevronDown,
	ArrowUpDown,
	Minus,
	Plus,
} from "lucide-react";
import { useGalleryStore } from "../stores/useGalleryStore";
import { useShallow } from "zustand/react/shallow";
import { UI_CONFIG } from "@/config/constants";

export const TopToolbar = () => {
	const {
		zoomLevel,
		setZoomLevel,
		viewMode,
		setViewMode,
		sortOption,
		setSortOption,
	} = useGalleryStore(
		useShallow((state) => ({
			zoomLevel: state.zoomLevel,
			setZoomLevel: state.setZoomLevel,
			viewMode: state.viewMode,
			setViewMode: state.setViewMode,
			sortOption: state.sortOption,
			setSortOption: state.setSortOption,
		})),
	);

	return (
		<div className="sticky top-0 z-50 flex h-16 w-full items-center justify-between border-b border-default-200 bg-background/80 px-6 backdrop-blur-md">
			{/* SEKCJA A: KONTEKST */}
			<div className="flex items-center gap-4">
				<h1 className="text-lg font-bold tracking-tight text-foreground">
					All Assets
				</h1>
				<span className="rounded-full bg-default-100 px-2.5 py-0.5 text-xs font-medium text-default-500">
					1,240 items
				</span>
			</div>

			{/* SEKCJA B: WYSZUKIWANIE */}
			<div className="flex-1 max-w-xl px-6">
				<Input
					classNames={{
						base: "max-w-full h-10",
						mainWrapper: "h-full",
						input: "text-small",
						inputWrapper:
							"h-full font-normal text-default-500 bg-default-400/20 dark:bg-default-500/20",
					}}
					placeholder="Search assets..."
					size="sm"
					startContent={<Search size={18} />}
					type="search"
				/>
			</div>

			{/* SEKCJA C: KONTROLA */}
			<div className="flex items-center gap-4">
				{/* ZOOM SLIDER */}
				<div className="w-64 flex items-center gap-2">
					<Slider
						size="sm"
						step={10} //co 10px
						color="foreground"
						maxValue={UI_CONFIG.GALLERY.MAX_ZOOM} // Max wielkość kafelka
						minValue={UI_CONFIG.GALLERY.MIN_ZOOM} // Min wielkość kafelka
						aria-label="Thumbnail Size"
						value={zoomLevel}
						onChange={(v) => {
							const val = Array.isArray(v) ? v[0] : v;
							setZoomLevel(val);
						}}
						startContent={
							<button
								type="button"
								className="text-default-400 hover:text-primary hover:cursor-pointer transition-colors outline-none focus-visible:ring-2 focus-visible:ring-primary rounded-full p-1"
								onClick={() => setZoomLevel(Math.max(100, zoomLevel - 10))}
								aria-label="Zoom Out"
							>
								<Minus size={16} />
							</button>
						}
						// PRAWY PRZYCISK (PLUS) ➕
						endContent={
							<button
								type="button"
								className="text-default-400 hover:text-primary hover:cursor-pointer transition-colors outline-none focus-visible:ring-2 focus-visible:ring-primary rounded-full p-1"
								onClick={() => setZoomLevel(Math.min(350, zoomLevel + 10))}
								aria-label="Zoom In"
							>
								<Plus size={16} />
							</button>
						}
					/>
				</div>

				<div className="h-6 w-px bg-default-300" />

				{/* SORT DROPDOWN */}
				<Dropdown>
					<DropdownTrigger>
						<Button
							variant="flat"
							size="sm"
							startContent={<ArrowUpDown size={16} />}
							endContent={<ChevronDown size={16} />}
							className="text-default-600 capitalize"
						>
							{/* Wyświetlamy aktualnie wybrany sort (np. "newest") */}
							{sortOption.replace("_", " ")}
						</Button>
					</DropdownTrigger>
					<DropdownMenu
						aria-label="Sort options"
						disallowEmptySelection
						selectionMode="single"
						selectedKeys={new Set([sortOption])}
						onSelectionChange={(keys) => {
							const selected = Array.from(keys)[0] as string;
							setSortOption(selected);
						}}
					>
						<DropdownItem key="newest">Date Added (Newest)</DropdownItem>
						<DropdownItem key="oldest">Date Added (Oldest)</DropdownItem>
						<DropdownItem key="name">Name (A-Z)</DropdownItem>
						<DropdownItem key="size">File Size</DropdownItem>
					</DropdownMenu>
				</Dropdown>

				{/* VIEW TOGGLE */}
				<ButtonGroup variant="flat" size="sm">
					<Button
						isIconOnly
						// Aktywny przycisk dostaje inny styl
						className={
							viewMode === "grid" ? "bg-default-300 text-foreground" : ""
						}
						onPress={() => setViewMode("grid")}
					>
						<Grid3X3 size={18} />
					</Button>
					<Button
						isIconOnly
						className={
							viewMode === "masonry" ? "bg-default-300 text-foreground" : ""
						}
						onPress={() => setViewMode("masonry")}
					>
						<LayoutList size={18} />
					</Button>
					<Button
						isIconOnly
						className={
							viewMode === "list" ? "bg-default-300 text-foreground" : ""
						}
						onPress={() => setViewMode("list")}
					>
						<Rows size={18} />
					</Button>
				</ButtonGroup>
			</div>
		</div>
	);
};

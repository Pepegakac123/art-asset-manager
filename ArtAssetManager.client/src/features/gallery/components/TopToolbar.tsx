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
} from "lucide-react";

export const TopToolbar = () => {
	return (
		<div className="sticky top-0 z-50 flex h-16 w-full items-center justify-between border-b border-default-200 bg-background/80 px-6 backdrop-blur-md">
			{/* SEKCJA A: KONTEKST (LEWA) */}
			<div className="flex items-center gap-4">
				<h1 className="text-lg font-bold tracking-tight text-foreground">
					All Assets
				</h1>
				<span className="rounded-full bg-default-100 px-2.5 py-0.5 text-xs font-medium text-default-500">
					1,240 items
				</span>
			</div>

			{/* SEKCJA B: WYSZUKIWANIE (ŚRODEK) */}
			<div className="flex-1 max-w-xl px-6">
				<Input
					classNames={{
						base: "max-w-full h-10",
						mainWrapper: "h-full",
						input: "text-small",
						inputWrapper:
							"h-full font-normal text-default-500 bg-default-400/20 dark:bg-default-500/20",
					}}
					placeholder="Search assets by name, tag or type..."
					size="sm"
					startContent={<Search size={18} />}
					type="search"
				/>
			</div>

			{/* SEKCJA C: KONTROLA (PRAWA) */}
			<div className="flex items-center gap-4">
				{/* ZOOM SLIDER */}
				<div className="w-32 flex items-center gap-2">
					<Slider
						size="sm"
						step={1}
						color="foreground"
						maxValue={300}
						minValue={100}
						defaultValue={150}
						aria-label="Thumbnail Size"
						className="max-w-md"
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
							className="text-default-600"
						>
							Newest
						</Button>
					</DropdownTrigger>
					<DropdownMenu aria-label="Sort options">
						<DropdownItem key="newest">Date Added (Newest)</DropdownItem>
						<DropdownItem key="oldest">Date Added (Oldest)</DropdownItem>
						<DropdownItem key="name">Name (A-Z)</DropdownItem>
						<DropdownItem key="size">File Size</DropdownItem>
					</DropdownMenu>
				</Dropdown>

				{/* VIEW TOGGLE */}
				<ButtonGroup variant="flat" size="sm">
					<Button isIconOnly aria-label="Grid View">
						<Grid3X3 size={18} />
					</Button>
					<Button isIconOnly aria-label="Masonry View">
						<LayoutList size={18} />
					</Button>
					<Button isIconOnly aria-label="List View">
						<Rows size={18} />
					</Button>
				</ButtonGroup>
			</div>
		</div>
	);
};

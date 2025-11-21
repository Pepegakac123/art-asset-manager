import { Autocomplete, AutocompleteItem } from "@heroui/autocomplete";
import { Chip } from "@heroui/chip";
import { Search, Hash } from "lucide-react";

// --- FAKE DATA GENERATOR (Symulacja dużej bazy) ---
// Tworzymy 1000 tagów, żeby przetestować wydajność wirtualizacji
const generateTags = () => {
	const baseTags = [
		{ label: "Cyberpunk", value: "cyberpunk" },
		{ label: "Nature", value: "nature" },
		{ label: "Character", value: "character" },
		{ label: "Weapon", value: "weapon" },
		{ label: "Vehicle", value: "vehicle" },
		{ label: "Texture", value: "texture" },
		{ label: "Architecture", value: "architecture" },
		{ label: "Sci-Fi", value: "scifi" },
	];

	const extraTags = Array.from({ length: 1000 }, (_, i) => ({
		label: `Tag_Generowany_${i}`,
		value: `tag_${i}`,
	}));

	return [...baseTags, ...extraTags];
};

const availableTags = generateTags();
const popularTags = ["Cyberpunk", "Nature", "Weapon", "Texture", "Sci-Fi"];

export const TagFilter = () => {
	return (
		<div className="px-2 mb-4">
			{/* HeroUI AUTOCOMPLETE */}
			<Autocomplete
				aria-label="Filter tags"
				placeholder="Filter tags..."
				size="sm"
				variant="flat"
				radius="md"
				startContent={<Search size={16} className="text-default-400" />}
				defaultItems={availableTags}
				// 👇 WIRTUALIZACJA (Turbo Mode) 🚀
				isVirtualized={true}
				maxListboxHeight={250} // Max wysokość listy w pikselach
				itemHeight={32} // Wysokość jednego wiersza (ważne dla obliczeń!)
				// 👇 Style (Ciemny motyw + poprawki)
				classNames={{
					popoverContent: "bg-content1 border border-default-200",
				}}
				inputProps={{
					classNames: {
						input: "text-sm",
						inputWrapper:
							"h-9 min-h-0 bg-default-100 group-data-[focus=true]:bg-default-200",
					},
				}}
				listboxProps={{
					hideSelectedIcon: true,
					itemClasses: {
						base: "text-default-400 data-[hover=true]:text-foreground data-[hover=true]:bg-default-100",
						title: "text-sm",
					},
				}}
			>
				{(item) => (
					<AutocompleteItem key={item.value} textValue={item.label}>
						<div className="flex items-center gap-2">
							<Hash size={14} className="text-default-400" />
							<span>{item.label}</span>
						</div>
					</AutocompleteItem>
				)}
			</Autocomplete>

			{/* CHMURA TAGÓW (Chips) */}
			<div className="flex flex-wrap gap-2 mt-3">
				{popularTags.map((tag) => (
					<Chip
						key={tag}
						size="md"
						variant="flat"
						classNames={{
							base: "h-7 bg-default-100 hover:bg-primary/20 hover:text-primary cursor-pointer transition-colors border border-transparent hover:border-primary/30",
							content:
								"text-xs font-medium text-default-500 hover:text-primary px-2",
						}}
						startContent={<Hash size={12} className="ml-1 opacity-50" />}
					>
						{tag}
					</Chip>
				))}
			</div>
		</div>
	);
};

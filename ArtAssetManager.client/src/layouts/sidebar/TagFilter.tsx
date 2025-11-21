import { Autocomplete, AutocompleteItem } from "@heroui/autocomplete";
import { Chip } from "@heroui/chip";
import { Switch } from "@heroui/switch";
import { Tooltip } from "@heroui/tooltip";
import { Search, Hash, Info, X } from "lucide-react";
import { useGalleryStore } from "../../features/gallery/stores/useGalleryStore";
import { useMemo } from "react";

// Fake data
const availableTags = [
	{ label: "Cyberpunk", value: "Cyberpunk" },
	{ label: "Nature", value: "Nature" },
	{ label: "Character", value: "Character" },
	{ label: "Weapon", value: "Weapon" },
	{ label: "Vehicle", value: "Vehicle" },
	{ label: "Texture", value: "Texture" },
	{ label: "Architecture", value: "Architecture" },
	{ label: "Sci-Fi", value: "Sci-Fi" },
];

/*
TODO: [API] Fetch Real Tags
- Zastąpić `generateTags()` hookiem `useQuery` pobierającym dane z GET /api/tags.
- Backend powinien zwracać tagi posortowane wg popularności (count), żeby "Popular Tags" miały sens.

TODO: [UX] Debounce Search
- Jeśli tagów będzie > 5000, warto dodać debounce (opóźnienie) do inputa wyszukiwania, choć wirtualizacja na razie radzi sobie świetnie.
*/

const popularTags = ["Cyberpunk", "Nature", "Weapon", "Texture", "Sci-Fi"];

export const TagFilter = () => {
	const selectedTags = useGalleryStore((state) => state.selectedTags);
	const toggleTag = useGalleryStore((state) => state.toggleTag);
	const removeTag = useGalleryStore((state) => state.removeTag);
	const isStrictMode = useGalleryStore((state) => state.isStrictMode);
	const setStrictMode = useGalleryStore((state) => state.setStrictMode);

	const filteredTags = useMemo(() => {
		return availableTags.filter((tag) => !selectedTags.includes(tag.value));
	}, [selectedTags]);

	return (
		<div className="px-2 mb-6">
			{/* 1. HEADER: STRICT MODE SWITCH */}
			<div className="flex items-center justify-between mb-3 px-1">
				<div className="flex items-center gap-2">
					<span className="text-[10px] font-semibold uppercase tracking-[0.15em] text-default-400/80">
						Filter Logic
					</span>
					<Tooltip
						content="If enabled, shows only assets containing ALL selected tags (AND logic). Otherwise shows assets with ANY of the tags (OR logic)."
						className="max-w-xs text-tiny"
					>
						<Info
							size={14}
							className="text-default-400/70 cursor-help hover:text-foreground transition-colors"
						/>
					</Tooltip>
				</div>
				<Switch
					size="sm"
					isSelected={isStrictMode}
					onValueChange={setStrictMode}
					aria-label="Strict Mode"
					color="primary"
				>
					<span className="text-[10px] text-default-500 font-medium">
						{isStrictMode ? "STRICT" : "LOOSE"}
					</span>
				</Switch>
			</div>

			{/* 2. SEARCH INPUT */}
			<Autocomplete
				key={selectedTags.join(",")}
				aria-label="Filter tags"
				placeholder="Add tag filter..."
				size="sm"
				variant="flat"
				radius="md"
				startContent={<Search size={16} className="text-default-400" />}
				defaultItems={filteredTags}
				selectedKey={null}
				isVirtualized={true}
				maxListboxHeight={250}
				itemHeight={32}
				onSelectionChange={(key) => {
					if (key) {
						toggleTag(key.toString());
					}
				}}
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

			{/* 3. ACTIVE FILTERS */}
			{selectedTags.length > 0 && (
				<div className="flex flex-wrap gap-2 mt-3 mb-2 animate-appearance-in">
					{selectedTags.map((tag) => (
						<Chip
							key={tag}
							size="md"
							variant="solid"
							color="primary"
							onClose={() => removeTag(tag)}
							classNames={{
								base: "h-7",
								content: "text-xs font-bold px-2 text-white",
								closeButton: "text-white/70 hover:text-white",
							}}
						>
							{tag}
						</Chip>
					))}
				</div>
			)}

			{/* 4. POPULAR TAGS */}
			<div className="mt-5">
				{" "}
				{/* Zwiększony odstęp z góry */}
				{/* 👇 ZMIANA: Ten sam styl techniczny co wyżej */}
				<p className="text-[10px] font-semibold uppercase tracking-[0.15em] text-default-400/80 mb-2.5 px-1">
					Popular Tags
				</p>
				<div className="flex flex-wrap gap-2">
					{popularTags.map((tag) => {
						const isActive = selectedTags.includes(tag);
						return (
							<Chip
								key={tag}
								size="md"
								variant="flat"
								className={`cursor-pointer transition-all border ${
									isActive
										? "bg-primary/10 border-primary/50 text-primary"
										: "bg-default-100 border-transparent hover:border-primary/30 hover:text-foreground text-default-500"
								}`}
								classNames={{
									base: "h-7",
									content: "text-xs font-medium px-2",
								}}
								startContent={
									<Hash
										size={12}
										className={isActive ? "text-primary" : "opacity-50"}
									/>
								}
								onClick={() => toggleTag(tag)}
							>
								{tag}
							</Chip>
						);
					})}
				</div>
			</div>
		</div>
	);
};

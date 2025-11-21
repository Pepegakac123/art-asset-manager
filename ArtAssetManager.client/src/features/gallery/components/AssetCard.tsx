import { Card, CardBody } from "@heroui/card";
import { Image } from "@heroui/image";
import { Checkbox } from "@heroui/checkbox";
import { Button } from "@heroui/button";
import {
	Dropdown,
	DropdownTrigger,
	DropdownMenu,
	DropdownItem,
} from "@heroui/dropdown";
import {
	Heart,
	MoreVertical,
	FileBox,
	Edit,
	FolderPlus,
	Trash,
	Box,
} from "lucide-react";
import { useGalleryStore } from "../stores/useGalleryStore";
import { useState } from "react";
import { Tooltip } from "@heroui/tooltip";

/*
TODO: [STATE] Global Selection Sync
- Usunąć lokalny stan `const [isSelected, setIsSelected] = useState(false)`.
- Podpiąć checkbox pod globalny store: `isSelected = useGalleryStore(s => s.selectedAssetIds.includes(id))`.
- To krytyczne, aby Multi-Select działał z Shift+Click i przyciskami "Select All".

TODO: [ACTIONS] Connect Context Menu to API
- Menu "Rename": Otworzyć modal/input i strzelić do PATCH /api/assets/{id}/rename.
- Menu "Delete": Strzelić do DELETE /api/assets/{id} (Soft Delete).
- Menu "Add to Collection": Otworzyć modal wyboru kolekcji.

TODO: [UX] Open in Explorer
- Plan (Krok 25): Dodać opcję "Show in Explorer" w menu kontekstowym (wymaga endpointu backendowego).

TODO: [PERFORMANCE] Image Optimization
- ThumbnailUrl: Upewnić się, że backend zwraca małą miniaturkę (np. 400px), a nie pełny plik 4K.
*/

export interface AssetCardProps {
	id: number;
	title: string;
	type: string;
	thumbnailUrl: string;
	isFavorite?: boolean;
}

export const AssetCard = ({
	id,
	title,
	type,
	thumbnailUrl,
	isFavorite = false,
}: AssetCardProps) => {
	const selectedAssetId = useGalleryStore((state) => state.selectedAssetId);
	const setSelectedAssetId = useGalleryStore(
		(state) => state.setSelectedAssetId,
	);
	const isActive = selectedAssetId === id;

	// TODO: W przyszłości podpiąć isSelected pod globalny store multi-selectu.
	const [isSelected, setIsSelected] = useState(false);
	const [isHovered, setIsHovered] = useState(false);

	const showControls = isHovered || isSelected;

	const actionBubbleClass = `
    flex items-center justify-center min-w-8 w-8 h-8 rounded-full 
    bg-content1/90 backdrop-blur-md 
    border transition-all duration-200
    ${isActive || isSelected ? "border-primary shadow-[0_0_10px_rgba(249,115,22,0.2)]" : "border-white/10 hover:border-primary hover:text-primary"}
  `;

	return (
		<Card
			shadow="sm"
			isPressable
			onPress={() => setSelectedAssetId(id)}
			onMouseEnter={() => setIsHovered(true)}
			onMouseLeave={() => setIsHovered(false)}
			className={`
        aspect-square transition-all duration-200 bg-content1 group
        ${isActive || isSelected ? "ring-2 ring-primary ring-offset-2 ring-offset-background z-10" : "border-transparent hover:border-default-300"}
      `}
			data-selected={isSelected}
		>
			<CardBody className="p-0 overflow-hidden relative h-full w-full">
				{/* 1. OBRAZEK */}
				<Image
					removeWrapper
					radius="none"
					alt={title}
					className={`z-0 w-full h-full object-cover transition-transform duration-500 ${isHovered ? "scale-110" : "scale-100"}`}
					src={thumbnailUrl}
				/>

				{/* 2. OVERLAY */}
				<div
					className={`absolute inset-0 bg-black/60 transition-opacity duration-300 z-10 pointer-events-none
            ${showControls ? "opacity-100" : "opacity-0"}`}
				/>

				{/* 3. GÓRA-PRAWA: AKCJE (Equal Size Bubbles) */}
				<div
					className={`absolute top-2 right-2 z-20 flex flex-row items-center gap-2 transition-opacity duration-200 ${showControls ? "opacity-100" : "opacity-0"}`}
				>
					{/* A. CHECKBOX WRAPPER (Udaje przycisk) */}
					<Tooltip content="Select Item" delay={500} closeDelay={0}>
						<div className={actionBubbleClass}>
							<Checkbox
								isSelected={isSelected}
								onValueChange={setIsSelected}
								radius="full"
								color="primary"
								size="md" // Większy rozmiar checkboxa
								aria-label="Select asset"
								classNames={{
									wrapper: "m-0 p-0 mr-[-1px]", // Reset marginów HeroUI
									// Ukrywamy domyślne tło checkboxa, bo wrapper robi robotę,
									// chyba że jest zaznaczony - wtedy chcemy pomarańczowe wypełnienie
									base: "p-0 m-0 gap-0",
								}}
							/>
						</div>
					</Tooltip>

					{/* B. SERCE (Favorite) */}
					<Tooltip
						content={isFavorite ? "Remove from Favorites" : "Add to Favorites"}
						delay={500}
						closeDelay={0}
					>
						<Button
							isIconOnly
							size="sm"
							radius="full"
							variant="light" // Używamy light, bo wrapper (actionBubbleClass) daje tło
							className={`${actionBubbleClass} ${isFavorite ? "text-danger border-danger hover:border-danger" : "text-default-400"}`}
						>
							<Heart size={18} fill={isFavorite ? "currentColor" : "none"} />
						</Button>
					</Tooltip>
				</div>

				{/* 4. DÓŁ-LEWA: TYP PLIKU (Badge) */}
				<div
					className={`absolute bottom-2 left-2 z-20 transition-opacity duration-200 ${showControls ? "opacity-100" : "opacity-0"}`}
				>
					<div className="px-2 py-1 rounded-md bg-black/80 backdrop-blur-md border border-white/10 flex items-center gap-1.5 shadow-lg">
						{type === "BLEND" && <Box size={14} className="text-orange-500" />}
						{type === "FBX" && <Box size={14} className="text-blue-500" />}
						{type === "PNG" && <FileBox size={14} className="text-green-500" />}
						<span className="text-[10px] font-bold text-white uppercase tracking-wider">
							{type}
						</span>
					</div>
				</div>

				{/* 5. DÓŁ-PRAWA: MENU KONTEKSTOWE */}
				<div
					className={`absolute bottom-2  right-2 z-20 transition-opacity duration-200 ${showControls ? "opacity-100" : "opacity-0"}`}
				>
					<Dropdown placement="bottom-end">
						<DropdownTrigger>
							{/* Używamy tej samej klasy 'actionBubbleClass' dla spójności */}
							<Button
								isIconOnly
								size="sm"
								radius="full"
								variant="light"
								className={actionBubbleClass}
							>
								<MoreVertical
									size={18}
									className="text-default-400 group-hover:text-primary transition-colors"
								/>
							</Button>
						</DropdownTrigger>

						<DropdownMenu
							aria-label="Asset Actions"
							classNames={{
								base: "bg-content1  shadow-xl",
							}}
							itemClasses={{
								base: "data-[hover=true]:bg-primary/10 data-[hover=true]:text-primary text-default-500 transition-colors",
								title: "text-sm font-medium",
							}}
						>
							<DropdownItem key="rename" startContent={<Edit size={16} />}>
								Rename
							</DropdownItem>
							<DropdownItem
								key="add-set"
								startContent={<FolderPlus size={16} />}
							>
								Add to Collection
							</DropdownItem>
							<DropdownItem
								key="delete"
								className="text-danger group/del"
								color="danger"
								startContent={
									<Trash size={16} className="group-hover/del:text-danger" />
								}
							>
								Delete
							</DropdownItem>
						</DropdownMenu>
					</Dropdown>
				</div>
			</CardBody>
		</Card>
	);
};

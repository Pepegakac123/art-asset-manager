import { Card, CardFooter, CardHeader } from "@heroui/card";
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
  Edit,
  FolderPlus,
  Trash,
  Box,
  Image as ImageIcon,
  FileBox,
  FolderOpen,
  Maximize2,
  HardDrive,
} from "lucide-react";
import { useState } from "react";
import { Asset } from "@/types/api";
import { useAssets } from "../hooks/useAssets";
import { AxiosResponse } from "axios";
import { UseMutateFunction } from "@tanstack/react-query";

interface AssetCardProps {
  asset: Asset;
  explorerfn: UseMutateFunction<
    AxiosResponse<any, any, {}>,
    any,
    string,
    unknown
  >;
  style?: React.CSSProperties;
}

export const AssetCard = ({ asset, explorerfn, style }: AssetCardProps) => {
  const {
    id,
    fileName,
    fileType,
    filePath,
    thumbnailUrl,
    imageWidth,
    imageHeight,
    fileExtension,
  } = asset;

  const [isSelected, setIsSelected] = useState(false);
  const [isHovered, setIsHovered] = useState(false);
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  // Bezpieczne łączenie URL (naprawa podwójnych slashy)
  const imageUrl = thumbnailUrl
    ? `${import.meta.env.VITE_API_URL}${thumbnailUrl.startsWith("/") ? "" : "/"}${thumbnailUrl}`
    : "/placeholder.png";

  const showControls = isHovered || isSelected || isMenuOpen;

  // Helper do ikonki
  const getFileIcon = () => {
    switch (fileType?.toLowerCase()) {
      case "model":
        return <Box size={14} className="text-white/80" />;
      case "image":
      case "texture":
        return <ImageIcon size={14} className="text-white/80" />;
      default:
        return <FileBox size={14} className="text-white/80" />;
    }
  };
  const formatBytes = (bytes: number, decimals = 0) => {
    if (!+bytes) return "0 B";
    const k = 1024;
    const dm = decimals < 0 ? 0 : decimals;
    const sizes = ["B", "KB", "MB", "GB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`;
  };
  // Funkcja otwierająca (zamiast isPressable)
  const handleCardClick = () => {
    console.log("Open details for asset:", id);
  };

  // Wrapper zapobiegający odpaleniu handleCardClick przy kliknięciu w akcje
  const stopProp = (e: React.SyntheticEvent) => {
    e.stopPropagation();
  };

  return (
    <Card
      // ❌ USUNIĘTE: isPressable (to powodowało błąd zagnieżdżonych buttonów)
      shadow="sm"
      radius="lg"
      className={`group relative h-full w-full border-none bg-black/20 transition-transform hover:scale-[1.02] ${
        isSelected ? "ring-2 ring-primary" : ""
      }`}
      style={style}
      // ✅ DODANE: Obsługa kliknięcia "ręcznie"
      onClick={handleCardClick}
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
    >
      {/* --- HEADER: Akcje (Checkbox, Heart, Menu) --- */}
      {/* Absolute positioning na górze */}
      <CardHeader className="absolute top-0 z-20 flex w-full justify-between p-2">
        <div
          className={`flex gap-2 transition-opacity duration-200 ${
            showControls ? "opacity-100" : "opacity-0"
          }`}
          onClick={stopProp} // Ważne: Kliknięcie w to nie otwiera assetu
          onKeyDown={stopProp}
        >
          <Checkbox
            isSelected={isSelected}
            onValueChange={setIsSelected}
            size="sm"
            classNames={{
              wrapper:
                "bg-black/40 border-white/50 group-data-[selected=true]:bg-primary",
            }}
          />
        </div>

        <div
          className={`flex gap-1 transition-opacity duration-200 ${
            showControls ? "opacity-100" : "opacity-0"
          }`}
          onClick={stopProp}
          onKeyDown={stopProp}
        >
          <Button
            isIconOnly
            size="sm"
            radius="full"
            variant="light"
            className="bg-black/40 text-white backdrop-blur-md hover:bg-primary/80 hover:text-white"
            onPress={() => explorerfn(filePath)}
            title="Open in Explorer"
          >
            <FolderOpen size={16} />
          </Button>
          {/* Przycisk Ulubione */}
          <Button
            isIconOnly
            size="sm"
            radius="full"
            variant="light"
            className="bg-black/40 text-white backdrop-blur-md hover:bg-black/60"
            onPress={() => console.log("Toggle favorite", id)}
          >
            <Heart
              size={16}
              className={
                asset.isFavorite ? "fill-danger text-danger" : "text-white"
              }
            />
          </Button>

          {/* Menu Dropdown */}
          <Dropdown placement="bottom-end" onOpenChange={setIsMenuOpen}>
            <DropdownTrigger>
              <Button
                isIconOnly
                size="sm"
                radius="full"
                variant="light"
                className="bg-black/40 text-white backdrop-blur-md hover:bg-black/60"
              >
                <MoreVertical size={16} />
              </Button>
            </DropdownTrigger>
            <DropdownMenu
              aria-label="Asset Actions"
              onAction={(key) => console.log("Action:", key)}
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
                className="text-danger"
                color="danger"
                startContent={<Trash size={16} />}
              >
                Delete
              </DropdownItem>
            </DropdownMenu>
          </Dropdown>
        </div>
      </CardHeader>

      {/* --- BODY: Obrazek tła --- */}
      {/* removeWrapper + object-cover sprawia że obrazek działa jak background-image */}
      <Image
        removeWrapper
        alt={fileName}
        className="z-0 h-full w-full object-cover"
        src={imageUrl}
        fallbackSrc="https://via.placeholder.com/400x400?text=No+Preview"
      />

      {/* --- FOOTER: Rozmyty pasek z nazwą --- */}
      <CardFooter className="absolute bottom-0 z-10 w-full justify-between border-t border-white/10 bg-black/60 p-2 backdrop-blur-md">
        <div className="flex w-full flex-col items-start gap-1">
          {/* Nazwa pliku */}
          <p className="w-full truncate text-tiny font-bold text-white/90 text-left">
            {fileName}
          </p>

          {/* Metadane*/}
          <div className="flex w-full items-center justify-between mt-1">
            {/* LEWA: Typ Pliku */}
            <span
              className="flex items-center gap-1 text-[10px] text-white/60"
              title="File Type"
            >
              {getFileIcon()}
              <span className="uppercase tracking-wider font-medium">
                {fileExtension?.replace(".", "")}
              </span>
            </span>

            {/* PRAWA: Wymiary i Rozmiar */}
            <div className="flex items-center gap-2 text-[9px] text-white/50">
              {/* Wymiary (jeśli są) */}
              {(imageWidth ?? 0) > 0 && (imageHeight ?? 0) > 0 && (
                <span className="flex items-center gap-1" title="Resolution">
                  <Maximize2 size={10} className="text-white/40" />
                  {imageWidth}×{imageHeight}
                </span>
              )}

              {/* Rozmiar Pliku */}
              <span className="flex items-center gap-1" title="File Size">
                <HardDrive size={10} className="text-white/40" />
                {formatBytes(asset.fileSize)}
              </span>
            </div>
          </div>
        </div>
      </CardFooter>
    </Card>
  );
};

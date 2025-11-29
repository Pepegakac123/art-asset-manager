import React, { useState } from "react";
import { Button } from "@heroui/button";
import { ScrollShadow } from "@heroui/scroll-shadow";
import { Tabs, Tab } from "@heroui/tabs";
import { Input, Textarea } from "@heroui/input";
import { Divider } from "@heroui/divider";
import { Tooltip } from "@heroui/tooltip";
import { Chip } from "@heroui/chip"; // Używamy Chipów HeroUI
import {
  Heart,
  FolderOpen,
  ExternalLink,
  Copy,
  Search,
  Edit3,
  Layers,
  Info,
  BoxSelect,
  PlusCircle,
  Trash2,
  FileText,
  Tag as TagIcon,
} from "lucide-react";

// --- MOCK DATA ---
const MOCK_ASSET = {
  id: 1,
  fileName: "Cyberpunk_Robot_v2.blend",
  filePath: "D:\\Assets\\Models\\SciFi\\Cyberpunk_Robot_v2.blend",
  fileType: "model",
  fileSize: 124500000,
  thumbnailUrl: "/thumbnails/placeholder.png",
  rating: 4,
  description: "High poly model of a cyberpunk robot. Needs textures.",
  isFavorite: true,
  imageWidth: null,
  imageHeight: null,
  dominantColor: "#ff0044",
  bitDepth: null,
  hasAlphaChannel: false,
  tags: [
    { id: 1, name: "scifi" },
    { id: 2, name: "robot" },
    { id: 3, name: "hard-surface" },
  ],
};

interface InspectorPanelProps {
  selectedAssetIds?: number[];
}

export const InspectorPanel = ({
  selectedAssetIds = [1],
}: InspectorPanelProps) => {
  const [activeTab, setActiveTab] = useState<string>("details");
  const isMultiSelect = selectedAssetIds.length > 1;

  // --- HELPER: SECTION HEADER (Styl z Sidebar.tsx) ---
  const SectionHeader = ({
    title,
    icon: Icon,
  }: {
    title: string;
    icon?: any;
  }) => (
    <div className="flex items-center gap-2 text-default-500 mb-2 px-1">
      {Icon && <Icon size={14} />}
      <span className="text-xs font-semibold uppercase tracking-wider">
        {title}
      </span>
    </div>
  );

  // --- RENDER: BATCH MODE ---
  if (isMultiSelect) {
    return (
      <div className="h-full w-full flex flex-col bg-content1">
        <div className="p-6 border-b border-default-100 flex flex-col items-center justify-center gap-3 bg-default-50/50">
          <div className="w-12 h-12 bg-primary/10 rounded-full flex items-center justify-center text-primary shadow-sm">
            <BoxSelect size={24} />
          </div>
          <div className="text-center">
            <h3 className="text-md font-bold text-default-900">
              {selectedAssetIds.length} items
            </h3>
            <p className="text-tiny text-default-500">Batch selection active</p>
          </div>
        </div>
        {/* Tu można dodać resztę batch actions */}
      </div>
    );
  }

  // --- RENDER: SINGLE ASSET MODE ---
  const asset = MOCK_ASSET;
  const isImage = asset.fileType === "image";

  return (
    <div className="h-full w-full flex flex-col bg-content1">
      {/* 1. STICKY HEADER */}
      {/* ZMIANA: Usunięto z-index problem, dodano lepszy styling inputa */}
      <div className="flex-none p-4 flex flex-col gap-3 border-b border-default-100 bg-content1">
        {/* Thumbnail */}
        <div className="relative group w-full aspect-video bg-default-100 rounded-lg overflow-hidden border border-default-200 shadow-sm">
          <img
            src={asset.thumbnailUrl}
            alt="Thumbnail"
            className="w-full h-full object-cover opacity-90 group-hover:opacity-100 transition-opacity"
          />
          <div className="absolute inset-0 bg-black/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center gap-2 backdrop-blur-[1px]">
            <Button
              isIconOnly
              size="sm"
              variant="flat"
              className="bg-white/20 text-white"
            >
              {isImage ? <Search size={16} /> : <Edit3 size={16} />}
            </Button>
          </div>
        </div>

        {/* Title & Path */}
        <div className="flex flex-col gap-1">
          <Input
            variant="underlined"
            value={asset.fileName}
            size="sm"
            classNames={{
              input: "font-bold text-small text-default-900",
              inputWrapper: "border-b-default-200 px-0 h-8",
            }}
          />

          <div
            className="flex items-center gap-1 group/path cursor-pointer relative"
            title={asset.filePath}
          >
            <span className="text-[10px] text-default-400 truncate flex-1 font-mono">
              {asset.fileName} {/* Tu docelowo relatywna ścieżka */}
            </span>
            <Tooltip content="Copy Path" closeDelay={0}>
              <Button
                size="sm"
                isIconOnly
                variant="light"
                className="h-5 w-5 min-w-0 text-default-400 opacity-0 group-hover/path:opacity-100"
              >
                <Copy size={10} />
              </Button>
            </Tooltip>
          </div>
        </div>
      </div>

      {/* 2. SCROLLABLE CONTENT */}
      {/* ZMIANA: flex-1 i min-h-0 są kluczowe dla scrolla w flex column */}
      <ScrollShadow className="flex-1 min-h-0 flex flex-col custom-scrollbar">
        {/* --- PROPERTIES SECTION --- */}
        <div className="p-4 flex flex-col gap-5">
          {/* Rating */}
          <div className="flex flex-col gap-1">
            <SectionHeader title="Rating" />
            <div className="flex text-warning px-1 cursor-pointer hover:opacity-80 transition-opacity">
              {/* Mock gwiazdek */}
              ★★★★<span className="text-default-300">★</span>
            </div>
          </div>

          {/* Description */}
          <div className="flex flex-col gap-1">
            <SectionHeader title="Description" icon={FileText} />
            <Textarea
              placeholder="Add a description..."
              minRows={2}
              maxRows={8}
              variant="faded"
              size="sm"
              defaultValue={asset.description}
              classNames={{
                input: "text-tiny",
                inputWrapper:
                  "bg-default-50 border-default-200 hover:border-default-300",
              }}
            />
          </div>

          {/* Tags */}
          <div className="flex flex-col gap-2">
            <SectionHeader title="Tags" icon={TagIcon} />
            <div className="flex flex-wrap gap-1.5 px-1">
              {asset.tags.map((tag) => (
                <Chip
                  key={tag.id}
                  size="sm"
                  variant="flat"
                  classNames={{
                    base: "bg-default-100 hover:bg-default-200 transition-colors cursor-pointer h-6",
                    content: "text-tiny text-default-600 font-medium",
                  }}
                >
                  #{tag.name}
                </Chip>
              ))}
              <button className="h-6 px-2 border border-dashed border-default-300 rounded-small text-[10px] text-default-400 hover:text-default-600 hover:border-default-400 transition-colors flex items-center">
                + Add
              </button>
            </div>
          </div>
        </div>

        <Divider className="opacity-50" />

        {/* --- TABS SECTION --- */}
        {/* ZMIANA: fullWidth=true i dopasowane style */}
        <div className="flex-1 flex flex-col">
          <Tabs
            fullWidth
            size="sm"
            variant="underlined"
            aria-label="Asset details"
            selectedKey={activeTab}
            onSelectionChange={(k) => setActiveTab(k as string)}
            classNames={{
              tabList: "p-0 border-b border-default-100 bg-content1 w-full",
              cursor: "w-full bg-primary h-[2px]",
              tab: "h-9 px-0",
              tabContent:
                "text-tiny font-medium group-data-[selected=true]:text-primary text-default-500",
            }}
          >
            {/* DETAILS TAB */}
            <Tab key="details" title="Details">
              <div className="p-4 flex flex-col gap-2">
                <DetailRow
                  label="Format"
                  value={asset.fileType.toUpperCase()}
                />
                <DetailRow
                  label="File Size"
                  value={`${(asset.fileSize / 1024 / 1024).toFixed(2)} MB`}
                />
                {asset.imageWidth && (
                  <DetailRow
                    label="Dimensions"
                    value={`${asset.imageWidth} x ${asset.imageHeight}`}
                  />
                )}
                {asset.bitDepth && (
                  <DetailRow
                    label="Bit Depth"
                    value={`${asset.bitDepth}-bit`}
                  />
                )}
                <DetailRow
                  label="Alpha"
                  value={asset.hasAlphaChannel ? "Yes" : "No"}
                />

                {/* Color Row */}
                <div className="flex justify-between items-center py-1.5 border-b border-default-100/50 last:border-0">
                  <span className="text-tiny text-default-500">
                    Dominant Color
                  </span>
                  <div className="flex items-center gap-2 group cursor-pointer">
                    <span className="text-tiny text-default-900 font-mono opacity-0 group-hover:opacity-100 transition-opacity">
                      {asset.dominantColor}
                    </span>
                    <div
                      className="w-3 h-3 rounded-full ring-1 ring-default-200 shadow-sm"
                      style={{ backgroundColor: asset.dominantColor }}
                    />
                  </div>
                </div>
              </div>
            </Tab>

            {/* VERSIONS TAB */}
            <Tab key="versions" title="Versions">
              <div className="p-6 text-center text-default-400 flex flex-col items-center gap-2">
                <Layers size={24} className="opacity-50" />
                <span className="text-tiny">No versions linked</span>
              </div>
            </Tab>

            {/* COLLECTIONS TAB */}
            <Tab key="collections" title="Collections">
              <div className="p-4">
                <div className="flex items-center gap-2 p-1.5 bg-default-50 rounded-md border border-default-100">
                  <FolderOpen size={14} className="text-primary" />
                  <span className="text-tiny text-default-700">
                    Sci-Fi Project A
                  </span>
                </div>
              </div>
            </Tab>
          </Tabs>
        </div>
      </ScrollShadow>

      {/* 4. STICKY FOOTER */}
      <div className="flex-none p-3 border-t border-default-200 bg-content1 flex gap-2 items-center">
        <Tooltip content="Open File">
          <Button
            isIconOnly
            variant="light"
            size="sm"
            className="text-default-500"
          >
            <ExternalLink size={18} />
          </Button>
        </Tooltip>
        <Tooltip content="Show in Explorer">
          <Button
            isIconOnly
            variant="light"
            size="sm"
            className="text-default-500"
          >
            <FolderOpen size={18} />
          </Button>
        </Tooltip>

        <div className="flex-1" />

        <Tooltip content="Add to Collection">
          <Button
            isIconOnly
            variant="light"
            size="sm"
            className="text-default-500"
          >
            <PlusCircle size={18} />
          </Button>
        </Tooltip>

        <Tooltip content={asset.isFavorite ? "Unfavorite" : "Favorite"}>
          <Button
            isIconOnly
            variant={asset.isFavorite ? "flat" : "light"}
            color={asset.isFavorite ? "danger" : "default"}
            size="sm"
            className={
              asset.isFavorite ? "text-danger bg-danger/10" : "text-default-500"
            }
          >
            <Heart
              size={18}
              fill={asset.isFavorite ? "currentColor" : "none"}
            />
          </Button>
        </Tooltip>
      </div>
    </div>
  );
};

// --- HELPER: Minimalist Detail Row ---
const DetailRow = ({
  label,
  value,
}: {
  label: string;
  value: string | number;
}) => (
  <div className="flex justify-between py-1.5 border-b border-default-100/50 last:border-0">
    <span className="text-tiny text-default-500">{label}</span>
    <span className="text-tiny text-default-900 font-medium">{value}</span>
  </div>
);

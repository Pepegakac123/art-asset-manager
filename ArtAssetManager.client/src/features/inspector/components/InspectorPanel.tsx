import React, { useState } from "react";
import { Button } from "@heroui/button";
import { ScrollShadow } from "@heroui/scroll-shadow";
import { Tabs, Tab } from "@heroui/tabs";
import { Textarea } from "@heroui/input";
import { Divider } from "@heroui/divider";
import { Tooltip } from "@heroui/tooltip";
import { Chip } from "@heroui/chip";
import { Spinner } from "@heroui/spinner";
import {
  Heart,
  ExternalLink,
  Layers,
  BoxSelect,
  FileText,
  Tag as TagIcon,
  FolderOpen,
  XCircle,
} from "lucide-react";
import { useGalleryStore } from "@/features/gallery/stores/useGalleryStore";
import { useAsset } from "../hooks/useAsset";
// IMPORT NOWEGO KOMPONENTU
import { InspectorHeader } from "./single/InspectorHeader";

// --- HELPER: SECTION HEADER ---
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

// --- HELPER: DETAIL ROW ---
const DetailRow = ({
  label,
  value,
}: {
  label: string;
  value: string | number;
}) => (
  <div className="flex justify-between py-1.5 border-b border-default-100/50 last:border-0">
    <span className="text-tiny text-default-500">{label}</span>
    <span className="text-tiny text-default-900 font-medium truncate max-w-[60%]">
      {value}
    </span>
  </div>
);

export const InspectorPanel = () => {
  const [activeTab, setActiveTab] = useState<string>("details");

  const selectedAssetIds = useGalleryStore((state) => state.selectedAssetIds);
  const selectionCount = selectedAssetIds.size;
  const isMultiSelect = selectionCount > 1;

  const activeAssetId =
    selectionCount === 1 ? selectedAssetIds.values().next().value : null;

  const { data: asset, isLoading, isError } = useAsset(activeAssetId);

  // --- BRAK SELEKCJI ---
  if (selectionCount === 0) {
    return (
      <div className="h-full w-full flex flex-col items-center justify-center text-default-300 gap-4 p-8 text-center">
        <div className="w-16 h-16 rounded-full bg-default-100 flex items-center justify-center">
          <XCircle size={32} className="opacity-50" />
        </div>
        <div>
          <h3 className="text-md font-medium text-default-500">No Selection</h3>
          <p className="text-tiny">Select an asset to view details</p>
        </div>
      </div>
    );
  }

  // --- MULTI SELECT ---
  if (isMultiSelect) {
    return (
      <div className="h-full w-full flex flex-col bg-content1">
        <div className="p-6 border-b border-default-100 flex flex-col items-center justify-center gap-3 bg-default-50/50">
          <div className="w-12 h-12 bg-primary/10 rounded-full flex items-center justify-center text-primary shadow-sm">
            <BoxSelect size={24} />
          </div>
          <div className="text-center">
            <h3 className="text-md font-bold text-default-900">
              {selectionCount} items
            </h3>
            <p className="text-tiny text-default-500">Batch selection active</p>
          </div>
        </div>
        <div className="p-8 text-center text-default-400 text-tiny">
          Batch editing coming soon...
        </div>
      </div>
    );
  }

  // --- LOADING ---
  if (isLoading) {
    return (
      <div className="h-full w-full flex items-center justify-center">
        <Spinner size="lg" color="primary" />
      </div>
    );
  }

  // --- ERROR ---
  if (isError || !asset) {
    return (
      <div className="h-full w-full flex items-center justify-center text-danger">
        <p>Failed to load asset details.</p>
      </div>
    );
  }

  // --- SINGLE ASSET RENDER ---
  return (
    <div className="h-full w-full flex flex-col bg-content1">
      {/* 1. NOWY HEADER KOMPONENT ✅ */}
      <InspectorHeader asset={asset} />

      {/* 2. SCROLLABLE CONTENT */}
      <ScrollShadow className="flex-1 min-h-0 flex flex-col custom-scrollbar">
        <div className="p-4 flex flex-col gap-5">
          {/* Rating */}
          <div className="flex flex-col gap-1">
            <SectionHeader title="Rating" />
            <div className="flex text-warning px-1 cursor-pointer hover:opacity-80 transition-opacity">
              {Array.from({ length: 5 }).map((_, i) => (
                <span
                  key={i}
                  className={
                    i < (asset.rating || 0)
                      ? "text-warning"
                      : "text-default-300"
                  }
                >
                  ★
                </span>
              ))}
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
              defaultValue={asset.description || ""}
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
              {asset.tags && asset.tags.length > 0 ? (
                asset.tags.map((tag: any) => (
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
                ))
              ) : (
                <span className="text-tiny text-default-400 italic">
                  No tags
                </span>
              )}
              <button className="h-6 px-2 border border-dashed border-default-300 rounded-small text-[10px] text-default-400 hover:text-default-600 hover:border-default-400 transition-colors flex items-center">
                + Add
              </button>
            </div>
          </div>
        </div>

        <Divider className="opacity-50" />

        {/* TABS */}
        <div className="flex-1 flex flex-col">
          <Tabs
            fullWidth
            size="sm"
            variant="underlined"
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
            <Tab key="details" title="Details">
              <div className="p-4 flex flex-col gap-2">
                <DetailRow
                  label="Format"
                  value={asset.fileExtension?.toUpperCase() || asset.fileType}
                />
                <DetailRow
                  label="File Size"
                  value={`${(asset.fileSize / 1024 / 1024).toFixed(2)} MB`}
                />
                {asset.imageWidth && asset.imageHeight && (
                  <DetailRow
                    label="Dimensions"
                    value={`${asset.imageWidth} x ${asset.imageHeight}`}
                  />
                )}
                <DetailRow
                  label="Created"
                  value={new Date(asset.dateAdded).toLocaleDateString()}
                />

                {asset.dominantColor && (
                  <div className="flex justify-between items-center py-1.5 border-b border-default-100/50 last:border-0">
                    <span className="text-tiny text-default-500">Color</span>
                    <div className="flex items-center gap-2">
                      <span className="text-tiny text-default-900 font-mono">
                        {asset.dominantColor}
                      </span>
                      <div
                        className="w-3 h-3 rounded-full ring-1 ring-default-200 shadow-sm"
                        style={{ backgroundColor: asset.dominantColor }}
                      />
                    </div>
                  </div>
                )}
              </div>
            </Tab>
            <Tab key="versions" title="Versions">
              <div className="p-6 text-center text-default-400">
                <Layers size={24} className="opacity-50 mx-auto mb-2" />
                <span className="text-tiny">No versions</span>
              </div>
            </Tab>
            <Tab key="collections" title="Collections">
              <div className="p-6 text-center text-default-400">
                <FolderOpen size={24} className="opacity-50 mx-auto mb-2" />
                <span className="text-tiny">No collections</span>
              </div>
            </Tab>
          </Tabs>
        </div>
      </ScrollShadow>

      {/* 4. STICKY FOOTER ACTIONS */}
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

        <div className="flex-1" />

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

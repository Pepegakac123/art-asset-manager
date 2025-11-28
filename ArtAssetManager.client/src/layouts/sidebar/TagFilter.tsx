import { Autocomplete, AutocompleteItem } from "@heroui/autocomplete";
import { Chip } from "@heroui/chip";
import { Switch } from "@heroui/switch";
import { Tooltip } from "@heroui/tooltip";
import { Search, Hash, Info } from "lucide-react";
import { useGalleryStore } from "../../features/gallery/stores/useGalleryStore";
import { useMemo } from "react";
import { useTags } from "./hooks/useTags";
import { Skeleton } from "@heroui/skeleton";

export const TagFilter = () => {
  const filters = useGalleryStore((state) => state.filters);
  const setFilters = useGalleryStore((state) => state.setFilters);

  const selectedTags = filters.tags;
  const isStrictMode = filters.matchAllTags;

  const { tags: apiTags, isLoading } = useTags();

  const handleToggleTag = (tag: string) => {
    const newTags = selectedTags.includes(tag)
      ? selectedTags.filter((t) => t !== tag)
      : [...selectedTags, tag];

    setFilters({ tags: newTags });
  };

  const handleStrictModeChange = (isSelected: boolean) => {
    setFilters({ matchAllTags: isSelected });
  };

  const filteredAutocompleteItems = useMemo(() => {
    if (!apiTags) return [];
    return apiTags
      .filter((tag) => !selectedTags.includes(tag.name))
      .map((tag) => ({
        label: tag.name,
        value: tag.name,
      }));
  }, [apiTags, selectedTags]);
  // 10 tagów jako "Popular" (backend zwraca je posortowane)
  const popularTagsList = useMemo(() => {
    if (!apiTags) return [];
    return apiTags.slice(0, 10);
  }, [apiTags]);

  return (
    <div className="px-2 mb-6">
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
          onValueChange={handleStrictModeChange}
          aria-label="Strict Mode"
          color="primary"
        >
          <span className="text-[10px] text-default-500 font-medium">
            {isStrictMode ? "STRICT" : "LOOSE"}
          </span>
        </Switch>
      </div>

      <Autocomplete
        aria-label="Filter tags"
        placeholder="Add tag filter..."
        size="sm"
        variant="flat"
        radius="md"
        startContent={<Search size={16} className="text-default-400" />}
        items={filteredAutocompleteItems}
        isLoading={isLoading}
        selectedKey={null}
        isVirtualized={true}
        maxListboxHeight={250}
        itemHeight={32}
        onSelectionChange={(key) => {
          if (key) {
            handleToggleTag(key.toString());
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
            <div className="flex items-center justify-between gap-2">
              <div className="flex items-center gap-2">
                <Hash size={14} className="text-default-400" />
                <span>{item.label}</span>
              </div>
            </div>
          </AutocompleteItem>
        )}
      </Autocomplete>

      {selectedTags.length > 0 && (
        <div className="flex flex-wrap gap-2 mt-3 mb-2 animate-appearance-in">
          {selectedTags.map((tag) => (
            <Chip
              key={tag}
              size="md"
              variant="solid"
              color="primary"
              onClose={() => handleToggleTag(tag)}
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
      {/* 4. POPULAR TAGS (Dynamic section) */}
      <div className="mt-5">
        <p className="text-[10px] font-semibold uppercase tracking-[0.15em] text-default-400/80 mb-2.5 px-1">
          Popular Tags
        </p>

        {isLoading ? (
          <div className="flex flex-wrap gap-2 opacity-70">
            <Skeleton className="h-7 w-16 rounded-lg" />
          </div>
        ) : popularTagsList.length > 0 ? (
          <div className="flex flex-wrap gap-2">
            {popularTagsList.map((tag) => {
              const isActive = selectedTags.includes(tag.name);
              return (
                <Chip
                  key={tag.id}
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
                  onClick={() => handleToggleTag(tag.name)}
                >
                  {tag.name}
                </Chip>
              );
            })}
          </div>
        ) : (
          <div className="flex items-center gap-2 px-1 text-default-400">
            <Info size={14} className="opacity-50" />
            <span className="text-[10px] font-medium opacity-60">
              No tags yet. Tag assets to populate list.
            </span>
          </div>
        )}
      </div>
    </div>
  );
};

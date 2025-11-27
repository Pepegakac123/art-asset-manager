import { useEffect, useState } from "react";
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
  Search,
  ChevronDown,
  ArrowUpAZ,
  ArrowDownAZ,
  Minus,
  Plus,
  RefreshCcw,
} from "lucide-react";
import { useGalleryStore, SortOption } from "../stores/useGalleryStore";
import { useShallow } from "zustand/react/shallow";
import { UI_CONFIG } from "@/config/constants";
import { useLocation } from "react-router-dom";

export const TopToolbar = () => {
  // 1. Pobieramy stan (używając nowego obiektu filters!)
  const {
    zoomLevel,
    setZoomLevel,
    viewMode,
    setViewMode,
    sortOption,
    setSortOption,
    sortDesc,
    toggleSortDirection,
    filters,
    setFilters,
    resetFilters,
  } = useGalleryStore(
    useShallow((state) => ({
      zoomLevel: state.zoomLevel,
      setZoomLevel: state.setZoomLevel,
      viewMode: state.viewMode,
      setViewMode: state.setViewMode,
      sortOption: state.sortOption,
      setSortOption: state.setSortOption,
      sortDesc: state.sortDesc,
      toggleSortDirection: state.toggleSortDirection,
      filters: state.filters,
      setFilters: state.setFilters,
      resetFilters: state.resetFilters,
    })),
  );

  const [searchValue, setSearchValue] = useState(filters.searchQuery);
  const location = useLocation();

  const getPageTitle = () => {
    switch (location.pathname) {
      case "/favorites":
        return "Favorites";
      case "/trash":
        return "Recycle Bin";
      case "/uncategorized":
        return "Uncategorized";
      // TODO: Dla kolekcji trzeba by pobrać nazwę z API, na razie placeholder
      case (location.pathname.match(/^\/collections\/\d+/) || {}).input:
        return "Collection";
      case "/":
      default:
        return "All Assets";
    }
  };
  // Aktualizuj Store dopiero po 400ms bezczynności
  useEffect(() => {
    const handler = setTimeout(() => {
      if (searchValue !== filters.searchQuery) {
        setFilters({ searchQuery: searchValue });
      }
    }, 400);

    return () => clearTimeout(handler);
  }, [searchValue, setFilters, filters.searchQuery]);
  useEffect(() => {
    setSearchValue(filters.searchQuery);
  }, [filters.searchQuery]);

  // Helper do wyświetlania nazwy sortowania
  const getSortLabel = (option: SortOption) => {
    switch (option) {
      case "dateadded":
        return "Date Added";
      case "filename":
        return "File Name";
      case "filesize":
        return "File Size";
      case "lastmodified":
        return "Last Modified";
      default:
        return option;
    }
  };

  return (
    <div className="sticky top-0 z-50 flex h-16 w-full items-center justify-between border-b border-default-200 bg-background/80 px-6 backdrop-blur-md">
      <div className="flex items-center gap-4">
        <h1 className="text-lg font-bold tracking-tight text-foreground">
          {getPageTitle()}
        </h1>
        {/* TODO: Podpiąć tutaj totalCount z React Query (wymaga przekazania propsa lub contextu) */}
        {/* Na razie placeholder */}
        {/* <span className="rounded-full bg-default-100 px-2.5 py-0.5 text-xs font-medium text-default-500">
          ... items
        </span> */}
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
          placeholder="Search by filename..."
          size="sm"
          startContent={<Search size={18} />}
          type="search"
          value={searchValue}
          onValueChange={setSearchValue}
          isClearable
          onClear={() => setSearchValue("")}
        />
      </div>

      {/* SEKCJA C: KONTROLA */}
      <div className="flex items-center gap-4">
        {/* 1. RESET FILTRÓW (Opcjonalny, ale przydatny) */}
        <Button
          isIconOnly
          variant="light"
          size="sm"
          onPress={resetFilters}
          title="Reset filters"
        >
          <RefreshCcw size={16} className="text-default-400" />
        </Button>

        <div className="h-6 w-px bg-default-300" />

        {/* 2. ZOOM SLIDER */}
        <div className="flex w-48 items-center gap-2">
          <Slider
            size="sm"
            step={50}
            color="primary"
            maxValue={UI_CONFIG.GALLERY.MAX_ZOOM}
            minValue={UI_CONFIG.GALLERY.MIN_ZOOM}
            aria-label="Thumbnail Size"
            value={zoomLevel}
            onChange={(v) => {
              const val = Array.isArray(v) ? v[0] : v;
              setZoomLevel(val);
            }}
            startContent={
              <button
                type="button"
                className="rounded-full p-1 text-default-400 outline-none transition-colors hover:cursor-pointer hover:text-primary focus-visible:ring-2 focus-visible:ring-primary"
                onClick={() =>
                  setZoomLevel(
                    Math.max(UI_CONFIG.GALLERY.MIN_ZOOM, zoomLevel - 50),
                  )
                }
              >
                <Minus size={14} />
              </button>
            }
            endContent={
              <button
                type="button"
                className="rounded-full p-1 text-default-400 outline-none transition-colors hover:cursor-pointer hover:text-primary focus-visible:ring-2 focus-visible:ring-primary"
                onClick={() =>
                  setZoomLevel(
                    Math.min(UI_CONFIG.GALLERY.MAX_ZOOM, zoomLevel + 50),
                  )
                }
              >
                <Plus size={14} />
              </button>
            }
          />
        </div>

        <div className="h-6 w-px bg-default-300" />

        {/* 3. SORTOWANIE (Split Button Pattern) */}
        <div className="flex items-center gap-1">
          <Dropdown>
            <DropdownTrigger>
              <Button
                variant="flat"
                size="sm"
                endContent={<ChevronDown size={16} />}
                className="text-default-600 capitalize min-w-[120px] justify-between"
              >
                {getSortLabel(sortOption)}
              </Button>
            </DropdownTrigger>
            <DropdownMenu
              aria-label="Sort options"
              disallowEmptySelection
              selectionMode="single"
              selectedKeys={new Set([sortOption])}
              onSelectionChange={(keys) => {
                const selected = Array.from(keys)[0] as SortOption;
                setSortOption(selected);
              }}
            >
              <DropdownItem key="dateadded">Date Added</DropdownItem>
              <DropdownItem key="filename">File Name</DropdownItem>
              <DropdownItem key="filesize">File Size</DropdownItem>
              <DropdownItem key="lastmodified">Last Modified</DropdownItem>
            </DropdownMenu>
          </Dropdown>

          <Button
            isIconOnly
            variant="flat"
            size="sm"
            onPress={toggleSortDirection}
            title={sortDesc ? "Descending" : "Ascending"}
          >
            {sortDesc ? <ArrowDownAZ size={18} /> : <ArrowUpAZ size={18} />}
          </Button>
        </div>

        {/* 4. VIEW TOGGLE */}
        <ButtonGroup variant="flat" size="sm">
          <Button
            isIconOnly
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
        </ButtonGroup>
      </div>
    </div>
  );
};

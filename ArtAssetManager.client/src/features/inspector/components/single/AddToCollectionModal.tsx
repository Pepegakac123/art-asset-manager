import { useState, useMemo } from "react";
import {
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
} from "@heroui/modal";
import { Button } from "@heroui/button";
import { Input } from "@heroui/input";
import { ScrollShadow } from "@heroui/scroll-shadow";
import { Search, FolderOpen, Plus, Check, Loader2, Shapes } from "lucide-react";
import { Asset } from "@/types/api";
import { useMaterialSets } from "@/layouts/sidebar/hooks/useMaterialSets";

interface AddToCollectionModalProps {
  isOpen: boolean;
  onOpenChange: (open: boolean) => void;
  asset: Asset;
}

export const AddToCollectionModal = ({
  isOpen,
  onOpenChange,
  asset,
}: AddToCollectionModalProps) => {
  const { materialSets, addAssetToSet } = useMaterialSets();
  const [searchQuery, setSearchQuery] = useState("");

  const [loadingSetId, setLoadingSetId] = useState<number | null>(null);

  const filteredSets = useMemo(() => {
    return materialSets.filter((set) =>
      set.name.toLowerCase().includes(searchQuery.toLowerCase()),
    );
  }, [materialSets, searchQuery]);

  const assetSetIds = useMemo(() => {
    return (asset.materialSets || []).map((s) => s.id);
  }, [asset.materialSets]);

  const handleAdd = async (setId: number) => {
    setLoadingSetId(setId);
    try {
      await addAssetToSet({ setId, assetId: asset.id });
    } finally {
      setLoadingSetId(null);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onOpenChange={onOpenChange}
      scrollBehavior="inside"
      backdrop="blur"
      size="md"
    >
      <ModalContent>
        {(onClose) => (
          <>
            <ModalHeader className="flex flex-col gap-1">
              Add to Collection
              <span className="text-tiny font-normal text-default-400">
                Select collections for{" "}
                <span className="font-mono text-foreground">
                  {asset.fileName}
                </span>
              </span>
            </ModalHeader>

            <ModalBody className="pt-0">
              {/* SEARCH */}
              <Input
                placeholder="Search collections..."
                startContent={<Search size={16} className="text-default-400" />}
                value={searchQuery}
                onValueChange={setSearchQuery}
                variant="faded"
                size="sm"
                classNames={{ inputWrapper: "bg-default-100" }}
              />

              {/* LISTA KOLEKCJI */}
              <ScrollShadow className="h-[300px] mt-2">
                <div className="flex flex-col gap-1">
                  {filteredSets.length > 0 ? (
                    filteredSets.map((set) => {
                      const isAlreadyAdded = assetSetIds.includes(set.id);
                      const isLoading = loadingSetId === set.id;

                      return (
                        <div
                          key={set.id}
                          className="flex items-center justify-between p-2 rounded-lg hover:bg-default-100 transition-colors border border-transparent hover:border-default-200"
                        >
                          <div className="flex items-center gap-3 overflow-hidden">
                            <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center text-primary">
                              <Shapes
                                size={16}
                                style={{ color: set.customColor || undefined }}
                                className={
                                  !set.customColor ? "text-primary" : ""
                                }
                              />
                            </div>
                            <span className="text-small text-default-700 truncate">
                              {set.name}
                            </span>
                          </div>

                          {/* ACTION BUTTON */}
                          {isAlreadyAdded ? (
                            <Button
                              size="sm"
                              isIconOnly
                              variant="flat"
                              color="success"
                              className="bg-success/10 text-success cursor-default rounded-full"
                            >
                              <Check size={16} />
                            </Button>
                          ) : (
                            <Button
                              size="sm"
                              isIconOnly
                              variant="light"
                              className="text-default-400 hover:text-primary hover:bg-primary/10 rounded-full"
                              onPress={() => handleAdd(set.id)}
                              isLoading={isLoading}
                            >
                              {!isLoading && <Plus size={18} />}
                            </Button>
                          )}
                        </div>
                      );
                    })
                  ) : (
                    <div className="text-center py-8 text-default-400">
                      <p>No collections found.</p>
                    </div>
                  )}
                </div>
              </ScrollShadow>
            </ModalBody>
            <ModalFooter>
              <Button variant="light" onPress={onClose}>
                Done
              </Button>
            </ModalFooter>
          </>
        )}
      </ModalContent>
    </Modal>
  );
};

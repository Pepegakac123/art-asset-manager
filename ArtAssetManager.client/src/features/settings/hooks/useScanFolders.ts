import { scannerService } from "@/services/scannerService";
import { addToast } from "@heroui/toast";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { CheckCircle2, XCircle, FolderPlus, Trash2 } from "lucide-react";

export const useScanFolders = () => {
	const queryClient = useQueryClient();

	const foldersQuery = useQuery({
		queryKey: ["scan-folders"],
		queryFn: scannerService.getFolders,
	});

	const addFolderMutation = useMutation({
		mutationFn: scannerService.addFolder,
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["scan-folders"] });
			addToast({
				title: "Success",
				description: "New folder linked to library.",
				color: "success",
				severity: "success",
				variant: "flat",
				timeout: 3000,
			});
		},
		onError: (error) => {
			const msg = error.message || "Unknown error";
			addToast({
				title: "Action Failed",
				description: msg,
				color: "danger",
				severity: "danger",
				variant: "flat",
			});
		},
	});

	const deleteFolderMutation = useMutation({
		mutationFn: scannerService.deleteFolder,
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["scan-folders"] });
			addToast({
				title: "Folder Removed",
				description: "It will no longer be scanned.",
				color: "warning",
				severity: "warning",
				variant: "flat",
			});
		},
		onError: (error) => {
			addToast({
				title: "Could not delete folder",
				description: error.message,
				color: "danger",
				severity: "danger",
				variant: "flat",
			});
		},
	});
	const validateMutation = useMutation({
		mutationFn: scannerService.validatePath,
	});
	return {
		folders: foldersQuery.data,
		isLoading: foldersQuery.isLoading,
		addFolder: addFolderMutation.mutateAsync,
		deleteFolder: deleteFolderMutation.mutateAsync,
		validatePath: validateMutation.mutateAsync,
		isValidating: validateMutation.isPending,
	};
};

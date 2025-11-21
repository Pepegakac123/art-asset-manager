// src/components/layout/Sidebar.tsx
import {Button, ButtonGroup} from "@heroui/button";
import {ScrollShadow} from "@heroui/scroll-shadow";
import { Home, FolderOpen, Tag, Settings, Layers, Heart } from "lucide-react";
import { Link, useLocation } from "react-router-dom";

export const Sidebar = () => {
	const location = useLocation();

	// Pomocnicza funkcja do sprawdzania aktywnego linku (później to rozbudujemy)
	const isActive = (path: string) => location.pathname === path;

	return (
		<aside className="h-full w-full flex flex-col border-r border-default-200 bg-content1">
			{/* 1. LOGO AREA */}
			<div className="flex h-16 flex-shrink-0 items-center px-6">
				<div className="flex items-center gap-2 text-xl font-bold tracking-tighter">
					<Layers className="h-6 w-6 text-primary" />
					<span>
						ArtAsset<span className="text-primary">Mngr</span>
					</span>
				</div>
			</div>

			{/* 2. NAVIGATION (SCROLLABLE) */}
			<ScrollShadow className="flex-1 px-4 py-2" hideScrollBar>
				<div className="flex flex-col gap-1">
					{/* Sekcja: Library */}
					<p className="px-2 mb-2 text-xs font-medium uppercase text-default-500 mt-2">
						Library
					</p>

					<Button
						as={Link}
						to="/"
						variant={isActive("/") ? "flat" : "light"}
						color={isActive("/") ? "primary" : "default"}
						className="justify-start"
						startContent={<Home size={20} />}
					>
						All Assets
					</Button>

					<Button
						as={Link}
						to="/favorites" // Placeholder link
						variant="light"
						className="justify-start"
						startContent={<Heart size={20} />}
					>
						Favorites
					</Button>

					{/* Sekcja: Organization */}
					<p className="px-2 mb-2 text-xs font-medium uppercase text-default-500 mt-6">
						Organization
					</p>

					<Button
						variant="light"
						className="justify-start"
						startContent={<FolderOpen size={20} />}
					>
						Folders
					</Button>

					<Button
						variant="light"
						className="justify-start"
						startContent={<Tag size={20} />}
					>
						Tags
					</Button>
				</div>
			</ScrollShadow>

			{/* 3. FOOTER / SETTINGS */}
			<div className="flex-shrink-0 border-t border-default-200 p-4">
				<Button
					variant="light"
					className="w-full justify-start text-default-500 hover:text-foreground"
					startContent={<Settings size={20} />}
				>
					Settings
				</Button>
			</div>
		</aside>
	);
};

import { Button, ButtonGroup } from "@heroui/button";
import { ScrollShadow } from "@heroui/scroll-shadow";
import {
	Home,
	FolderOpen,
	Tag,
	Settings,
	Layers,
	Heart,
	type LucideIcon,
} from "lucide-react";
import { Link, useLocation } from "react-router-dom";

interface SidebarItemProps {
	icon: LucideIcon;
	label: string;
	to: string;
}

const SidebarItem = ({ icon: Icon, label, to }: SidebarItemProps) => {
	const location = useLocation();
	const isActive =
		to === "/" ? location.pathname === "/" : location.pathname.startsWith(to);

	return (
		<Button
			as={Link}
			to={to}
			disableRipple={false}
			radius="md"
			className={`w-full justify-start h-10 gap-4 px-4 transition-colors ${
				isActive
					? "bg-primary/15 text-primary font-semibold" // Aktywny: Pomarańcz
					: "bg-transparent text-default-500 hover:bg-default-100 hover:text-foreground"
			}`}
		>
			<Icon size={20} strokeWidth={isActive ? 2.5 : 2} />
			<span className="truncate">{label}</span>
		</Button>
	);
};

export const Sidebar = () => {
	return (
		<aside className="h-full w-full flex flex-col border-r border-default-200 bg-content1">
			{/* LOGO AREA */}
			<div className="flex h-16 flex-shrink-0 items-center px-6 border-b border-default-200">
				<div className="flex items-center gap-3 text-xl font-bold tracking-tight">
					<div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary/10 text-primary">
						<Layers size={20} />
					</div>

					<span className="text-foreground">
						ArtAsset<span className="text-primary">Mngr</span>
					</span>
				</div>
			</div>

			{/* NAVIGATION LIST */}
			<ScrollShadow className="flex-1 p-3 space-y-6" hideScrollBar>
				{/* LIBRARY */}
				<div>
					<p className="px-4 mb-2 text-xs font-bold uppercase tracking-wider text-default-400/80">
						Library
					</p>
					<div className="flex flex-col gap-1">
						<SidebarItem icon={Home} label="All Assets" to="/" />
						<SidebarItem icon={Heart} label="Favorites" to="/favorites" />
					</div>
				</div>

				{/* ORGANIZATION */}
				<div>
					<p className="px-4 mb-2 text-xs font-bold uppercase tracking-wider text-default-400/80">
						Organization
					</p>
					<div className="flex flex-col gap-1">
						<SidebarItem icon={FolderOpen} label="Folders" to="/folders" />
						<SidebarItem icon={Tag} label="Tags" to="/tags" />
					</div>
				</div>
			</ScrollShadow>

			{/* FOOTER */}
			<div className="flex-shrink-0 border-t border-default-200 p-3">
				<Button
					variant="light"
					className="w-full justify-start gap-4 px-4 text-default-500 hover:text-foreground"
					startContent={<Settings size={20} />}
				>
					Settings
				</Button>
			</div>
		</aside>
	);
};

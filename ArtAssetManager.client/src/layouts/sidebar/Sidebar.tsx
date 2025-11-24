import { Button } from "@heroui/button";
import { ScrollShadow } from "@heroui/scroll-shadow";
import {
	Home,
	Settings,
	Layers,
	Heart,
	Box,
	Shapes,
	Trash2, // Import ikony kosza
} from "lucide-react";
import { SidebarSection } from "./SidebarSection";
import { SidebarItem } from "./SidebarItem";
import { TagFilter } from "./TagFilter";
import { useNavigate } from "react-router-dom";

/*
TODO: [API] Dynamic Statistics
- "All Assets count", "Trash count": Pobierać te liczby z API (np. endpoint GET /api/stats/sidebar).
- Uncategorized count: To ważna metryka dla workflow "Inbox Zero".

TODO: [FEATURE] Smart Collections & Drag-Drop
- UI Guidelines (Sekcja 6.2): Dodać sekcję "Smart Collections" (zapisane filtry z bazy).
- UI Guidelines (Sekcja 6.2): Obsłużyć Drag & Drop - przeciąganie assetu z Gridu na nazwę Kolekcji w Sidebarze powinno go do niej dodać.

TODO: [ROUTING] Active Link Logic
- Upewnić się, że kliknięcie w "Trash" faktycznie zmienia filtr w Store (`isDeleted: true`) i odświeża Grid.
*/

export const Sidebar = () => {
	const navigate = useNavigate();
	return (
		<aside className="h-full w-full flex flex-col border-r border-default-200 bg-content1/50 backdrop-blur-xl">
			{/* LOGO */}
			<div className="flex h-16 flex-shrink-0 items-center px-5 border-b border-default-100/50">
				<div className="flex items-center gap-3 text-xl font-bold tracking-tight select-none">
					<div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary/10 text-primary ring-1 ring-primary/20">
						<Layers size={20} />
					</div>
					<span className="text-foreground text-lg">
						ArtAsset<span className="text-primary">Mngr</span>
					</span>
				</div>
			</div>

			{/* NAVIGATION LIST */}
			<ScrollShadow className="flex-1 py-4 px-3 custom-scrollbar">
				{/* LIBRARY */}
				<SidebarSection title="Library">
					<SidebarItem icon={Home} label="All Assets" to="/" count={1240} />
					<SidebarItem
						icon={Heart}
						label="Favorites"
						to="/favorites"
						count={42}
					/>
					<SidebarItem
						icon={Box}
						label="Uncategorized"
						to="/uncategorized"
						count={128}
					/>
					<SidebarItem icon={Trash2} label="Trash" to="/trash" count={12} />
				</SidebarSection>

				{/* COLLECTIONS */}
				<SidebarSection title="Collections">
					{/* max-h-48 (ok 192px) sprawi, że jak będzie ich dużo, pojawi się scroll wewnątrz sekcji */}
					<ScrollShadow
						className="max-h-48 custom-scrollbar"
						hideScrollBar={false}
					>
						<div className="flex flex-col gap-0.5 pr-1">
							<SidebarItem
								icon={Shapes}
								label="Sci-Fi Project A"
								to="/c/scifi"
							/>
							<SidebarItem
								icon={Shapes}
								label="Fantasy Props"
								to="/c/fantasy"
							/>
							<SidebarItem
								icon={Shapes}
								label="Instagram Dailies"
								to="/c/dailies"
							/>
							<SidebarItem
								icon={Shapes}
								label="Client Work 2024"
								to="/c/work"
							/>
							<SidebarItem icon={Shapes} label="Old References" to="/c/refs" />
							<SidebarItem icon={Shapes} label="Environment Kit" to="/c/env" />
							<SidebarItem icon={Shapes} label="Weapon Parts" to="/c/weap" />
						</div>
					</ScrollShadow>

					<Button
						size="sm"
						variant="light"
						className="w-full justify-start h-8 text-xs text-default-400 data-[hover=true]:text-primary mt-1 pl-2"
						startContent={<span className="text-lg font-light mr-1">+</span>}
					>
						New Collection
					</Button>
				</SidebarSection>

				{/* TAGS */}
				<SidebarSection title="Tags">
					<TagFilter />
				</SidebarSection>
			</ScrollShadow>

			{/* FOOTER */}
			<div className="flex-shrink-0 border-t border-default-200 p-3 bg-content1">
				<Button
					variant="light"
					className="w-full justify-start gap-3 px-3 text-default-500 hover:text-foreground"
					startContent={<Settings size={18} />}
					onPress={() => navigate("/settings")}
				>
					Settings
				</Button>
			</div>
		</aside>
	);
};

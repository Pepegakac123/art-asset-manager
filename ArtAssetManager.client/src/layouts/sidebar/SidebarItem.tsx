import { useLocation, Link } from "react-router-dom";

interface SidebarItemProps {
	icon: any;
	label: string;
	to?: string;
	count?: number;
	onClick?: () => void;
}

export const SidebarItem = ({
	icon: Icon,
	label,
	to,
	count,
	onClick,
}: SidebarItemProps) => {
	const location = useLocation();
	const isActive = to
		? to === "/"
			? location.pathname === "/"
			: location.pathname.startsWith(to)
		: false;

	const content = (
		<>
			<Icon
				size={18}
				strokeWidth={isActive ? 2.5 : 2}
				className={isActive ? "text-primary" : "text-default-500"}
			/>
			<span
				className={`flex-1 truncate text-sm ${isActive ? "text-foreground font-medium" : "text-default-600"}`}
			>
				{label}
			</span>
			{count !== undefined && (
				<span className="text-[10px] font-bold text-default-400 bg-default-100 px-2 py-0.5 rounded-full">
					{count}
				</span>
			)}
		</>
	);

	const className = `w-full flex items-center gap-3 px-2 h-9 rounded-md transition-colors cursor-pointer select-none ${
		isActive ? "bg-primary/10" : "hover:bg-default-100"
	}`;

	if (to) {
		return (
			<Link to={to} className={className}>
				{content}
			</Link>
		);
	}

	return (
		// biome-ignore lint/a11y/noStaticElementInteractions: <explanation>
		// biome-ignore lint/a11y/useKeyWithClickEvents: <explanation>
		<div onClick={onClick} className={className}>
			{content}
		</div>
	);
};

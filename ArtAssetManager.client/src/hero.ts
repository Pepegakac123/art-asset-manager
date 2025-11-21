// src/hero.ts
import { heroui } from "@heroui/react";

// ZMIANA: Musi być 'export default', żeby @plugin w CSS to złapał!
export default heroui({
	themes: {
		dark: {
			colors: {
				background: "#09090b", // Zinc-950
				foreground: "#ecedee", // Zinc-100

				content1: "#18181b", // Zinc-900
				content2: "#27272a", // Zinc-800
				content3: "#3f3f46", // Zinc-700
				content4: "#52525b", // Zinc-600

				primary: {
					DEFAULT: "#FAFAFA",
					foreground: "#09090b",
				},

				focus: "#a855f7",

				danger: {
					DEFAULT: "#f31260",
					foreground: "#ffffff",
				},
				success: {
					DEFAULT: "#17c964",
					foreground: "#000000",
				},

				default: {
					DEFAULT: "#3f3f46",
					foreground: "#ecedee",
				},
			},
			layout: {
				radius: {
					small: "4px",
					medium: "8px",
					large: "12px",
				},
				borderWidth: {
					small: "1px",
					medium: "1px",
					large: "2px",
				},
			},
		},
	},
});

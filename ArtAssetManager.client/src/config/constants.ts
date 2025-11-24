// Zamiast enuma:
export const UI_CONFIG = {
	GALLERY: {
		DEFAULT_ZOOM: 220,
		MIN_ZOOM: 220,
		MAX_ZOOM: 400,
		AllowedDisplayContentModes: {
			default: "default",
			favorites: "favorites",
			uncategorized: "uncategorized",
			trash: "trash",
			collection: "collection",
		},
	},
	QUERY: {
		STALE_TIME: 1000 * 60 * 5, // 5 minut
		GC_TIME: 1000 * 60 * 15, // 15 minut
		RETRY_COUNT: 3,
	},
} as const;

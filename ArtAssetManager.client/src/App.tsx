import { Route, Routes } from "react-router-dom";
import { MainLayout } from "./layouts/MainLayout";
import { GalleryGrid } from "./features/gallery/components/GalleryGrid";
import { UI_CONFIG } from "./config/constants";

function App() {
	return (
		<Routes>
			<Route element={<MainLayout />}>
				<Route
					path="/"
					element={
						<GalleryGrid
							mode={UI_CONFIG.GALLERY.AllowedDisplayContentModes.default}
						/>
					}
				/>
				<Route
					path="/favorites"
					element={
						<GalleryGrid
							mode={UI_CONFIG.GALLERY.AllowedDisplayContentModes.favorites}
						/>
					}
				/>
				<Route
					path="/uncategorized"
					element={
						<GalleryGrid
							mode={UI_CONFIG.GALLERY.AllowedDisplayContentModes.uncategorized}
						/>
					}
				/>
				<Route
					path="/trash"
					element={
						<GalleryGrid
							mode={UI_CONFIG.GALLERY.AllowedDisplayContentModes.trash}
						/>
					}
				/>
				<Route
					path="/collections/:collectionId"
					element={
						<GalleryGrid
							mode={UI_CONFIG.GALLERY.AllowedDisplayContentModes.collection}
						/>
					}
				/>
				<Route path="/settings" element={<div>Settings</div>} />
			</Route>
		</Routes>
	);
}

export default App;

import { Route, Routes } from "react-router-dom";
import { MainLayout } from "./layouts/MainLayout";
import { GalleryGrid } from "./features/gallery/components/GalleryGrid";

function App() {
	return (
		<Routes>
			<Route element={<MainLayout />}>
				<Route path="/" element={<GalleryGrid />} />
				{/* Inne trasy później */}
			</Route>
		</Routes>
	);
}

export default App;

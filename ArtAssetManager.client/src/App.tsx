import { Route, Routes } from "react-router-dom";
import { MainLayout } from "./layouts/MainLayout";

function App() {
	return (
		<Routes>
			<Route element={<MainLayout />}>
				<Route path="/" element={<div>Home</div>} />
				{/* Inne trasy później */}
			</Route>
		</Routes>
	);
}

export default App;

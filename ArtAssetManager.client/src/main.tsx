import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { HeroUIProvider } from "@heroui/react";
import { ThemeProvider as NextThemesProvider } from "next-themes";
import App from "./App";
import "./index.css";

const queryClient = new QueryClient({
	defaultOptions: {
		queries: {
			staleTime: 1000 * 60 * 5,
			retry: 1,
		},
	},
});

ReactDOM.createRoot(document.getElementById("root")!).render(
	<React.StrictMode>
		<BrowserRouter>
			<QueryClientProvider client={queryClient}>
				<HeroUIProvider>
					{/* NextThemesProvider zarządza klasą 'dark' na elemencie <html> */}
					<NextThemesProvider attribute="class" defaultTheme="dark">
						<App />
					</NextThemesProvider>
				</HeroUIProvider>
			</QueryClientProvider>
		</BrowserRouter>
	</React.StrictMode>,
);

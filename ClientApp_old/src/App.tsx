import "./App.css";

import { RouterProvider, createRouter } from "@tanstack/react-router";

import { QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { SideBarContextProvider } from "~/contexts/SideBarContextProvider";
import { ThemeProvider } from "./chadcn/theme-provider";
import { TouchProvider } from "./chadcn/ui/hybrid-tooltip";
import { TooltipProvider } from "./chadcn/ui/tooltip";
import { AppGeneralContextProvider } from "./contexts/AppGeneralContextProvider";
import {
  AuthenticationContextProvider,
  useAuthenticationContext,
} from "./contexts/AuthenticationContextProvider";
import LoadingPage from "./pages/LoadingPage";
import { queryClient } from "./queryClient";
import { routeTree } from "./routeTree.gen";

export const router = createRouter({
  routeTree,
  defaultPendingComponent: LoadingPage,
  context: {
    userLoggedIn: undefined!,
  },
  defaultPreload: "intent",
  defaultPreloadStaleTime: 0,
  // defaultPendingMs: 5000,
  defaultPendingMinMs: 2000,
});

declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AppGeneralContextProvider>
        <AuthenticationContextProvider>
          <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
            <TouchProvider>
              <TooltipProvider>
                <SideBarContextProvider>
                  <InnerApp />
                </SideBarContextProvider>
              </TooltipProvider>
            </TouchProvider>
          </ThemeProvider>
        </AuthenticationContextProvider>
      </AppGeneralContextProvider>
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}

function InnerApp() {
  const { userLoggedIn } = useAuthenticationContext();
  return <RouterProvider router={router} context={{ userLoggedIn }} />;
}

export default App;

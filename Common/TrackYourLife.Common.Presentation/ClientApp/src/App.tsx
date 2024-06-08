import "./App.css";

import { RouterProvider, createRouter } from "@tanstack/react-router";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { SideBarContextProvider } from "~/contexts/SideBarContextProvider";
import { ThemeProvider } from "./chadcn/theme-provider";
import { ApiContextProvider } from "./contexts/ApiContextProvider";
import { AppGeneralContextProvider } from "./contexts/AppGeneralContextProvider";
import {
  AuthenticationContextProvider,
  useAuthenticationContext,
} from "./contexts/AuthenticationContextProvider";
import { useApiRequests } from "./hooks/useApiRequests";
import LoadingPage from "./pages/LoadingPage";
import { routeTree } from "./routeTree.gen";

const queryClient = new QueryClient();

export const router = createRouter({
  routeTree,
  defaultPendingComponent: LoadingPage,
  context: {
    userLoggedIn: undefined!,
    fetchRequest: undefined!,
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
        <ApiContextProvider>
          <AuthenticationContextProvider>
            <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
              <SideBarContextProvider>
                <InnerApp />
              </SideBarContextProvider>
            </ThemeProvider>
          </AuthenticationContextProvider>
        </ApiContextProvider>
      </AppGeneralContextProvider>
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}

function InnerApp() {
  const { userLoggedIn } = useAuthenticationContext();
  const { fetchRequest } = useApiRequests();
  return (
    <RouterProvider router={router} context={{ userLoggedIn, fetchRequest }} />
  );
}

export default App;

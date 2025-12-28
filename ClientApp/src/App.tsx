import { QueryClientProvider } from "@tanstack/react-query";
import { createRouter, RouterProvider } from "@tanstack/react-router";

import { ThemeProvider } from "./components/theme-provider";
import { AppGeneralContextProvider } from "./contexts/AppGeneralContextProvider";
import {
  AuthenticationContextProvider,
  useAuthenticationContext,
} from "./contexts/AuthenticationContextProvider";
import LoadingPage from "./pages/LoadingPage";
import { queryClient } from "./queryClient";
import { routeTree } from "./routeTree.gen";

// eslint-disable-next-line react-refresh/only-export-components
export const router = createRouter({
  routeTree,
  defaultPendingComponent: () => {
    return <LoadingPage />;
  },

  defaultErrorComponent: () => {
    return <h1>Error</h1>;
  },
  notFoundMode: "root",

  context: {
    userLoggedIn: undefined!,
  },
  defaultPreload: "intent",
  defaultPreloadStaleTime: 1000 * 60 * 5, // 5 minutes
  defaultPreloadDelay: 100, // 100ms delay before preloading
});

declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthenticationContextProvider>
        <InnerApp />
      </AuthenticationContextProvider>
    </QueryClientProvider>
  );
}

function InnerApp() {
  // Performance monitoring for the main app
  const { userLoggedIn } = useAuthenticationContext();
  return (
    <AppGeneralContextProvider>
      <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
        <RouterProvider router={router} context={{ userLoggedIn }} />
      </ThemeProvider>
    </AppGeneralContextProvider>
  );
}

export default App;

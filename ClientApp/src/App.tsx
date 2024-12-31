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
  defaultPendingComponent: LoadingPage,
  context: {
    userLoggedIn: undefined!,
  },
  defaultPreload: "intent",
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
      <AuthenticationContextProvider>
        <InnerApp />
      </AuthenticationContextProvider>
    </QueryClientProvider>
  );
}

function InnerApp() {
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

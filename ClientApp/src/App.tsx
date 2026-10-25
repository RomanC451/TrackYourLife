import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider } from "@tanstack/react-router";

import { ThemeProvider } from "./components/theme-provider";
import { AppGeneralContextProvider } from "./contexts/AppGeneralContextProvider";
import {
  AuthenticationContextProvider,
  useAuthenticationContext,
} from "./contexts/AuthenticationContextProvider";
import { queryClient } from "./queryClient";
import { router } from "./router";

export { router } from "./router";

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
  const { userLoggedIn, userData } = useAuthenticationContext();
  return (
    <AppGeneralContextProvider>
      <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
        <RouterProvider router={router} context={{ userLoggedIn, userData }} />
      </ThemeProvider>
    </AppGeneralContextProvider>
  );
}

export default App;

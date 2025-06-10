import React from "react";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { Outlet } from "@tanstack/react-router";
import { TanStackRouterDevtools } from "@tanstack/router-devtools";

import { useTheme } from "@/components/theme-provider";
import { Toaster } from "@/components/ui/sonner";
import { screensEnum } from "@/constants/tailwindSizes";
import { useAppGeneralStateContext } from "@/contexts/AppGeneralContextProvider";

type RootLayoutProps = React.HTMLAttributes<"div">;

const RootPageLayout: React.FC<RootLayoutProps> = () => {
  const { screenSize } = useAppGeneralStateContext();
  const { theme } = useTheme();

  return (
    <>
      <Outlet />
      <Toaster
        richColors
        expand={screenSize.width >= screensEnum.sm}
        theme={theme}
        position={
          screenSize.width >= screensEnum.sm ? "bottom-right" : "top-center"
        }
        toastOptions={{ duration: 5000 }}
      />
      {import.meta.env.MODE === "development" && (
        <>
          <TanStackRouterDevtools position="bottom-left" />
          <ReactQueryDevtools />
        </>
      )}
    </>
  );
};

export default RootPageLayout;

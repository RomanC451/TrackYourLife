import React, { PropsWithChildren } from "react";

import { Toaster } from "~/chadcn/ui/sonner";
import { cn } from "~/utils";

type RootLayoutProps = PropsWithChildren & React.HTMLAttributes<"div">;

const RootLayout: React.FC<RootLayoutProps> = ({ children, className }) => {
  return (
    <div className={cn("flex  min-h-screen w-screen flex-col ", className)}>
      {children}
      <Toaster richColors expand />
    </div>
  );
};

export default RootLayout;

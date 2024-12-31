import React, { PropsWithChildren } from "react";

import { cn } from "@/lib/utils";

type NavBarLayoutProps = PropsWithChildren & {
  navBarElement: React.ReactNode;
  className?: string;
};

function NavBarLayout({
  children,
  navBarElement,
  className,
}: NavBarLayoutProps) {
  return (
    <div className={cn("flex h-screen w-full flex-col", className)}>
      {navBarElement}
      {children}
    </div>
  );
}

export default NavBarLayout;

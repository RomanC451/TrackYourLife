import React, { PropsWithChildren } from "react";
import { cn } from "~/utils";

type NavBarLayoutProps = PropsWithChildren & {
  navBarElement: React.ReactNode;
  className?: string;
};

const NavBarLayout: React.FC<NavBarLayoutProps> = ({
  children,
  navBarElement,
  className,
}): React.JSX.Element => {
  return (
    <div className={cn("flex w-full flex-grow flex-col ", className)}>
      {navBarElement}
      {children}
    </div>
  );
};

export default NavBarLayout;

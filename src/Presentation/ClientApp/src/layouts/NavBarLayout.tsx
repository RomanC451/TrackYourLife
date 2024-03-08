import React, { PropsWithChildren } from "react";

type NavBarLayoutProps = PropsWithChildren & { navBarElement: React.ReactNode };

const NavBarLayout: React.FC<NavBarLayoutProps> = ({
  children,
  navBarElement,
}): React.JSX.Element => {
  return (
    <div className="flex flex-grow flex-col">
      {navBarElement}
      {children}
    </div>
  );
};

export default NavBarLayout;

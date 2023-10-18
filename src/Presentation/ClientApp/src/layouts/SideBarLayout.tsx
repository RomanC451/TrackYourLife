import React from "react";
import SideBar from "~/components/sidebar/SideBar";

interface SideBarLayoutProps {
  children: React.ReactNode;
}

const SideBarLayout: React.FC<SideBarLayoutProps> = ({
  children
}): JSX.Element => {
  if (React.Children.count(children) != 1) {
    throw new Error("This layout component can only have one child.");
  }

  return (
    <div className="flex h-full min-h-[100vh] min-w-[100vw]">
      <SideBar />
      {children}
    </div>
  );
};

export default SideBarLayout;

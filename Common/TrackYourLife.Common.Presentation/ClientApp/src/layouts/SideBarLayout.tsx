import React, { PropsWithChildren } from "react";
import SideBar from "~/components/sidebar/SideBar";

const SideBarLayout: React.FC<PropsWithChildren> = ({
  children,
}): JSX.Element => {
  return (
    <div className="flex">
      <SideBar />
      <div className="w-full overflow-hidden">{children}</div>
    </div>
  );
};

export default SideBarLayout;

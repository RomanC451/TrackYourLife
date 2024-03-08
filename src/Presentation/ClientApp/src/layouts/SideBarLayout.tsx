import React, { PropsWithChildren } from "react";
import SideBar from "~/components/sidebar/SideBar";

const SideBarLayout: React.FC<PropsWithChildren> = ({
  children,
}): JSX.Element => {
  // Assert.reactChildreanLengthLowerThan(children, 2);

  return (
    <div className="flex h-full w-full flex-grow">
      <SideBar />
      <div className="flex flex-grow">{children}</div>
    </div>
  );
};

export default SideBarLayout;

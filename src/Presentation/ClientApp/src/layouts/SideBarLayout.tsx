import React from "react";
import SideBar from "~/components/sidebar/SideBar";
import { screensEnum } from "~/constants/tailwindSizes";
import { useAppGeneralStateContext } from "~/contexts/AppGeneralContextProvider";
import { Assert } from "~/utils";

interface SideBarLayoutProps {
  children: React.ReactNode;
}

const SideBarLayout: React.FC<SideBarLayoutProps> = ({
  children
}): JSX.Element => {
  Assert.reactChildreanLengthLowerThan(children, 2);

  const { screenSize } = useAppGeneralStateContext();

  return (
    <div className="flex h-full min-h-[100vh] min-w-[100vw]">
      {screenSize.width > screensEnum.lg ? <SideBar /> : null}

      {children}
    </div>
  );
};

export default SideBarLayout;

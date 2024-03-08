import { useNavigate } from "@tanstack/react-router";
import { motion } from "framer-motion";
import React, { useEffect, useRef, useState } from "react";

import SideBarArrowButton from "~/components/buttons/SideBarArrowButton";
import AccountButton from "~/components/sidebar/components/AccountButton";
import LogOutButton from "~/components/sidebar/components/LogOutButton";
import SectionSeparator from "~/components/sidebar/components/SectionSeparator";
import SettingsButton from "~/components/sidebar/components/SettingsButton";
import {
  sidebarLinks,
  sidebarSectionsEnum,
} from "~/components/sidebar/components/sideBarLinks";
import { screensEnum } from "~/constants/tailwindSizes";
import { useAppGeneralStateContext } from "~/contexts/AppGeneralContextProvider";
import { useSideBarContext } from "~/contexts/SideBarContextProvider";

import SideBarButton from "./components/SideBarButton";
import SideBarHeader from "./components/SideBarHeader";

export const sideBarWidths = {
  opened: 190,
  closed: 73,
};

const SideBar = () => {
  const { screenSize } = useAppGeneralStateContext();
  const { sideBarOpened, setSidebarOpened, setSideBarWidth } =
    useSideBarContext();

  const sideBarRef = useRef<HTMLDivElement>(null);
  useEffect(() => {
    if (screenSize.width <= screensEnum.lg) setSideBarWidth(0);
  }, [screenSize]);

  const navigate = useNavigate();

  // !to do: fix t
  // const location = useLocation();

  const [activePage, setActivePage] = useState(sidebarSectionsEnum.About);

  function toggleSideBarButton(): void {
    setSidebarOpened(!sideBarOpened);
  }

  return (
    <nav className="relative ">
      {screenSize.width > screensEnum.lg ? (
        <motion.div
          initial={{
            width: sideBarWidths[sideBarOpened ? "opened" : "closed"],
          }}
          animate={{
            width: sideBarWidths[sideBarOpened ? "opened" : "closed"],
          }}
          transition={{ duration: 0.5, ease: "easeInOut" }}
          className={`h-full min-h-[750px] overflow-hidden bg-secondary`}
          ref={sideBarRef}
          onAnimationComplete={() => {
            setSideBarWidth(sideBarWidths[sideBarOpened ? "opened" : "closed"]);
          }}
        >
          <SideBarHeader sideBarOpened={sideBarOpened} />
          <div className="ml-[22px] mt-[35px] flex items-center justify-between">
            <p className="font-['Saira_Condensed'] text-[12px] font-[700] leading-[110%] ">
              MENU
            </p>
            <SideBarArrowButton
              sideBarOpened={sideBarOpened}
              toggleSideBarButton={toggleSideBarButton}
            />
          </div>
          <ol className={`mt-[14px] w-full pl-[19px] pr-[19px]`}>
            {sidebarLinks.map((link, index) => (
              <React.Fragment key={index}>
                <SideBarButton
                  key={index}
                  svg={link.svg}
                  text={link.section.toString()}
                  active={link.section === activePage}
                  onClick={() => {
                    setActivePage(link.section);
                    navigate({ to: link.link, search: {} });
                  }}
                />
              </React.Fragment>
            ))}
            <SectionSeparator />
            <LogOutButton />
            <SettingsButton />
            <SectionSeparator />
            <AccountButton />
            <SectionSeparator />
          </ol>
        </motion.div>
      ) : null}
    </nav>
  );
};

export default SideBar;

import { motion } from "framer-motion";
import React, { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useLocalStorage } from "usehooks-ts";
import useUserData from "~/auth/useUserData";
import SideBarArrowButton from "~/components/buttons/SideBarArrowButton";
import AccountButton from "~/components/sidebar/components/AccountButton";
import DarkModeButton from "~/components/sidebar/components/DarkModeButton";
import LogOutButton from "~/components/sidebar/components/LogOutButton";
import SectionSeparator from "~/components/sidebar/components/SectionSeparator";
import SettingsButton from "~/components/sidebar/components/SettingsButton";
import {
  getSidebarActivePage,
  sidebarLinks
} from "~/components/sidebar/components/sideBarLinks";

import SideBarButton from "./components/SideBarButton";
import SideBarHeader from "./components/SideBarHeader";

const sideBarWidths = {
  opened: 190,
  closed: 73
};

const SideBar = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const [acitvePage, setActivePage] = useState(getSidebarActivePage(location));

  const [sideBarOpened, setSidebarOpened] = useLocalStorage(
    "sideBarOpened",
    true
  );

  function toggleSideBarButton(): void {
    setSidebarOpened(!sideBarOpened);
  }

  return (
    <nav className="relative">
      <motion.div
        initial={{ width: sideBarWidths[sideBarOpened ? "opened" : "closed"] }}
        animate={{ width: sideBarWidths[sideBarOpened ? "opened" : "closed"] }}
        transition={{ duration: 0.5, ease: "easeInOut" }}
        className={`min-h-[750px] h-full  bg-second-gray-bg overflow-hidden`}
      >
        <SideBarHeader sideBarOpened={sideBarOpened} />
        <div className="ml-[22px] mt-[35px] flex items-center justify-between">
          <p className="font-['Saira_Condensed'] text-[12px] font-[700] leading-[110%] text-sidebar-gray ">
            MENU
          </p>
          <SideBarArrowButton
            sideBarOpened={sideBarOpened}
            toggleSideBarButton={toggleSideBarButton}
          />
        </div>
        <ol className={`pl-[19px] pr-[19px] mt-[14px] w-full`}>
          {sidebarLinks.map((link, index) => (
            <React.Fragment key={index}>
              <SideBarButton
                key={index}
                svg={link.svg}
                text={link.section.toString()}
                active={link.section === acitvePage}
                onClick={() => {
                  setActivePage(link.section);
                  if (link.link != "") {
                    navigate(link.link);
                  }
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
          <DarkModeButton />
        </ol>
      </motion.div>
    </nav>
  );
};

export default SideBar;

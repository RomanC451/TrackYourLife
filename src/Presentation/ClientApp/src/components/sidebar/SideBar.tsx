import { motion } from "framer-motion";
import React, { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useLocalStorage } from "usehooks-ts";
import DarkModeButton from "~/components/buttons/DarkModeButton";
import SideBarArrowButton from "~/components/buttons/SideBarArrowButton";
import SectionSeparator from "~/components/sidebar/SectionSeparator";
import SideBarElement from "~/components/sidebar/SideBarElement";
import {
  getSidebarActiveElement,
  sidebarSections
} from "~/components/sidebar/sideBarSections";

import SideBarHeader from "./SideBarHeader";

const sideBarWidths = {
  opened: 190,
  closed: 73
};

const SideBar = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const [activeElement, setActiveElement] = useState(
    getSidebarActiveElement(location)
  );
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
          {sidebarSections.map((section, index) => (
            <React.Fragment key={index}>
              {section.map((element, index) => (
                <SideBarElement
                  key={index}
                  svg={element.svg}
                  section={element.section}
                  active={element.section === activeElement}
                  onClick={() => {
                    setActiveElement(element.section);
                    if (element.link != "") {
                      navigate(element.link);
                    }
                  }}
                />
              ))}
              <SectionSeparator />
            </React.Fragment>
          ))}
          <DarkModeButton />
        </ol>
      </motion.div>
    </nav>
  );
};

export default SideBar;

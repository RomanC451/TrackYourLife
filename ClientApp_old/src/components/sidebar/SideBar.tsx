import { motion } from "framer-motion";

import { useNavigate, useRouterState } from "@tanstack/react-router";
import React from "react";
import { Separator } from "~/chadcn/ui/separator";
import {
  SideBarPositions,
  useSideBarContext,
} from "~/contexts/SideBarContextProvider";
import SideBarCloseButton from "../buttons/SideBarArrowButton";
import AccountButton from "./components/AccountButton";
import LogOutButton from "./components/LogOutButton";
import SettingsButton from "./components/SettingsButton";
import SideBarButton from "./components/SideBarButton";
import SideBarHeader from "./components/SideBarHeader";
import { sidebarLinks } from "./components/sideBarLinks";

const animationVariants = {
  opened: {
    width: 190,
  },
  closed: (position: SideBarPositions) => ({
    width: position === "absolute" ? 0 : 73,
  }),
};

const SideBar = () => {
  const {
    sideBarOpened,
    setSidebarOpened,
    toggleSideBarOpened,
    sideBarPosition,
    deferredSideBarPosition,
    setIsAnimating,
  } = useSideBarContext();

  const router = useRouterState();

  const pathName = router.location.pathname;

  const navigate = useNavigate();

  return (
    <>
      <motion.nav
        variants={animationVariants}
        custom={sideBarPosition}
        initial={sideBarOpened ? "opened" : "closed"}
        animate={sideBarOpened ? "opened" : "closed"}
        transition={{ duration: 0.5, ease: "easeInOut" }}
        onAnimationStart={() => setIsAnimating(true)}
        onAnimationComplete={() => setIsAnimating(false)}
        style={{
          position: deferredSideBarPosition,
        }}
        className="h- z-20 flex-shrink-0"
        whileHover={
          sideBarOpened
            ? ""
            : deferredSideBarPosition === "static"
              ? "opened"
              : ""
        }
      >
        <motion.div
          variants={animationVariants}
          custom={sideBarPosition}
          initial={sideBarOpened ? "opened" : "closed"}
          animate={sideBarOpened ? "opened" : "closed"}
          transition={{ duration: 0.5, ease: "easeInOut" }}
          whileHover={
            sideBarOpened
              ? ""
              : deferredSideBarPosition === "static"
                ? "opened"
                : ""
          }
          className="fixed h-full overflow-hidden bg-secondary pt-5 "
        >
          <div className="relative mb-14 flex h-6 w-full items-center justify-end px-2">
            <SideBarCloseButton
              className=" size-4 rounded-full p-0 disabled:opacity-0"
              toggleSideBarButton={toggleSideBarOpened}
              disabled={
                deferredSideBarPosition === "static" ||
                sideBarPosition === "static"
              }
            />
          </div>
          <SideBarHeader
            sideBarOpened={sideBarOpened}
            deferredSideBarPosition={deferredSideBarPosition}
          />
          <div className=" ml-[22px] mt-[35px] flex items-center">
            <p className="font-['Saira_Condensed'] text-[12px] font-[700] leading-[110%] ">
              MENU
            </p>
          </div>
          <ol className={`mt-[14px] w-full space-y-3`}>
            {sidebarLinks.map((link, index) => (
              <React.Fragment key={index}>
                <SideBarButton
                  key={index}
                  svg={link.svg}
                  text={link.text}
                  active={pathName.includes(link.link)}
                  onClick={() => {
                    if (sideBarPosition === "absolute") setSidebarOpened(false);
                    navigate({ to: link.link, search: {} });
                  }}
                />
              </React.Fragment>
            ))}
            <Separator className="m-auto h-[2px] w-10/12" />
            <LogOutButton />
            <SettingsButton />
            <Separator className="m-auto w-10/12" />
            <AccountButton />
            <Separator className="m-auto w-10/12" />
          </ol>
        </motion.div>
        <p
          className="opacity-0
        "
        >
          random string
        </p>
      </motion.nav>
      {sideBarPosition === "absolute" && sideBarOpened ? (
        <div
          className="fixed z-10 h-screen w-screen bg-black bg-opacity-50"
          onClick={toggleSideBarOpened}
        />
      ) : null}
    </>
  );
};

export default SideBar;

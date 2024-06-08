import { AnimatePresence, motion } from "framer-motion";
import React from "react";
import { SideBarPositions } from "~/contexts/SideBarContextProvider";

interface IProps {
  sideBarOpened: boolean;
  deferredSideBarPosition: SideBarPositions;
}

const SideBarHeader: React.FC<IProps> = ({
  sideBarOpened,
  deferredSideBarPosition,
}): JSX.Element => {
  return (
    <div
      className="relative
      h-16 w-full text-center font-['Source_Serif_4'] text-3xl leading-8"
    >
      <AnimatePresence initial={false}>
        {deferredSideBarPosition === "static" && sideBarOpened ? (
          <motion.p
            key={sideBarOpened ? "sidebarOpened" : "sidebarClosed"}
            initial={{ opacity: 0, width: 190 }}
            animate={{ opacity: 1, transition: { delay: 0.5 } }}
            exit={{ opacity: 0 }}
            className="absolute"
          >
            TRACK YOUR LIFE
          </motion.p>
        ) : deferredSideBarPosition === "static" && !sideBarOpened ? (
          <motion.p
            key={sideBarOpened ? "sidebarOpened" : "sidebarClosed"}
            initial={{ opacity: 0, width: 190, height: 33 }}
            animate={{
              opacity: 1,
              transition: {
                delay: 0.5,
              },
            }}
            exit={{ opacity: 0 }}
            className="absolute left-[50%] top-[50%] translate-x-[-50%] translate-y-[-50%] "
          >
            T
          </motion.p>
        ) : (
          <p className="w-[190px]">TRACK YOUR LIFE</p>
        )}
      </AnimatePresence>
    </div>
  );
};

export default SideBarHeader;

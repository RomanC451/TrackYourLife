import { AnimatePresence, motion } from "framer-motion";
import React from "react";

interface IProps {
  sideBarOpened: boolean;
}

const SideBarHeader: React.FC<IProps> = ({ sideBarOpened }): JSX.Element => {
  return (
    <div className="relative mt-[70px] block h-[66px] w-auto flex-shrink-0 text-center font-['Source_Serif_4'] text-[30px] font-[400] leading-[33px]">
      <AnimatePresence initial={false}>
        {sideBarOpened ? (
          <motion.p
            key={sideBarOpened ? "sidebarOpened" : "sidebarClosed"}
            initial={{ opacity: 0, width: 190 }}
            animate={{ opacity: 1, transition: { delay: 0.5 } }}
            exit={{ opacity: 0 }}
            // transition={{ duration: 2, delay: 2 }}
            className="absolute"
          >
            TRACK YOUR LIFE
          </motion.p>
        ) : (
          <motion.p
            key={sideBarOpened ? "sidebarOpened" : "sidebarClosed"}
            initial={{ opacity: 0, width: 190, height: 33 }}
            animate={{ opacity: 1, transition: { delay: 0.5 } }}
            exit={{ opacity: 0 }}
            // transition={{ duration: 2 }}
            className="absolute left-[50%] top-[50%] translate-x-[-50%] translate-y-[-50%] "
          >
            T
          </motion.p>
        )}
      </AnimatePresence>
    </div>
  );
};

export default SideBarHeader;

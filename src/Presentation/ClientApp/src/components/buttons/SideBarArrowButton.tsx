import { motion } from "framer-motion";
import React from "react";
import { SideBarArrowSvg } from "~/assets/sidebar";
import { colors } from "~/constants/tailwindColors";

interface IProps {
  sideBarOpened: boolean;
  toggleSideBarButton: () => void;
}

const SideBarArrowButton: React.FC<IProps> = ({
  sideBarOpened,
  toggleSideBarButton,
}): JSX.Element => {
  return (
    <div className="absolute right-0 top-[159px] z-[200]">
      <div className="mr-[-7.5px] h-[15px] w-[15px]">
        <motion.button
          className={`relative h-full w-full`}
          onClick={toggleSideBarButton}
          initial={{ rotate: sideBarOpened ? 180 : 0 }}
          animate={{ rotate: sideBarOpened ? 0 : 180 }}
          transition={{ duration: 0.5 }}
        >
          <SideBarArrowSvg
            fill={colors.violet}
            className={`absolute left-[-3px] top-[-3px] z-10 rounded-full  backdrop-blur`}
          />
        </motion.button>
      </div>
    </div>
  );
};

export default SideBarArrowButton;

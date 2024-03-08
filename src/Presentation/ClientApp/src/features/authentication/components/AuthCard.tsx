import { motion } from "framer-motion";
import React from "react";
import { ReactComponent as Calendar } from "~/assets/auth/Calendar.svg";
import { ReactComponent as YogaGirlSvg } from "~/assets/auth/YogaGirl.svg";
import { cardAnimations, cardTransitionProps } from "../data/animationsConfig";
import { TAuthModes } from "../data/enums";

type AuthCardProps = {
  authMode: TAuthModes;
};

/**
 * React component for the authentication card.
 * @returns A JSX Element.
 */
const AuthCard: React.FC<AuthCardProps> = ({ authMode }) => {
  return (
    <motion.div
      id="test-id"
      className=" absolute left-0 top-[-1px] z-20 flex h-[650px] w-[390px] flex-col  items-center justify-around rounded bg-[#c5dde2] shadow-2xl "
      initial={cardAnimations[authMode]}
      animate={cardAnimations[authMode]}
      transition={cardTransitionProps}
    >
      <div className="flex flex-col items-center">
        <Calendar className="h-16 w-16" />

        <div className="mt-7 font-bold"> TAKE CONTROL OF YOUR LIFE</div>
      </div>
      <YogaGirlSvg />
    </motion.div>
  );
};

export default AuthCard;

import { AnimationControls, motion } from "framer-motion";
import React from "react";
import { useAuthenticationContext } from "~/contexts/authentication/AuthenticationContextProvider";
import { authModesEnum } from "~/features/authentication/data/authEnums";

const cardPos = [0, 345];

const AuthCard: React.FC = () => {
  const { cardAnimationRef, authMode } = useAuthenticationContext();
  return (
    <motion.div
      id="test-id"
      className={` absolute top-8 left-0 z-10 ml-4 mr-8 flex h-[500px] w-[400px]  flex-col flex-wrap items-center justify-start rounded-2xl bg-[#c5dde2] shadow-2xl `}
      animate={cardAnimationRef}
      initial={{ x: cardPos[authMode === authModesEnum.logIn ? 0 : 1] }}
    >
      <div className=" mt-20 flex h-[100px] w-[85px] flex-col items-center rounded-xl bg-white drop-shadow-2xl">
        <div className="mt-5 h-[13px] w-[27px] rounded-3xl bg-red-400"></div>
        <div className="mt-3 text-3xl">130</div>
      </div>
      <div className="mt-7 font-bold"> TAKE CONTROL OF YOUR LIFE</div>
    </motion.div>
  );
};

export default AuthCard;

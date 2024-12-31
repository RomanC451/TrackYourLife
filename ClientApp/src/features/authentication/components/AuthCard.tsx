import { memo } from "react";
import { AnimatePresence, motion } from "framer-motion";

import Logo from "@/assets/logo.svg?react";
import { cn } from "@/lib/utils";

import { AuthMode, authModes } from "../data/enums";

type AuthCardProps = {
  authMode: AuthMode;
  visible: boolean;
  onAnimationComplete: () => void;
  onAnimationStart: () => void;
};

const cardAnimations = {
  [authModes.logIn]: { x: -1, borderRadius: "8px 0px 0px 8px" },
  [authModes.singUp]: { x: 409, borderRadius: "0px 8px 8px 0px" },
};
const cardTransitionProps = { type: "spring", duration: 1 };

const AuthCard = memo(function ({
  authMode,
  visible,
  onAnimationComplete,
  onAnimationStart,
}: AuthCardProps) {
  return (
    <AnimatePresence>
      {visible ? (
        <motion.div
          id="test-id"
          className={cn(
            "absolute left-0 top-[-1px] z-20 flex h-[650px] w-[390px] flex-col items-center justify-around rounded bg-[#c5dde2] shadow-2xl dark:bg-[#39adc5]",
          )}
          initial={cardAnimations[authMode]}
          animate={cardAnimations[authMode]}
          transition={cardTransitionProps}
          onAnimationStart={onAnimationStart}
          onAnimationComplete={onAnimationComplete}
        >
          <div className="flex flex-col items-center">
            <Logo className="size-40" />

            <div className="dark: mt-7 font-bold text-black">
              TAKE CONTROL OF YOUR LIFE
            </div>
          </div>
          {/* <YogaGirlSvg /> */}
        </motion.div>
      ) : null}
    </AnimatePresence>
  );
});

export default AuthCard;

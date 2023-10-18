import { motion } from "framer-motion";
import React from "react";
import { useAuthenticationContext } from "~/contexts/authentication/AuthenticationContextProvider";
import { authAlerts } from "~/features/authentication/data/alerts";
import { authModes } from "~/features/authentication/data/enums";

import { Alert, Grow } from "@mui/material";

const cardPos = {
  [authModes.logIn]: 30,
  [authModes.singUp]: 370
};
export const transitionProps = { type: "spring", duration: 1 };

/**
 * React component for the authentication card.
 * @returns A JSX Element.
 */
const AuthCard: React.FC = () => {
  const { authMode, alert, setAlert } = useAuthenticationContext();
  return (
    <motion.div
      id="test-id"
      className=" absolute top-8 left-0 flex h-[500px] w-[400px]  flex-col flex-wrap items-center justify-start rounded-2xl bg-[#c5dde2] shadow-2xl z-20"
      initial={{ x: cardPos[authMode] }}
      animate={{ x: cardPos[authMode] }}
      transition={transitionProps}
    >
      <div className=" mt-20 flex h-[100px] w-[85px] flex-col items-center rounded-xl bg-white drop-shadow-2xl">
        <div className="mt-5 h-[13px] w-[27px] rounded-3xl bg-red-400"></div>
        <div className="mt-3 text-3xl">130</div>
      </div>
      <div className="mt-7 font-bold"> TAKE CONTROL OF YOUR LIFE</div>
      <Grow in={alert.message != ""} timeout={1000}>
        <div className="w-[80%] mt-[50px]">
          {alert.message != "" ? (
            <Alert
              severity={alert.severity}
              onClose={() => setAlert(authAlerts.unknown)}
            >
              {alert.message}
            </Alert>
          ) : null}
        </div>
      </Grow>
    </motion.div>
  );
};

export default AuthCard;

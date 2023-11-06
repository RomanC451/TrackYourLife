import { motion } from "framer-motion";
import React, { useState } from "react";
import { Wretch } from "wretch/types";
import { useAuthenticationContext } from "~/contexts/AuthenticationContextProvider";
import { userEndpoints } from "~/data/apiSettings";
import {
  authAlertEnum,
  authModesEnum
} from "~/features/authentication/data/enums";
import { postFetch } from "~/services/postFetch";

import { Alert, Grow } from "@mui/material";

import { useApiContext } from "../../../contexts/ApiContextProvider";

const cardPos = {
  [authModesEnum.logIn]: 30,
  [authModesEnum.singUp]: 370
};
export const transitionProps = { type: "spring", duration: 1 };

/**
 * React component for the authentication card.
 * @returns A JSX Element.
 */
const AuthCard: React.FC = () => {
  const { authMode, alert, setAlert, emailToVerificate } =
    useAuthenticationContext();
  const { defaultApi } = useApiContext();

  const [emailResendingSeconds, setEmailResendingSeconds] = useState(0);

  const resendEmailVerification = () => {
    if (emailToVerificate === "" || emailResendingSeconds > 0) return;
    postResendVerificationEmail();
    setEmailResendingSeconds(5);
    let seconds = 5;

    const interval = setInterval(() => {
      if (seconds > 0) {
        setEmailResendingSeconds(seconds - 1);
        seconds--;
      } else {
        clearInterval(interval);
      }
    }, 1000);

    return () => clearInterval(interval);
  };

  async function postResendVerificationEmail() {
    const data = { email: emailToVerificate };
    postFetch(defaultApi, data, userEndpoints.resendVerificationEmail);
  }

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
      <div className="flex flex-col justify-between items-center flex-grow w-[80%]">
        <Grow in={alert.message != ""} timeout={1000}>
          <div className=" mt-[50px]">
            {alert.message != "" ? (
              <Alert
                severity={alert.severity}
                onClose={() => setAlert(authAlertEnum.unknown)}
              >
                {alert.message}
              </Alert>
            ) : null}
          </div>
        </Grow>
        <button
          type="button"
          className="hover:underline disabled:text-red mb-[50px]"
          style={{
            visibility: emailToVerificate !== "" ? "visible" : "hidden"
          }}
          onClick={resendEmailVerification}
          disabled={emailResendingSeconds > 0}
        >
          Resend verification email.
          {emailResendingSeconds > 0 ? ` (${emailResendingSeconds})` : null}
        </button>
      </div>
    </motion.div>
  );
};

export default AuthCard;

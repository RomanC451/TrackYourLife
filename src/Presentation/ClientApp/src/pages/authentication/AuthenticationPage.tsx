import React, { useEffect, useState } from "react";
import { useAppGeneralStateContext } from "~/contexts/AppGeneralContextProvider";
import {
  AuthenticationContextProvider,
  useAuthenticationContext
} from "~/contexts/authentication/AuthenticationContextProvider";
import { LogInForm, SignUpForm } from "~/features/authentication/";
import { authModesEnum } from "~/features/authentication/data/authEnums";

import AuthCard from "./components/AuthCard";

const LoginPage = () => {
  const { screenSize } = useAppGeneralStateContext();
  const { authMode, cardAnimationRef } = useAuthenticationContext();

  return (
    <AuthenticationContextProvider>
      <div className=" flex min-h-[100vh] items-center justify-center">
        <div className="h-[600px] rounded-2xl shadow-2xl shadow-gray-700">
          <div className="relative top-[50%] left-[50%] flex h-[95%] w-[97%] translate-x-[-50%] translate-y-[-50%] items-center justify-around  rounded-2xl   border-1 border-gray-200 bg-white shadow-xl ">
            {screenSize > 900 ? <AuthCard /> : null}
            {screenSize > 900 || authMode === authModesEnum.singUp ? (
              <SignUpForm />
            ) : null}

            {screenSize > 900 || authModesEnum.logIn ? <LogInForm /> : null}
          </div>
        </div>
      </div>
    </AuthenticationContextProvider>
  );
};

export default LoginPage;

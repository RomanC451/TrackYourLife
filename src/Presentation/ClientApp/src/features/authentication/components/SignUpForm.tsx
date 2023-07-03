import React from "react";
import { AuthenticationButton } from "~/components/buttons";
import { useAuthenticationContext } from "~/contexts/authentication/AuthenticationContextProvider";

import useSignup from "../hooks/useSignup";
import SignUpSlider from "./SignUpSlider";

/**
 * A form component for user sign up.
 * @returns A React component representing the sign up form.
 */
const SignUpForm: React.FC = () => {
  const { switchAuthMode } = useAuthenticationContext();

  const { sliderRef, inputsErrors, changeUserData, handleSignUpRequest } =
    useSignup();

  return (
    <div className="mt-2 ml-4 mr-4 flex h-[500px] w-[370px]  flex-col  flex-wrap items-center justify-start rounded-lg">
      <div className="mt-[5%] w-full text-center text-2xl font-bold">
        SIGN UP
      </div>
      <div className="mt-[3%] w-full text-center text-xs font-bold text-gray-400">
        Take control of your life
      </div>
      <div className="mt-[5%] h-[1px] w-[80%] bg-gray-400"></div>
      <SignUpSlider
        changeUserData={changeUserData}
        inputsErrors={inputsErrors}
        ref={sliderRef}
      />
      <AuthenticationButton
        className="mt-4"
        text="Sign Up"
        onClick={() => handleSignUpRequest()}
      />
      <div className="mt-8">
        <button
          type="button"
          className="hover:underline"
          onClick={switchAuthMode}
        >
          I already have an account.
        </button>
      </div>
    </div>
  );
};

export default SignUpForm;

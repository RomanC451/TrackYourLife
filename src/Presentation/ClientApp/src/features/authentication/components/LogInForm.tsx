import React from "react";
import { AuthenticationButton } from "~/components/buttons";
import { CustomTextField, PasswordField } from "~/components/text_fields";
import { useAuthenticationContext } from "~/contexts/authentication/AuthenticationContextProvider";
import { formStatesEnum } from "~/data/forms";

import { Alert, Grow } from "@mui/material";

import useLogin, { userDataRefsType } from "../hooks/useLogin";

/**
 * React component for the log in form.
 * @returns A JSX Element.
 */
const LogInForm: React.FC = (): JSX.Element => {
  console.log("rerender");

  const { switchAuthMode } = useAuthenticationContext();

  const { formState, setFormState, changeUserData, handleLogInRequest } =
    useLogin();

  const handleInputChange = (
    event: React.ChangeEvent<HTMLInputElement>
  ): void => {
    const { name, value } = event.target;
    changeUserData(name as keyof userDataRefsType, value);
  };

  const handleEnterPressed = (
    event: React.KeyboardEvent<HTMLInputElement>
  ): void => {
    if (event.key === "Enter") {
      // Handle the Enter key press
      handleLogInRequest();
    }
  };

  return (
    <div className="mt-2 ml-4 mr-4 flex h-[500px] w-[380px]  flex-col  flex-wrap items-center justify-start rounded-lg">
      <div className="mt-[5%] mb-[3%] w-full text-center text-2xl font-bold">
        LOG IN
      </div>
      <Grow in={formState != formStatesEnum.unknown} timeout={1000}>
        <div>
          {formState === formStatesEnum.somethingWrong && (
            <Alert
              severity="error"
              onClose={() => setFormState(formStatesEnum.unknown)}
            >
              Something went wrong! Try again later.
            </Alert>
          )}
          {formState === formStatesEnum.bad && (
            <Alert
              severity="error"
              onClose={() => setFormState(formStatesEnum.unknown)}
            >
              Wrong credentials! Try again.
            </Alert>
          )}
          {formState === formStatesEnum.good && (
            <Alert
              severity="success"
              onClose={() => setFormState(formStatesEnum.unknown)}
            >
              Log in successfully!
            </Alert>
          )}
        </div>
      </Grow>
      <div className="mt-[3%] w-full text-center text-xs font-bold text-gray-400">
        Take control of your life
      </div>
      <div className="mt-[5%] h-[1px] w-[80%] bg-gray-400"></div>
      <CustomTextField
        label="Email"
        name="email"
        className="mt-8 w-[80%]"
        onChange={handleInputChange}
      />
      <PasswordField
        label="Password"
        name="password"
        className="mt-8 w-[80%]"
        onChange={handleInputChange}
        onKeyPress={handleEnterPressed}
      />
      <AuthenticationButton
        className="mt-12"
        text="Log In"
        onClick={handleLogInRequest}
      />
      <button
        type="button"
        className="mt-8 hover:underline"
        onClick={switchAuthMode}
      >
        I don't have an account.
      </button>
    </div>
  );
};

export default LogInForm;

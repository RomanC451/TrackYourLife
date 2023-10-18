import React from "react";
import { CustomTextField, PasswordField } from "~/components/textFields";

import useLogin from "../hooks/useLogin";

/**
 * React component for the log in form.
 * @returns A JSX Element.
 */
const LogInForm: React.FC = (): JSX.Element => {
  const { register, onSubmit, errors, switchToSignUp, isAnimating } =
    useLogin();

  return (
    <div className="flex h-[480px] w-[380px] flex-col  flex-wrap items-center justify-start rounded-lg">
      <div className="font-bold grid place-items-center gap-[10px]">
        <span className="text-2xl">LOG IN</span>
        <span className="text-xs">Take control of your life</span>
      </div>

      <div className="mt-[4px] h-[1px] w-[80%] bg-slate-400"></div>
      <form className="w-[80%] flex-grow flex flex-col" onSubmit={onSubmit()}>
        <section className="w-full grid gap-[10px] pt-[10px]">
          <CustomTextField
            inputProps={register("email")}
            label="Email"
            errorMessage={errors.email?.message}
          />
          <PasswordField
            inputProps={register("password")}
            label="Password"
            errorMessage={errors.password?.message}
          />
        </section>
        <div className="flex flex-col justify-between flex-grow">
          <button
            type="submit"
            className={`mt-12 w-full bg-gray h-[50px] rounded-3xl text-white shadow-lg hover:shadow-2xl disabled:bg-slate-300`}
          >
            Log In
          </button>
          <button
            type="button"
            className="hover:underline disabled:text-red"
            onClick={switchToSignUp}
            disabled={isAnimating}
          >
            I don't have an account.
          </button>
        </div>
      </form>
    </div>
  );
};

export default LogInForm;

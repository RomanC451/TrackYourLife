import React, { useRef } from "react";
import { Slider, SliderPropsRef } from "~/animations";
import { CustomTextField, PasswordField } from "~/components/textFields";

import useSignup from "../hooks/useSignup";

/**
 * SignUpForm component for user registration.
 * @returns React component
 */
const SignUpForm: React.FC = () => {
  const { register, onSubmit, errors, swithcToLogIn, isAnimating } =
    useSignup();

  const sliderRef = useRef<SliderPropsRef>(null);

  const goTo = () => {
    if (Object.keys(errors).length === 0) return;

    for (let pageIndex = 0; pageIndex < formPages.length; pageIndex++) {
      for (const child of formPages[pageIndex].props.children) {
        if (errors.hasOwnProperty(child.props.inputProps?.name)) {
          sliderRef.current?.goToPage(pageIndex);
          return;
        }
      }
    }
  };

  // !TODO: To memoize formPages
  const formPages = [
    <section className="w-full grid gap-[10px] pt-[10px] mb-[10px] h-full">
      <CustomTextField
        inputProps={register("email")}
        label="Email"
        defaultValue="catalin.roman451@gmail.com"
        errorMessage={errors.email?.message}
      />
      <PasswordField
        inputProps={register("password")}
        label="Password"
        defaultValue="Waryor.001"
        errorMessage={errors.password?.message}
      />
      <PasswordField
        inputProps={register("confirmPassword")}
        label="Confirm password"
        defaultValue="Waryor.001"
        errorMessage={errors.confirmPassword?.message}
      />
    </section>,
    <section className="w-full grid gap-[10px] pt-[10px]">
      <CustomTextField
        inputProps={register("firstName")}
        label="First Name"
        defaultValue="Catalin"
        errorMessage={errors.firstName?.message}
      />
      <CustomTextField
        inputProps={register("lastName")}
        label="Last Name"
        defaultValue="Roman"
        errorMessage={errors.lastName?.message}
      />
    </section>
  ];

  return (
    <div className="flex h-[480px] w-[380px] flex-col  flex-wrap items-center justify-start rounded-lg">
      <div className="font-bold grid place-items-center gap-[10px]">
        <span className="text-2xl">SIGN UP</span>
        <span className="text-xs">Take control of your life</span>
      </div>
      <div className="mt-[4px] h-[1px] w-[80%] bg-slate-400"></div>
      <form className=" w-[80%] flex flex-col flex-grow" onSubmit={onSubmit()}>
        <Slider ref={sliderRef} pages={formPages} />

        <div className="flex flex-col justify-between flex-grow">
          <button
            type="submit"
            className={`w-full bg-gray  h-[50px] rounded-3xl text-white shadow-lg hover:shadow-2xl`}
            onClick={goTo}
          >
            Sign Up
          </button>
          <button
            type="button"
            className="hover:underline disabled:text-red"
            onClick={swithcToLogIn}
            disabled={isAnimating}
          >
            I already have an account.
          </button>
        </div>
      </form>
    </div>
  );
};

export default SignUpForm;

import React, { forwardRef, useImperativeHandle, useRef } from "react";
import { Slider } from "~/animations/Slider";
import { SliderRef } from "~/animations/Slider/Slider";
import { CustomTextField, PasswordField } from "~/components/text_fields";
import { useAuthenticationContext } from "~/contexts/authentication/AuthenticationContextProvider";
import {
  inputsErrorsType,
  userDataRefsType
} from "~/features/authentication/hooks/useSignup";
import { ObjectStateType } from "~/hooks/useObjectState";

interface SignUpSliderProps {
  changeUserData: (
    name: keyof userDataRefsType,
    value: userDataRefsType[keyof userDataRefsType]
  ) => void;
  inputsErrors: ObjectStateType<inputsErrorsType>;
  ref: React.Ref<SignUpSliderRef>;
}

export interface SignUpSliderRef {
  goToErrorPageContainingLabel: (labelName: string) => void;
}

const SignUpSlider: React.FC<SignUpSliderProps> = forwardRef<
  SignUpSliderRef,
  SignUpSliderProps
>(({ changeUserData, inputsErrors }, ref): JSX.Element => {
  const sliderRef = useRef<SliderRef>(null);

  const sliderComponents = [
    <div className="flex w-full flex-col items-center">
      <CustomTextField
        label="Email"
        name="email"
        className="mt-4  w-[80%]"
        onChange={handleInputChange}
        errorMessage={inputsErrors.email}
      />
      <PasswordField
        label="Password"
        name="password"
        className="mt-4 w-[80%]"
        onChange={handleInputChange}
        errorMessage={inputsErrors.password}
      />
      <PasswordField
        label="Confirm password"
        name="confirmPassword"
        className="mt-4 w-[80%]"
        onChange={handleInputChange}
        errorMessage={inputsErrors.confirmPassword}
      />
    </div>,
    <div className="flex w-full flex-col items-center">
      <CustomTextField
        label="First Name"
        name="firstName"
        className="mt-4  w-[80%]"
        onChange={handleInputChange}
        errorMessage={inputsErrors.firstName}
      />
      <CustomTextField
        label="Last Name"
        name="lastName"
        className="mt-4 w-[80%]"
        onChange={handleInputChange}
        errorMessage={inputsErrors.lastName}
      />
    </div>
  ];

  function handleInputChange(event: React.ChangeEvent<HTMLInputElement>): void {
    const { name, value } = event.target;
    changeUserData(name as keyof userDataRefsType, value);
  }

  function goToErrorPageContainingLabel(labelName: string) {
    console.log("goToErrorPage");
    sliderComponents.forEach((page, pageIndex) => {
      page.props.children.forEach((child: JSX.Element) => {
        if (child.props.name === labelName) {
          console.log("if reached");
          console.log(pageIndex);
          sliderRef.current?.goToSliderComponent(pageIndex);
          return;
        }
      });
    });
  }

  useImperativeHandle(ref, () => ({
    goToErrorPageContainingLabel
  }));

  return (
    <Slider
      className="h-[260px] w-[100%]"
      width={358}
      ref={sliderRef}
      sliderComponents={sliderComponents}
    />
  );
});

export default SignUpSlider;

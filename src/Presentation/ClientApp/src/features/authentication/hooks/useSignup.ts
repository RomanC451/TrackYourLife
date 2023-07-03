import { useRef } from "react";
import { useNavigate } from "react-router-dom";
import { useApiContext } from "~/contexts/ApiContextProvider";
import { useAuthenticationContext } from "~/contexts/authentication/AuthenticationContextProvider";
import { userEndpoints } from "~/data/apiSettings";
import { sliderComponentPosition } from "~/data/positionEnums";
import { SignUpSliderRef } from "~/features/authentication/components/SignUpSlider";
import { getRegisterInputAndError } from "~/features/authentication/data/errors";
import { useObjectRef, useObjectState } from "~/hooks";
import { postFetch } from "~/services/postFetch";

export type inputsErrorsType = {
  email: string;
  password: string;
  confirmPassword: string;
  lastName: string;
  firstName: string;
};

export type userDataRefsType = {
  email: string;
  password: string;
  confirmPassword: string;
  lastName: string;
  firstName: string;
};

type badRequestErrorType = {
  detail: string;
  errors: null;
  status: number;
  title: string;
  type: string;
};

const sliderPages = [
  ["email", "password", "confirmPassword"],
  ["lastName", "firstName"]
];

const useSignup = () => {
  const navigate = useNavigate();

  const { defaultApi, setJwtToken } = useApiContext();

  const isRequestPending = useRef(false);

  const sliderRef = useRef<SignUpSliderRef>(null);

  const [userDataRefs, changeUserData] = useObjectRef({
    email: "",
    password: "",
    confirmPassword: "",
    lastName: "",
    firstName: ""
  });

  const [inputsErrors, changeInputsErrors] = useObjectState<inputsErrorsType>({
    email: "",
    password: "",
    confirmPassword: "",
    lastName: "",
    firstName: ""
  });

  function handleSignUpRequest() {
    const wrongField = Object.keys(inputsErrors).find(
      (key) => inputsErrors[key as keyof inputsErrorsType] !== ""
    );

    if (wrongField) {
      sliderRef.current?.goToErrorPageContainingLabel(wrongField);
    }

    if (isRequestPending.current) {
      return;
    }

    isRequestPending.current = true;

    postFetch(
      defaultApi,
      userDataRefs.current,
      userEndpoints.register,
      setJwtToken
    )
      .badRequest((error) => {
        try {
          var errorJson = JSON.parse(error.message) as badRequestErrorType;
          console.log(errorJson);
          const [inputName, errorMessage] = getRegisterInputAndError(
            errorJson.type
          );
          changeInputsErrors(inputName, errorMessage, true);
          console.log("inputChanged");
          sliderRef.current?.goToErrorPageContainingLabel(inputName);
        } catch (e) {}
      })
      .json((data) => {
        navigate("/home");
      })
      .finally(() => {
        isRequestPending.current = false;
      });
  }

  return {
    sliderRef,
    inputsErrors,
    changeUserData,
    handleSignUpRequest
  };
};

export default useSignup;

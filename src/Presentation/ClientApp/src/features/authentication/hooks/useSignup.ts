import { useForm } from "react-hook-form";
import { WretchError } from "wretch/types";
import { useApiContext } from "~/contexts/ApiContextProvider";
import { useAuthenticationContext } from "~/contexts/authentication/AuthenticationContextProvider";
import { userEndpoints } from "~/data/apiSettings";
import { authAlerts } from "~/features/authentication/data/alerts";
import { authErrors } from "~/features/authentication/data/errors";
import {
  signUpSchema,
  TSignUpSchema
} from "~/features/authentication/data/schemas";
import { postFetch } from "~/services/postFetch";

import { zodResolver } from "@hookform/resolvers/zod";

/**
 * Custom hook for handling user sign up functionality.
 * @returns An object containing the following properties:
 * - register: A function to register form inputs with react-hook-form.
 * - onSubmit: A function to handle form submission.
 * - errors: An object containing any validation errors.
 * - switchAuthMode: A function to switch between authentication modes.
 * - isAnimating: A boolean indicating whether the authentication form is currently animating.
 */
const useSignup = () => {
  const { switchAuthMode, isAnimating, setAlert } = useAuthenticationContext();

  const { defaultApi, setJwtToken } = useApiContext();

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors }
  } = useForm<TSignUpSchema>({
    resolver: zodResolver(signUpSchema),
    shouldFocusError: false,
    defaultValues: {
      email: "catalin.roman451@gmail.com",
      password: "Waryor.001",
      confirmPassword: "Waryor.001",
      firstName: "Catalin",
      lastName: "Roman"
    }
  });

  const onSubmit = () => {
    return handleSubmit(postSignUpRequest);
  };

  async function postSignUpRequest(data: TSignUpSchema) {
    console.log(data);
    postFetch(defaultApi, data, userEndpoints.register, setJwtToken)
      .badRequest((error: WretchError) => {
        const { type: errorType, detail: errorDetail } = JSON.parse(
          error.message
        );

        console.log(errorDetail);

        switch (errorType) {
          case authErrors.EmailNotUnique:
            setError("email", { type: "manual", message: errorDetail });
            break;

          default:
            setAlert(authAlerts.somethingWrong);
        }
      })
      .json(() => {
        setAlert(authAlerts.successfulRegister);
        switchAuthMode();
      })
      .catch(() => {
        setAlert(authAlerts.somethingWrong);
      });
  }

  return {
    register,
    onSubmit,
    errors,
    swithcToLogIn: switchAuthMode,
    isAnimating
  };
};

export default useSignup;

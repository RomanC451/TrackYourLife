import { FieldErrors, useForm, UseFormRegister } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { WretchError } from "wretch/resolver";
import { useApiContext } from "~/contexts/ApiContextProvider";
import { useAuthenticationContext } from "~/contexts/authentication/AuthenticationContextProvider";
import { userEndpoints } from "~/data/apiSettings";
import { authAlerts } from "~/features/authentication/data/alerts";
import { authErrors } from "~/features/authentication/data/errors";
import {
  logInSchema,
  TLogInSchema
} from "~/features/authentication/data/schemas";
import { postFetch } from "~/services/postFetch";

import { zodResolver } from "@hookform/resolvers/zod";

/**
 * Custom hook for handling login functionality.
 * @returns An object containing the following properties:
 * - register: A function to register input fields with React Hook Form.
 * - onSubmit: A function to handle form submission.
 * - errors: An object containing validation errors for the form.
 * - switchToSignUp: A function to switch to the sign up page.
 * - isAnimating: A boolean indicating whether the authentication context is currently animating.
 */
interface useLoginRetrun {
  register: UseFormRegister<TLogInSchema>;
  onSubmit: () => (e?: React.BaseSyntheticEvent | undefined) => Promise<void>;
  errors: FieldErrors<TLogInSchema>;
  switchToSignUp: () => void;
  isAnimating: boolean;
}

const useLogin = (): useLoginRetrun => {
  const navigate = useNavigate();

  const { isAnimating, switchAuthMode, setAlert } = useAuthenticationContext();

  const { defaultApi, setJwtToken } = useApiContext();

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors }
  } = useForm<TLogInSchema>({
    resolver: zodResolver(logInSchema),
    defaultValues: {
      email: "catalin.roman451@gmail.com",
      password: "Waryor.001"
    }
  });

  const onSubmit = () => {
    return handleSubmit(postLogInRequest, () => {
      console.log("errors");
    });
  };

  async function postLogInRequest(data: TLogInSchema) {
    postFetch(defaultApi, data, userEndpoints.login, setJwtToken)
      .badRequest((error: WretchError) => {
        try {
          var { type: errorType, detail: errorDetail } = JSON.parse(
            error.message
          );
          switch (errorType) {
            case authErrors.InvalidCredentials:
              setAlert(authAlerts.wrongCredentials);
              break;
            case authErrors.EmailNotVerified:
              setError("email", { type: "manual", message: errorDetail });
              break;
            default:
              setAlert(authAlerts.somethingWrong);
          }
        } catch (e) {
          setAlert(authAlerts.somethingWrong);
        }
      })
      .json((data: { jwtToken: string }) => {
        setAlert(authAlerts.good);
        setJwtToken(data.jwtToken);
        navigate("/home");
      })
      .catch(() => {
        setAlert(authAlerts.somethingWrong);
      });
  }

  return {
    register,
    onSubmit,
    errors,
    switchToSignUp: switchAuthMode,
    isAnimating
  };
};

export default useLogin;

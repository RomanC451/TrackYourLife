import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { useNavigate, useSearch } from "@tanstack/react-router";
import { useState } from "react";
import { UseFormReturn, useForm } from "react-hook-form";
import { toast } from "sonner";
import { WretchError } from "wretch/resolver";
import { useApiContext } from "~/contexts/ApiContextProvider";
import { getErrorObject, toastDefaultServerError } from "~/data/apiSettings";
import { useApiRequests } from "~/hooks/useApiRequests";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { AuthSearchSchema } from "~/routes/auth";
import { authErrors } from "../data/errors";
import { TLogInSchema, logInSchema } from "../data/schemas";
import { postLogInRequest } from "../requests/postLogInRequest";
import {
  TResendVerificationEmailData,
  postResendVerificationEmail,
} from "../requests/postResendVerificationEmail";
import { useLogOutMutation } from "./useLogOutMutation";

interface useLoginRetrun {
  form: UseFormReturn<TLogInSchema, unknown, TLogInSchema>;
  onSubmit: (data: TLogInSchema) => void;
  isSubmitting: boolean;
}

/**
 * Custom hook for handling login functionality.
 * @returns An object containing the form and onSubmit function.
 */
const useLogIn = (): useLoginRetrun => {
  const navigate = useNavigate();

  const search: AuthSearchSchema = useSearch({
    from: "/auth",
  });

  const { setJwtToken } = useApiContext();

  const { fetchRequest } = useApiRequests();

  const [emailToVerificate, setEmailToVerificate] = useState<string>("");

  const form = useForm<TLogInSchema>({
    resolver: zodResolver(logInSchema),
    defaultValues: {
      email: "catalin.roman451@gmail.com",
      password: "Waryor.001",
    },
  });

  const { logOutMutation } = useLogOutMutation();

  /**
   * Represents a login mutation.
   * @typeParam TLogInSchema - The type of the login schema.
   * @param variables - The variables for the login mutation.
   * @returns A promise that resolves to the result of the login mutation.
   */
  const loginMutation = useMutation({
    onMutate: () => {
      logOutMutation.mutate();
    },
    mutationFn: (variables: TLogInSchema) => {
      return postLogInRequest({
        fetchRequest,
        data: variables,
      });
    },
    onError: (error: WretchError, variables) => {
      const errorObject = getErrorObject(error);
      if (!errorObject) {
        toastDefaultServerError();
        setEmailToVerificate("");
        return;
      }

      console.log(errorObject);
      switch (errorObject.type) {
        case authErrors.InvalidCredentials:
          toast.error("Invalid credentials", {
            description: " Please check your email and password.",
          });
          setEmailToVerificate("");
          break;
        case authErrors.EmailNotVerified:
          console.log("email not verified");
          setEmailToVerificate(variables.email);
          form.setError("email", {
            type: "custom",
            message: errorObject.detail,
          });
          toast.warning(
            "Email not verified. Please check your email and verify it.",
            {
              action: {
                label: "Resend email",
                onClick: () => resendEmailVerification(),
              },
            },
          );

          break;
        default:
          toastDefaultServerError();
          setEmailToVerificate("");
      }
    },
    onSuccess: (resp) => {
      setJwtToken(resp.jwtToken);
      navigate({
        to: search?.redirect ?? "/home",
        search: {},
        replace: search?.redirect ? true : false,
      });
    },
  });

  /**
   * Mutation hook for resending email verification.
   *
   * @remarks
   * This hook is used to resend the verification email to the user.
   * It handles the mutation function, error handling, and success handling.
   *
   * @returns The resendEmailVerificationMutation object.
   */
  const resendEmailVerificationMutation = useMutation({
    mutationFn: (variables: TResendVerificationEmailData) => {
      return postResendVerificationEmail(fetchRequest, variables);
    },
    onError: (error: WretchError) => {
      const errorObject = getErrorObject(error);
      if (!errorObject) {
        toastDefaultServerError();
        return;
      }

      switch (errorObject.status) {
        case 404:
          toast("Email not found", {
            description: "Please register first and try again.",
          });
          break;
        default:
          toastDefaultServerError();
      }
    },

    onSuccess: () => {
      toast("Verification email sent", {
        description: "Please check your email and verify it.",
      });
    },
  });

  /**
   * Resends the email verification for the specified email.
   *
   * @remarks
   * This function sends a request to the server to resend the email verification for the provided email address.
   * If the email address is empty, the function will return early without making the request.
   */
  const resendEmailVerification = () => {
    if (emailToVerificate === "") return;

    resendEmailVerificationMutation.mutate({
      email: emailToVerificate,
    });
  };

  const loadingState = useDelayedLoading(500, loginMutation.isPending);

  return {
    form,
    onSubmit: (data: TLogInSchema) => loginMutation.mutate(data),
    isSubmitting: loadingState.isLoading,
  };
};

export default useLogIn;

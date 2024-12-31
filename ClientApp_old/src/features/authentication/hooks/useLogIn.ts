import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { useNavigate, useSearch } from "@tanstack/react-router";
import globalAxios from "axios";
import { StatusCodes } from "http-status-codes";
import { useState } from "react";
import { UseFormReturn, useForm } from "react-hook-form";
import { toast } from "sonner";
import { ApiError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { AuthSearchSchema } from "~/routes/auth";
import { TLogInSchema, logInSchema } from "../data/schemas";

import { AuthApi, LogInUserRequest } from "~/services/openapi";
import { handleApiError } from "~/utils/handleApiError";
import { apiErrors } from "../data/errors";

interface useLoginReturn {
  form: UseFormReturn<TLogInSchema, unknown, undefined>;
  onSubmit: (data: TLogInSchema) => void;
  isSubmitting: boolean;
}

const authApi = new AuthApi();

/**
 * Custom hook for handling login functionality.
 * @returns An object containing the form and onSubmit function.
 */
const useLogIn = (): useLoginReturn => {
  const navigate = useNavigate();

  const search: AuthSearchSchema = useSearch({
    from: "/auth",
  });

  const [emailForVerification, setEmailForVerification] = useState<string>("");

  const form = useForm<TLogInSchema>({
    resolver: zodResolver(logInSchema),
    defaultValues: {
      email: "catalin.roman451@gmail.com",
      password: "Waryor.001",
    },
  });

  // const { logOutMutation } = useLogOutMutation();

  /**
   * Represents a login mutation.
   * @typeParam TLogInSchema - The type of the login schema.
   * @param variables - The variables for the login mutation.
   * @returns A promise that resolves to the result of the login mutation.
   */
  const loginMutation = useMutation({
    mutationFn: async (variables: LogInUserRequest) => {
      // if (globalAxios.defaults.headers.common["Authorization"])
      //   await logOutMutation.mutateAsync();

      return authApi.loginUser(variables);
    },
    onError: (error: ApiError, variables) =>
      handleApiError({
        error,
        errorHandlers: {
          [StatusCodes.BAD_REQUEST]: {
            default: (errorData) => {
              toast.error("Failed", { description: errorData.detail });
            },
            [apiErrors.User.InvalidCredentials]: () => {
              toast.error("Invalid credentials", {
                description: " Please check your email and password.",
              });
            },
            [apiErrors.Email.NotVerified]: (errorData) => {
              setEmailForVerification(variables.email ?? "");
              form.setError("email", {
                type: "custom",
                message: errorData.detail,
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
            },
          },
        },
        defaultHandler: () => setEmailForVerification(""),
      }),

    onSuccess: (resp) => {
      globalAxios.defaults.headers.common["Authorization"] =
        `Bearer ${resp.data.tokenValue}`;

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
    mutationFn: (variables: { email: string }) =>
      authApi.resendEmailVerification(variables.email),
    onError: (error: ApiError) =>
      handleApiError({
        error,
        errorHandlers: {
          [StatusCodes.NOT_FOUND]: {
            default: () => {
              toast.warning("Email not found", {
                description: "Please register first and try again.",
              });
            },
          },
        },
      }),

    onSuccess: () => {
      toast("Verification email sent", {
        description: "Please check your email and verify it.",
      });
    },
  });

  /**
   * Re sends the email verification for the specified email.
   *
   * @remarks
   * This function sends a request to the server to resend the email verification for the provided email address.
   * If the email address is empty, the function will return early without making the request.
   */
  const resendEmailVerification = () => {
    if (emailForVerification === "") return;

    resendEmailVerificationMutation.mutate({
      email: emailForVerification,
    });
  };

  const loadingState = useDelayedLoading(loginMutation.isPending);

  return {
    form,
    onSubmit: (data: TLogInSchema) => loginMutation.mutate(data),
    isSubmitting: loadingState.isLoading,
  };
};

export default useLogIn;

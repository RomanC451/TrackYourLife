import { useState } from "react";
import { useNavigate, useSearch } from "@tanstack/react-router";
import globalAxios from "axios";
import { StatusCodes } from "http-status-codes";
import { ErrorOption } from "react-hook-form";
import { toast } from "sonner";

import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";
import { useCustomMutation } from "@/hooks/useCustomMutation";
import { getSafeRedirectUrl } from "@/lib/urlSanitizer";
import { queryClient } from "@/queryClient";
import { AuthSearchSchema } from "@/routes/auth";
import { AuthApi, LoginUserRequest } from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";

import { apiAuthErrors } from "../data/errors";
import { LogInSchema } from "../data/schemas";

const authApi = new AuthApi();

export default function useLoginMutation(
  setError: (
    name: keyof LogInSchema,
    error: ErrorOption,
    options?: {
      shouldFocus: boolean;
    },
  ) => void,
  resendEmailVerification: (email: string) => void,
) {
  const navigate = useNavigate();

  const search: AuthSearchSchema = useSearch({
    from: "/auth",
  });

  const { setQueryEnabled } = useAuthenticationContext();

  const [emailForVerification, setEmailForVerification] = useState<string>("");

  const loginMutation = useCustomMutation({
    mutationFn: async (variables: LoginUserRequest) => {
      return authApi.loginUser(variables);
    },
    onError: (error, variables) =>
      handleApiError({
        error,
        errorHandlers: {
          [StatusCodes.BAD_REQUEST]: {
            default: (errorData) => {
              toast.error("Failed", { description: errorData.detail });
            },
            [apiAuthErrors.User.InvalidCredentials]: () => {
              toast.error("Invalid credentials", {
                description: " Please check your email and password.",
              });
            },
            [apiAuthErrors.Email.NotVerified]: (errorData) => {
              setEmailForVerification(variables.email ?? "");
              setError("email", {
                type: "custom",
                message: errorData.detail,
              });
              toast.warning(
                "Email not verified. Please check your email and verify it.",
                {
                  action: {
                    label: "Resend email",
                    onClick: () =>
                      resendEmailVerification(emailForVerification),
                  },
                },
              );
            },
          },
        },
        validationErrorsHandler: () =>
          toast.error("Invalid credentials", {
            description: " Please check your email and password.",
          }),
        defaultHandler: () => setEmailForVerification(""),
      }),

    onSuccess: (resp) => {
      globalAxios.defaults.headers.common["Authorization"] =
        `Bearer ${resp.data.tokenValue}`;

      queryClient.invalidateQueries({ queryKey: ["userData"] });

      setQueryEnabled(true);

      queryClient.resetQueries();

      setTimeout(() => {
        const safeRedirect = getSafeRedirectUrl(search?.redirect, "/home");
        navigate({
          to: safeRedirect,
          search: {},
          replace: !!search?.redirect,
        });
      }, 100);
    },
  });

  return loginMutation;
}

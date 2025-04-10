import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { useNavigate, useSearch } from "@tanstack/react-router";
import globalAxios from "axios";
import { StatusCodes } from "http-status-codes";
import { ErrorOption } from "react-hook-form";
import { toast } from "sonner";

import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";
import useDelayedLoading from "@/hooks/useDelayedLoading";
import { queryClient } from "@/queryClient";
import { AuthSearchSchema } from "@/routes/auth";
import { AuthApi, LogInUserRequest } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
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
        defaultHandler: () => setEmailForVerification(""),
      }),

    onSuccess: (resp) => {
      globalAxios.defaults.headers.common["Authorization"] =
        `Bearer ${resp.data.tokenValue}`;

      queryClient.invalidateQueries({ queryKey: ["userData"] });

      setQueryEnabled(true);

      setTimeout(() => {
        navigate({
          to: search?.redirect ?? "/home",
          search: {},
          replace: !!search?.redirect,
        });
      }, 100);
    },
  });

  const isPending = useDelayedLoading(loginMutation.isPending);

  return { loginMutation, isPending };
}

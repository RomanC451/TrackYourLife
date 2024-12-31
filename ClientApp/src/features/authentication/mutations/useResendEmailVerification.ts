import { useMutation } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";
import { toast } from "sonner";

import { AuthApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

const authApi = new AuthApi();

function useResendEmailVerification() {
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

  return { resendEmailVerificationMutation };
}

export default useResendEmailVerification;

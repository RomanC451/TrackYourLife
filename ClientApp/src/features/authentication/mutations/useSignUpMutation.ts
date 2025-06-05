import { useMutation } from "@tanstack/react-query";
import { ErrorOption } from "react-hook-form";
import { toast } from "sonner";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { AuthApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

import { SignUpSchema } from "../data/schemas";

const authApi = new AuthApi();

const useSignUpMutation = (
  setError: (
    name: keyof SignUpSchema,
    error: ErrorOption,
    options?: {
      shouldFocus: boolean;
    },
  ) => void,
  onFieldError: () => void,
) => {
  const signUpMutation = useMutation({
    mutationFn: (variables: SignUpSchema) => authApi.registerUser(variables),
    onError: (error: ApiError) =>
      handleApiError({
        error,
        validationErrorsHandler: (name, error, options) => {
          setError(name, error, options);
          onFieldError();
        },
      }),

    onSuccess: () => {
      toast.success("Account created successfully.", {
        description: "Please check your email to verify your account.",
      });
    },
  });

  const isPending = useDelayedLoading(signUpMutation.isPending);

  return { signUpMutation, isPending };
};

export default useSignUpMutation;

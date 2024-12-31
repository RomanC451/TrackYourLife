import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { ApiError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { AuthApi } from "~/services/openapi";
import { TSignUpSchema, signUpSchema } from "../data/schemas";

import { StatusCodes } from "http-status-codes";
import { handleApiError } from "~/utils/handleApiError";
import { apiErrors } from "../data/errors";

const authApi = new AuthApi();

/**
 * Custom hook for handling signup functionality.
 * @returns An object containing the form and onSubmit function.
 */
const useSignup = () => {
  const form = useForm<TSignUpSchema>({
    resolver: zodResolver(signUpSchema),
    shouldFocusError: false,
    defaultValues: {
      email: "catalin.roman451@gmail.com",
      password: "Waryor.001",
      confirmPassword: "Waryor.001",
      firstName: "Catalin",
      lastName: "Roman",
    },
    reValidateMode: "onChange",
    resetOptions: {
      keepErrors: true,
      keepDirty: true,
      keepDirtyValues: true,
    },
  });

  const signUpMutation = useMutation({
    mutationFn: (variables: TSignUpSchema) => authApi.registerUser(variables),
    onError: (error: ApiError) =>
      handleApiError({
        error,
        errorHandlers: {
          [StatusCodes.BAD_REQUEST]: {
            [apiErrors.Email.AlreadyUsed]: (errorData) => {
              form.setError("email", {
                type: "custom",
                message: errorData.detail,
              });
            },
          },
        },
      }),

    onSuccess: () => {
      toast.success("Account created successfully.", {
        description: "Please check your email to verify your account.",
      });
    },
  });

  const loadingState = useDelayedLoading(signUpMutation.isPending);

  return {
    form,
    onSubmit: (data: TSignUpSchema) => signUpMutation.mutate(data),
    isSubmitting: loadingState.isLoading,
  };
};

export default useSignup;

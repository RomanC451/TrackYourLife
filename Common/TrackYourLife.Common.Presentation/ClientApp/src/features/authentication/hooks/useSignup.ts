import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { ApiError, toastDefaultServerError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { AuthenticationApi } from "~/services/openapi";
import { TSignUpSchema, signUpSchema } from "../data/schemas";

import { apiErrors } from "../data/errors";

const authApi = new AuthenticationApi();

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
    mutationFn: (variables: TSignUpSchema) => authApi.register(variables),
    onError: (error: ApiError) => {
      console.log(error);

      const errorObject = error.response?.data;

      console.log(errorObject);
      if (!errorObject) {
        toastDefaultServerError();
        return;
      }

      switch (errorObject.type) {
        case apiErrors.Email.AlreadyUsed:
          form.setError("email", {
            type: "custom",
            message: errorObject.detail,
          });
          break;

        default:
          toastDefaultServerError();
      }
    },
    onSuccess: () => {
      toast.success("Account created successfully.", {
        description: "Please check your email to verify your account.",
      });
    },
  });

  const loadingState = useDelayedLoading(500, signUpMutation.isPending);

  return {
    form,
    onSubmit: (data: TSignUpSchema) => signUpMutation.mutate(data),
    isSubmitting: loadingState.isLoading,
  };
};

export default useSignup;

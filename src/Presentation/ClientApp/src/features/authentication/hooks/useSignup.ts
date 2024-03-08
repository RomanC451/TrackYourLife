import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { WretchError } from "wretch";
import { getErrorObject, toastDefaultServerError } from "~/data/apiSettings";
import { postSignUpRequest } from "~/features/authentication/requests/postSignUpRequest";
import { useApiRequests } from "~/hooks/useApiRequests";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { authErrors } from "../data/errors";
import { TSignUpSchema, signUpSchema } from "../data/schemas";

/**
 * Custom hook for handling signup functionality.
 * @returns An object containing the form and onSubmit function.
 */
const useSignup = () => {
  const { fetchRequest } = useApiRequests();

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
    mutationFn: (variables: TSignUpSchema) =>
      postSignUpRequest({
        fetchRequest: fetchRequest,
        data: variables,
      }),
    onError: (error: WretchError) => {
      const errorObject = getErrorObject(error);
      if (!errorObject) {
        toastDefaultServerError();
        return;
      }
      console.log(error);
      switch (errorObject.type) {
        case authErrors.EmailNotUnique:
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

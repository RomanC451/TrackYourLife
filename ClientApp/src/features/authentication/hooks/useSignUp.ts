import { zodResolver } from "@hookform/resolvers/zod";
import { useNavigate } from "@tanstack/react-router";
import { useForm } from "react-hook-form";

import { authModes } from "../data/enums";
import { signUpSchema, SignUpSchema } from "../data/schemas";
import useSignUpMutation from "../mutations/useSignUpMutation";

const defaultValues = import.meta.env.DEV
  ? {
      email: import.meta.env.VITE_DEV_EMAIL || "",
      password: import.meta.env.VITE_DEV_PASSWORD || "",
      confirmPassword: import.meta.env.VITE_DEV_PASSWORD || "",
      firstName: import.meta.env.VITE_DEV_FIRST_NAME || "",
      lastName: import.meta.env.VITE_DEV_LAST_NAME || "",
    }
  : {
      email: "",
      password: "",
      confirmPassword: "",
      firstName: "",
      lastName: "",
    };

const useSignUp = (onFieldError: () => void) => {
  const navigate = useNavigate();

  const form = useForm<SignUpSchema>({
    resolver: zodResolver(signUpSchema),
    shouldFocusError: false,
    defaultValues: defaultValues,
    reValidateMode: "onChange",
    resetOptions: {
      keepErrors: true,
      keepDirty: true,
      keepDirtyValues: true,
    },
  });

  const signUpMutation = useSignUpMutation(form.setError, onFieldError);

  return {
    form,
    onSubmit: (data: SignUpSchema) =>
      signUpMutation.mutate(data, {
        onSuccess: () => {
          navigate({ to: "/auth", search: { authMode: authModes.logIn } });
        },
      }),
    pendingState: signUpMutation.pendingState,
  };
};

export default useSignUp;

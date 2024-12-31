import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";

import { signUpSchema, SignUpSchema } from "../data/schemas";
import useSignUpMutation from "../mutations/useSignUpMutation";

const useSignUp = (onFieldError: () => void) => {
  const form = useForm<SignUpSchema>({
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

  const { signUpMutation, isPending } = useSignUpMutation(
    form.setError,
    onFieldError,
  );

  return {
    form,
    onSubmit: (data: SignUpSchema) => signUpMutation.mutate(data),
    isSubmitting: isPending,
  };
};

export default useSignUp;

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";

import { signUpSchema, SignUpSchema } from "../data/schemas";
import useSignUpMutation from "../mutations/useSignUpMutation";

const useSignUp = (onFieldError: () => void) => {
  const form = useForm<SignUpSchema>({
    resolver: zodResolver(signUpSchema),
    shouldFocusError: false,
    defaultValues: {
      email: "",
      password: "",
      confirmPassword: "",
      firstName: "",
      lastName: "",
    },
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
    onSubmit: (data: SignUpSchema) => signUpMutation.mutate(data),
    pendingState: signUpMutation.pendingState,
  };
};

export default useSignUp;

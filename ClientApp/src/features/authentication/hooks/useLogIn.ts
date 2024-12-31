import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";

import { logInSchema, LogInSchema } from "../data/schemas";
import useLoginMutation from "../mutations/useLoginMutation";
import useResendEmailVerification from "../mutations/useResendEmailVerification";

const useLogIn = () => {
  const form = useForm<LogInSchema>({
    resolver: zodResolver(logInSchema),
    defaultValues: {
      email: "catalin.roman451@gmail.com",
      password: "Waryor.001",
    },
  });

  const { loginMutation, isPending } = useLoginMutation(
    form.setError,
    resendEmailVerification,
  );

  const { resendEmailVerificationMutation } = useResendEmailVerification();

  function resendEmailVerification(email: string) {
    if (!email) return;

    resendEmailVerificationMutation.mutate({
      email,
    });
  }

  return {
    form,
    onSubmit: (data: LogInSchema) => loginMutation.mutate(data),
    isSubmitting: isPending,
  };
};

export default useLogIn;

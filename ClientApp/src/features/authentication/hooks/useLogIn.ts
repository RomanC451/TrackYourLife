import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { useLocalStorage } from "usehooks-ts";
import { v4 as uuidv4 } from "uuid";

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

  const [deviceId, setDeviceId] = useLocalStorage("deviceId", uuidv4(), {
    serializer: (v) => v,
    deserializer: (v) => v,
  });

  const { resendEmailVerificationMutation } = useResendEmailVerification();

  function resendEmailVerification(email: string) {
    if (!email) return;

    resendEmailVerificationMutation.mutate({
      email,
    });
  }

  return {
    form,
    onSubmit: (data: LogInSchema) =>
      loginMutation.mutateAsync(
        { ...data, deviceId },
        { onSuccess: () => setDeviceId(deviceId) },
      ),
    isSubmitting: isPending,
  };
};

export default useLogIn;

import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { z } from "zod";

import { router } from "@/App";
import { Button } from "@/components/ui/button";
import { authModes } from "@/features/authentication/data/enums";
import { AuthApi } from "@/services/openapi";

const emailVerificationSearchSchema = z.object({
  token: z.string(),
});

export type EmailVerificationSearchSchema = z.infer<
  typeof emailVerificationSearchSchema
>;

const authApi = new AuthApi();

export function EmailVerificationSuccess() {
  const navigate = useNavigate();
  const [count, setCount] = useState(10);

  useEffect(() => {
    const interval = setInterval(() => {
      setCount((current) => current - 1);
    }, 1000);
    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    if (count === 0) {
      navigate({ to: "/auth", search: { authMode: authModes.logIn } });
    }
  }, [count, navigate]);

  return (
    <div className="flex min-h-[50vh] w-full flex-col items-center justify-center gap-4">
      <div className="flex items-center gap-4">Email verified successfully</div>
      <div className="flex items-center gap-4">
        You will be redirected to the login page in {count} seconds.
      </div>
      <div className="flex items-center gap-4">
        <Button
          onClick={() =>
            navigate({ to: "/auth", search: { authMode: authModes.logIn } })
          }
        >
          Redirect now
        </Button>
      </div>
    </div>
  );
}

export const Route = createFileRoute("/email-verification")({
  validateSearch: emailVerificationSearchSchema,
  loaderDeps: ({ search }) => ({ token: search.token }),
  loader: async ({ deps }) => {
    await authApi.verifyEmail({ token: deps.token });
  },
  onError: () => {
    toast.error("Link has expired.");
    router.navigate({ to: "/error" });
  },
  component: EmailVerificationSuccess,
});

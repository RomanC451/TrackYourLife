import { createFileRoute } from "@tanstack/react-router";
import { toast } from "sonner";
import { z } from "zod";

import { router } from "@/App";
import EmailVerificationPage from "@/pages/EmailVerificationPage";
import { AuthApi } from "@/services/openapi";

const emailVerificationSearchSchema = z.object({
  token: z.string(),
});

export type EmailVerificationSearchSchema = z.infer<
  typeof emailVerificationSearchSchema
>;

const authApi = new AuthApi();

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
  component: EmailVerificationPage,
});

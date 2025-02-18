import { createFileRoute, lazyRouteComponent } from "@tanstack/react-router";
import { z } from "zod";

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
  component: lazyRouteComponent(() => import("@/pages/EmailVerificationPage")),
  errorComponent: () => <div>Failed to verify email</div>,
});

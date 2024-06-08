import { createFileRoute, lazyRouteComponent } from "@tanstack/react-router";
import { z } from "zod";
import { UserApi } from "~/services/openapi";

const emailVerificationSearchSchema = z.object({
  token: z.string(),
});

export type EmailVerificationSearchSchema = z.infer<
  typeof emailVerificationSearchSchema
>;

const userApi = new UserApi();

export const Route = createFileRoute("/emailVerification")({
  validateSearch: emailVerificationSearchSchema,
  loaderDeps: ({ search }) => ({ token: search.token }),
  loader: async ({ deps }) => {
    await userApi.verifyEmail(deps.token);
  },
  component: lazyRouteComponent(() => import("~/pages/EmailVerificationPage")),
  errorComponent: () => <div>Failed to verify email</div>,
});

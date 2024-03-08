import { createFileRoute, lazyRouteComponent } from "@tanstack/react-router";
import { z } from "zod";
import { userEndpoints } from "~/data/apiSettings";
import { requestTypes } from "~/hooks/useApiRequests";

const emailVerificationSearchSchema = z.object({
  token: z.string(),
});

export type EmailVerificationSearchSchema = z.infer<
  typeof emailVerificationSearchSchema
>;

export const Route = createFileRoute("/emailVerification")({
  validateSearch: emailVerificationSearchSchema,
  loaderDeps: ({ search }) => ({ token: search.token }),
  loader: async ({ context, deps }) => {
    const fetchRequest = context.fetchRequest;

    const param = encodeURIComponent(deps.token);

    const endpoint = `${userEndpoints.verifyEmail}?token=${param}`;

    await fetchRequest({
      endpoint: endpoint,
      requestType: requestTypes.POST,
    });
  },
  component: lazyRouteComponent(() => import("~/pages/EmailVerificationPage")),
  errorComponent: () => <div>Failed to verify email</div>,
});

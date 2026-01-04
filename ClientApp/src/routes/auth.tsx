import { createFileRoute } from "@tanstack/react-router";
import { z } from "zod";

import { authModes } from "@/features/authentication/data/enums";
import { isValidRedirectUrl } from "@/lib/urlSanitizer";
import AuthPage from "@/pages/AuthPage";

// eslint-disable-next-line react-refresh/only-export-components
export const authSearchSchema = z.object({
  authMode: z.nativeEnum(authModes),
  redirect: z
    .string()
    .optional()
    .refine((url) => !url || isValidRedirectUrl(url), {
      message: "Invalid redirect URL",
    }),
});

export type AuthSearchSchema = z.infer<typeof authSearchSchema>;

export const Route = createFileRoute("/auth")({
  validateSearch: authSearchSchema,

  component: AuthPage,
});

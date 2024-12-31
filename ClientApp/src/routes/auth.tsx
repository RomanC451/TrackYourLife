import AuthPage from "@/pages/AuthPage";
import { createFileRoute } from "@tanstack/react-router";

import { z } from "zod";

// eslint-disable-next-line react-refresh/only-export-components
export const authSearchSchema = z.object({
  redirect: z.string().optional(),
});

export type AuthSearchSchema = z.infer<typeof authSearchSchema>;

export const Route = createFileRoute("/auth")({
  validateSearch: authSearchSchema,
  component: AuthPage,
});

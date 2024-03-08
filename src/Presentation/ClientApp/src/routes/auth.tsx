import { FileRoutesByPath, createFileRoute } from "@tanstack/react-router";
import { z } from "zod";
import { AuthenticationPage } from "~/pages";

export const authSearchSchema = z.object({
  redirect: z.custom<keyof FileRoutesByPath>().optional(),
});

export type AuthSearchSchema = z.infer<typeof authSearchSchema>;

export const Route = createFileRoute("/auth")({
  validateSearch: authSearchSchema,
  component: AuthenticationPage,
});

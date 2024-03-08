import { createFileRoute } from "@tanstack/react-router";
import { LandingPage } from "~/pages";

export const Route = createFileRoute("/")({
  // validateSearch: authSearchSchema,
  component: LandingPage,
});

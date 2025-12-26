import { createFileRoute } from "@tanstack/react-router";

import MissingPage from "@/pages/MissingPage";

export const Route = createFileRoute("/not-found")({
  component: MissingPage,
});

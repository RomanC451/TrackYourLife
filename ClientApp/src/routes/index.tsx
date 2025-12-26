import { createFileRoute, lazyRouteComponent } from "@tanstack/react-router";

import MissingPage from "@/pages/MissingPage";

export const Route = createFileRoute("/")({
  component: lazyRouteComponent(() => import("@/pages/LandingPage")),
  notFoundComponent: MissingPage,
});

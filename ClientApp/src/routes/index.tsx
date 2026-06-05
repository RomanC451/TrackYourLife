import { createFileRoute, lazyRouteComponent } from "@tanstack/react-router";

import { NotFoundContent } from "./not-found";

export const Route = createFileRoute("/")({
  component: lazyRouteComponent(() => import("@/pages/LandingPage")),
  notFoundComponent: NotFoundContent,
});

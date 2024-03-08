import { createFileRoute } from "@tanstack/react-router";
import protectedRouteBoforeLoad from "~/auth/protectedRouteBoforeLoad";
import { HealthPage } from "~/pages";
import LoadingPage from "~/pages/LoadingPage";

export const Route = createFileRoute("/home")({
  loader: protectedRouteBoforeLoad,
  component: HealthPage,
  pendingComponent: LoadingPage,
  pendingMs: 200,
  pendingMinMs: 1000,
  gcTime: 0,
});

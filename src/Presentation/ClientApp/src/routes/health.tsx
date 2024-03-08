import { createFileRoute } from "@tanstack/react-router";
import { HealthPage } from "../pages";

export const Route = createFileRoute("/health")({
  pendingComponent: () => <div>Loading...</div>,
  component: HealthPage,
});

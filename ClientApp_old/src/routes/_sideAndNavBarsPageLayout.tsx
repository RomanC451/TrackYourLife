import { createFileRoute } from "@tanstack/react-router";
import SideAndNavBarsPageLayout from "~/layouts/pageLayouts/SideAndNavBarsPageLayout";

export const Route = createFileRoute("/_sideAndNavBarsPageLayout")({
  component: SideAndNavBarsPageLayout,
});

import { createFileRoute } from "@tanstack/react-router";
import { TabsContent } from "~/chadcn/ui/tabs";

export const Route = createFileRoute("/_sideAndNavBarsPageLayout/nutrition/")({
  component: () => <TabsContent value="health">index</TabsContent>,
});

import { createFileRoute } from "@tanstack/react-router";
import { TabsContent } from "~/chadcn/ui/tabs";
import { CaloriesComponent } from "~/features/health";

export const Route = createFileRoute(
  "/_sideAndNavBarsPageLayout/nutrition/foodDiary",
)({
  pendingComponent: () => <div>Loading...</div>,
  component: () => (
    <TabsContent value="foodDiary" className="w-full">
      <CaloriesComponent />
    </TabsContent>
  ),
});

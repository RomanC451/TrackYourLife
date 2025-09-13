import { createFileRoute } from "@tanstack/react-router";

import { EditFoodDiaryDialog } from "@/features/nutrition/diary/components/foodDiaryDialogs/EditFoodDiaryDialog";
import { foodDiariesQueryOptions } from "@/features/nutrition/diary/queries/useDiaryQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary/_dialogs/foodDiary/edit/$diaryId",
)({
  component: RouteComponent,

  loader: async ({ params }) => {
    await queryClient.ensureQueryData(
      foodDiariesQueryOptions.byId(params.diaryId),
    );
  },
});

function RouteComponent() {
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/nutrition/diary",
  });

  const { diaryId } = Route.useParams();

  return (
    <EditFoodDiaryDialog
      foodDiaryId={diaryId}
      onSuccess={() => {
        navigateBackOrDefault();
      }}
      onClose={() => navigateBackOrDefault()}
    />
  );
}

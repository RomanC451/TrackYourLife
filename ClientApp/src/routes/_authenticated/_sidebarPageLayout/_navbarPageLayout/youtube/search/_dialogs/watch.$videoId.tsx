import { Suspense } from "react";
import { createFileRoute } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";

import VideoPlayerDialog from "@/features/youtube/components/dialogs/VideoPlayerDialog";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/search/_dialogs/watch/$videoId",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const { videoId } = Route.useParams();
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/youtube/search",
  });

  const handleClose = () => {
    navigateBackOrDefault();
  };

  return (
    <Suspense
      fallback={
        <div className="flex h-full w-full items-center justify-center">
          <Loader2 className="h-8 w-8 animate-spin text-white" />
        </div>
      }
    >
      <VideoPlayerDialog videoId={videoId} onClose={handleClose} />
    </Suspense>
  );
}

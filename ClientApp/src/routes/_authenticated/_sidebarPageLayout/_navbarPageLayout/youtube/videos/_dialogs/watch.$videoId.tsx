import { Suspense } from "react";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";

import VideoPlayerDialog from "@/features/youtube/components/dialogs/VideoPlayerDialog";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/videos/_dialogs/watch/$videoId"
)({
  component: RouteComponent,
});

function RouteComponent() {
  const { videoId } = Route.useParams();
  const navigate = useNavigate();

  const handleClose = () => {
    navigate({ to: "/youtube/videos" });
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


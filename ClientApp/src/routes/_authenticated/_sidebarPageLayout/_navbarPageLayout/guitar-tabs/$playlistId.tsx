import { createFileRoute } from "@tanstack/react-router";

import GuitarPlaylistPage from "@/features/guitar-tabs/components/GuitarPlaylistPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/guitar-tabs/$playlistId",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const { playlistId } = Route.useParams();
  return <GuitarPlaylistPage playlistId={playlistId} />;
}

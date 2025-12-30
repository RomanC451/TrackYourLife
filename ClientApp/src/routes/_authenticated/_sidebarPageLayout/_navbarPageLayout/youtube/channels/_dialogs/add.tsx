import { createFileRoute, useNavigate } from "@tanstack/react-router";

import AddChannelDialog from "@/features/youtube/components/dialogs/AddChannelDialog";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/channels/_dialogs/add",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();

  const handleClose = () => {
    navigate({ to: "/youtube/channels" });
  };

  return <AddChannelDialog onClose={handleClose} />;
}

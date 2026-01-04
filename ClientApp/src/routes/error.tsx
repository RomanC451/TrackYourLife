import { createFileRoute, useNavigate } from "@tanstack/react-router";

import { Button } from "@/components/ui/button";
import { authModes } from "@/features/authentication/data/enums";

export const Route = createFileRoute("/error")({
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();
  return (
    <div className="flex h-[50vh] w-full flex-col items-center justify-center gap-4 text-2xl">
      <p>Ooops... something went wrong</p>
      <Button
        onClick={() =>
          navigate({ to: "/auth", search: { authMode: authModes.logIn } })
        }
      >
        Go back
      </Button>
    </div>
  );
}

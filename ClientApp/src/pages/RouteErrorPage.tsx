import { useNavigate } from "@tanstack/react-router";
import { CircleAlert } from "lucide-react";

import { Button } from "@/components/ui/button";

function RouteErrorPage({
  error,
  reset,
}: {
  error: unknown;
  reset: () => void;
}) {
  const navigate = useNavigate();

  if (import.meta.env.DEV) {
    const message =
      error instanceof Error ? (error.stack ?? error.message) : String(error);
    return <pre style={{ whiteSpace: "pre-wrap" }}>{message}</pre>;
  }

  return (
    <div className="flex min-h-[50vh] w-full flex-col items-center justify-center gap-4 px-6 text-center">
      <div className="flex size-16 items-center justify-center rounded-full bg-muted">
        <CircleAlert className="size-8 text-muted-foreground" />
      </div>
      <div className="flex max-w-md flex-col gap-2">
        <h1 className="text-2xl font-semibold tracking-tight">
          Something went wrong
        </h1>
        <p className="text-muted-foreground">
          This page ran into a problem. Try again, or go back home.
        </p>
      </div>
      <div className="flex gap-3">
        <Button type="button" onClick={() => reset()}>
          Try again
        </Button>
        <Button
          type="button"
          variant="outline"
          onClick={() => {
            void navigate({ to: "/home" });
          }}
        >
          Go home
        </Button>
      </div>
    </div>
  );
}

export default RouteErrorPage;

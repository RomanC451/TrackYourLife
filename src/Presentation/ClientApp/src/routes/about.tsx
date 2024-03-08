import { Link, createFileRoute } from "@tanstack/react-router";
import protectedRouteBoforeLoad from "~/auth/protectedRouteBoforeLoad";
import { Button } from "~/chadcn/ui/button";
import LoadingPage from "~/pages/LoadingPage";

export const Route = createFileRoute("/about")({
  beforeLoad: protectedRouteBoforeLoad,
  component: About,
  pendingComponent: LoadingPage,
});

function About() {
  return (
    <div className="p-2">
      Hello from About!
      <Link to="/home" preload={false}>
        <Button>Go Home.</Button>
      </Link>
    </div>
  );
}

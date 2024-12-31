import { useEffect, useState } from "react";
import { createFileRoute } from "@tanstack/react-router";

import BooleanSpan from "@/components/debug/BooleanSpan";
import DebugCard from "@/components/debug/DebugCard";
import JsonStringifySpan from "@/components/debug/JsonStringifySpan";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import { useSidebar } from "@/components/ui/sidebar";
import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/debug",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const { open, isHovered } = useSidebar();
  const { userData, userLoggedIn } = useAuthenticationContext();

  const [isUserLoggedIn, setIsUserLoggedIn] = useState(false);

  useEffect(() => {
    userLoggedIn().then((res) => setIsUserLoggedIn(res));
  }, [userLoggedIn]);

  return (
    <div className="flex flex-wrap gap-4 p-10">
      <DebugCard title="Sidebar">
        <DebugCard title="Open">
          <BooleanSpan state={open} />
        </DebugCard>
        <DebugCard title="IsHovered">
          <BooleanSpan state={isHovered} />
        </DebugCard>
      </DebugCard>
      <DebugCard title={"isUserLoggedIn"}>
        <BooleanSpan state={isUserLoggedIn} />
      </DebugCard>
      <DebugCard title={"userData"} className="overflow-scroll">
        <JsonStringifySpan object={userData} />
      </DebugCard>
      <Sheet>
        <SheetTrigger>Notifications</SheetTrigger>
        <SheetContent></SheetContent>
      </Sheet>
    </div>
  );
}

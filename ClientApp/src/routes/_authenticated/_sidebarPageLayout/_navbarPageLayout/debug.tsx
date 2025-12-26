import { useEffect, useState } from "react";
import { createFileRoute } from "@tanstack/react-router";

import BooleanSpan from "@/components/debug/BooleanSpan";
import DebugCard from "@/components/debug/DebugCard";
import JsonStringifySpan from "@/components/debug/JsonStringifySpan";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { useSidebar } from "@/components/ui/sidebar";
import { useAppGeneralStateContext } from "@/contexts/AppGeneralContextProvider";
import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";
import { useMobileOptimizations } from "@/hooks/use-mobile-optimizations";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/debug",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const { open, isHovered } = useSidebar();
  const { userData, userLoggedIn } = useAuthenticationContext();
  const { screenSize } = useAppGeneralStateContext();
  const { isLowEndDevice, isMobile } = useMobileOptimizations();
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
      <DebugCard title={"screenSizes"}>
        <JsonStringifySpan object={screenSize} />
      </DebugCard>
      <DebugCard title={"isLowEndDevice"}>
        <JsonStringifySpan object={isLowEndDevice} />
      </DebugCard>
      <DebugCard title={"isMobile"}>
        <JsonStringifySpan object={isMobile} />
      </DebugCard>
      <Card className="p-4">
        <Input type="number" />
      </Card>
      <Button
        onClick={async () => {
          const isLoggedIn = await userLoggedIn();
          console.log(isLoggedIn);
        }}
      >
        IsLoggedIn
      </Button>
    </div>
  );
}

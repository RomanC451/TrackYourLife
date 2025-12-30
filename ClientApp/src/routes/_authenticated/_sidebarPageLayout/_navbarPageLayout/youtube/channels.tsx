import { Suspense, useState } from "react";
import { createFileRoute, Outlet, useNavigate } from "@tanstack/react-router";
import { Loader2, Plus } from "lucide-react";

import { router } from "@/App";
import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import ChannelsList from "@/features/youtube/components/channelsList/ChannelsList";
import CategoryTabs, {
  CategoryTabValue,
} from "@/features/youtube/components/common/CategoryTabs";
import { VideoCategory } from "@/services/openapi";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/channels",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();
  const [category, setCategory] = useState<CategoryTabValue>("all");

  const selectedCategory: VideoCategory | null =
    category === "all" ? null : category;

  return (
    <PageCard>
      <PageTitle title="Channels">
        <div className="flex items-center gap-4">
          <CategoryTabs value={category} onValueChange={setCategory} />
          <Button
            onClick={() => {
              navigate({ to: "/youtube/channels/add" });
            }}
            onMouseEnter={() => {
              router.preloadRoute({ to: "/youtube/channels/add" });
            }}
            onTouchStart={() => {
              router.preloadRoute({ to: "/youtube/channels/add" });
            }}
          >
            <Plus className="mr-1 h-4 w-4" />
            Add Channel
          </Button>
        </div>
      </PageTitle>
      <Suspense
        fallback={
          <div className="flex items-center justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
          </div>
        }
      >
        <ChannelsList category={selectedCategory} />
      </Suspense>
      <Outlet />
    </PageCard>
  );
}

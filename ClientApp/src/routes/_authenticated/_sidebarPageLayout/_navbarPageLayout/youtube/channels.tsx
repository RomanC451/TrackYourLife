import { Suspense, useCallback } from "react";
import { createFileRoute, Outlet, useNavigate } from "@tanstack/react-router";
import { Loader2, Plus } from "lucide-react";
import z from "zod";

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
  validateSearch: z.object({
    category: z.nativeEnum(VideoCategory).default(VideoCategory.Educational),
  }),
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();
  const { category } = Route.useSearch();

  const handleCategoryChange = useCallback(
    (category: CategoryTabValue) => {
      navigate({ to: "/youtube/channels", search: { category } });
    },
    [navigate],
  );

  return (
    <PageCard>
      <PageTitle title="Channels">
        <CategoryTabs value={category} onValueChange={handleCategoryChange} />
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
        {/* </div> */}
      </PageTitle>
      <Suspense
        fallback={
          <div className="flex items-center justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
          </div>
        }
      >
        <ChannelsList category={category} />
      </Suspense>
      <Outlet />
    </PageCard>
  );
}

import { Suspense, useCallback } from "react";
import { createFileRoute, Outlet, useNavigate } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";
import { z } from "zod";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import CategoryTabs, {
  CategoryTabValue,
} from "@/features/youtube/components/common/CategoryTabs";
import VideosList from "@/features/youtube/components/videosList/VideosList";
import { VideoCategory } from "@/services/openapi";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/videos",
)({
  validateSearch: z.object({
    category: z.nativeEnum(VideoCategory).default(VideoCategory.Educational),
  }),
  component: RouteComponent,
});

function RouteComponent() {
  const { category } = Route.useSearch();

  const navigate = useNavigate();

  const handleCategoryChange = useCallback(
    (category: CategoryTabValue) => {
      navigate({ to: "/youtube/videos", search: { category } });
    },
    [navigate],
  );

  return (
    <PageCard>
      <PageTitle title="Videos">
        <CategoryTabs value={category} onValueChange={handleCategoryChange} />
      </PageTitle>
      <Suspense
        fallback={
          <div className="flex items-center justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
          </div>
        }
      >
        <VideosList category={category} />
      </Suspense>
      <Outlet />
    </PageCard>
  );
}

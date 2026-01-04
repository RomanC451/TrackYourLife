import { Suspense, useState } from "react";
import { createFileRoute, Outlet } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";

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
  component: RouteComponent,
});

function RouteComponent() {
  const [category, setCategory] = useState<CategoryTabValue>(
    VideoCategory.Educational,
  );

  return (
    <PageCard>
      <PageTitle title="Videos">
        <CategoryTabs value={category} onValueChange={setCategory} />
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

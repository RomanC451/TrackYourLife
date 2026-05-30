import { Suspense, useCallback, useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { createFileRoute, Outlet, useNavigate } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";
import { z } from "zod";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import CategoryTabs, {
  CategoryTabValue,
} from "@/features/youtube/components/common/CategoryTabs";
import VideosList from "@/features/youtube/components/videosList/VideosList";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";
import useYoutubeFavoritesTab from "@/features/youtube/hooks/useYoutubeFavoritesTab";
import {
  listFilterFromSearch,
  useSyncYoutubeListSearchParams,
} from "@/features/youtube/youtubeListSearch";

const youtubeCategorySearchSchema = z.object({
  youtubeCategoryId: z
    .union([z.string().uuid(), z.literal("all"), z.literal("favorites")])
    .optional(),
});

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/videos",
)({
  validateSearch: youtubeCategorySearchSchema,
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();
  const search = Route.useSearch();

  const settingsQuery = useQuery({
    ...youtubeQueryOptions.settings(),
  });

  const { hasFavoriteChannels } = useYoutubeFavoritesTab(settingsQuery.isSuccess);

  const sortedCategories = useSyncYoutubeListSearchParams({
    isSettingsSuccess: settingsQuery.isSuccess,
    categories: settingsQuery.data?.categories ?? [],
    youtubeCategoryId: search.youtubeCategoryId,
    hasFavoriteChannels,
    navigate,
    base: "/youtube/videos",
  });

  const listFilter = useMemo(
    () =>
      listFilterFromSearch(
        search.youtubeCategoryId,
        settingsQuery.isSuccess && sortedCategories.length > 0
          ? search.youtubeCategoryId !== undefined
          : false,
      ),
    [
      search.youtubeCategoryId,
      settingsQuery.isSuccess,
      sortedCategories.length,
    ],
  );

  const handleCategoryChange = useCallback(
    (next: CategoryTabValue) => {
      navigate({
        to: "/youtube/videos",
        search: {
          youtubeCategoryId: next,
        },
      });
    },
    [navigate],
  );

  const tabValue: CategoryTabValue | null =
    search.youtubeCategoryId === "all" ||
    search.youtubeCategoryId === "favorites"
      ? search.youtubeCategoryId
      : search.youtubeCategoryId && listFilter !== null
        ? listFilter
        : null;

  return (
    <PageCard>
      <PageTitle title="Videos">
        {settingsQuery.isPending ? (
          <CategoryTabs
            categories={[]}
            value="all"
            onValueChange={() => {}}
            isLoading
          />
        ) : settingsQuery.isError ? (
          <Alert variant="destructive" className="max-w-md">
            <AlertTitle>Could not load categories</AlertTitle>
            <AlertDescription className="flex flex-col gap-2">
              <p>Check your connection and try again.</p>
              <Button
                type="button"
                variant="outline"
                size="sm"
                className="w-fit"
                onClick={() => settingsQuery.refetch()}
              >
                Retry
              </Button>
            </AlertDescription>
          </Alert>
        ) : sortedCategories.length === 0 ? (
          <p className="text-sm text-muted-foreground">
            Add a category in YouTube settings to filter videos.
          </p>
        ) : (
          tabValue !== null && (
            <CategoryTabs
              categories={sortedCategories}
              value={tabValue}
              onValueChange={handleCategoryChange}
              showFavoritesTab={hasFavoriteChannels}
            />
          )
        )}
      </PageTitle>
      {listFilter !== null && sortedCategories.length > 0 ? (
        <Suspense
          fallback={
            <div className="flex items-center justify-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
          }
        >
          <VideosList key={listFilter} categoryFilter={listFilter} />
        </Suspense>
      ) : settingsQuery.isSuccess && sortedCategories.length === 0 ? (
        <div className="flex flex-col items-center justify-center py-12 text-center text-muted-foreground">
          <p>No categories configured.</p>
        </div>
      ) : settingsQuery.isSuccess ? (
        <div className="flex items-center justify-center py-12">
          <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
        </div>
      ) : null}
      <Outlet />
    </PageCard>
  );
}

import { Suspense, useCallback, useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { createFileRoute, Outlet, useNavigate, useRouterState } from "@tanstack/react-router";
import { Loader2, Plus } from "lucide-react";
import z from "zod";

import { router } from "@/App";
import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import ChannelsList from "@/features/youtube/components/channelsList/ChannelsList";
import CategoryTabs, {
  CategoryTabValue,
} from "@/features/youtube/components/common/CategoryTabs";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";
import {
  listFilterFromSearch,
  useSyncYoutubeListSearchParams,
} from "@/features/youtube/youtubeListSearch";

const youtubeCategorySearchSchema = z.object({
  youtubeCategoryId: z.union([z.string().uuid(), z.literal("all")]).optional(),
});

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/channels",
)({
  validateSearch: youtubeCategorySearchSchema,
  component: RouteComponent,
});

function RouteComponent() {
  return <ChannelsListPage />;
}

function ChannelsListPage() {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const pathname = useRouterState({ select: (s) => s.location.pathname });
  const isAddChannelDialog = pathname.endsWith("/youtube/channels/add");

  const settingsQuery = useQuery({
    ...youtubeQueryOptions.settings(),
  });

  const sortedCategories = useSyncYoutubeListSearchParams({
    isSettingsSuccess: settingsQuery.isSuccess,
    categories: settingsQuery.data?.categories ?? [],
    youtubeCategoryId: search.youtubeCategoryId,
    navigate,
    base: "/youtube/channels",
    syncSearch: !isAddChannelDialog,
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
        to: "/youtube/channels",
        search: {
          youtubeCategoryId: next === "all" ? "all" : next,
        },
      });
    },
    [navigate],
  );

  const tabValue: CategoryTabValue | null =
    search.youtubeCategoryId === "all"
      ? "all"
      : search.youtubeCategoryId && listFilter !== null
        ? listFilter
        : null;

  const addChannelDialogSearch = useMemo((): {
    youtubeCategoryId: string | "all";
  } => {
    const tab =
      search.youtubeCategoryId === "all" ||
      search.youtubeCategoryId === undefined
        ? sortedCategories[0]?.id
        : search.youtubeCategoryId;
    return {
      youtubeCategoryId: tab === undefined ? "all" : tab,
    };
  }, [search.youtubeCategoryId, sortedCategories]);

  return (
    <PageCard>
      <PageTitle title="Channels">
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
            Add a category in YouTube settings to organize channels.
          </p>
        ) : (
          tabValue !== null && (
            <CategoryTabs
              categories={sortedCategories}
              value={tabValue}
              onValueChange={handleCategoryChange}
            />
          )
        )}
        <Button
          onClick={() => {
            navigate({
              to: "/youtube/channels/add",
              search: addChannelDialogSearch,
            });
          }}
          onMouseEnter={() => {
            router.preloadRoute({
              to: "/youtube/channels/add",
              search: addChannelDialogSearch,
            });
          }}
          onTouchStart={() => {
            router.preloadRoute({
              to: "/youtube/channels/add",
              search: addChannelDialogSearch,
            });
          }}
        >
          <Plus className="mr-1 h-4 w-4" />
          Add Channel
        </Button>
      </PageTitle>
      {listFilter !== null && sortedCategories.length > 0 ? (
        <Suspense
          fallback={
            <div className="flex items-center justify-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
          }
        >
          <ChannelsList
            categoryFilter={listFilter}
            categories={sortedCategories}
          />
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

import { useMemo } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute, useNavigate } from "@tanstack/react-router";

import AddChannelDialog from "@/features/youtube/components/dialogs/AddChannelDialog";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";
import { sortYoutubeCategoriesByDisplayOrder } from "@/features/youtube/youtubeListSearch";

import { Route as YoutubeChannelsRoute } from "../../channels";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/channels/_dialogs/add",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();
  const search = YoutubeChannelsRoute.useSearch();
  const { data: settings } = useSuspenseQuery({
    ...youtubeQueryOptions.settings(),
  });

  const sortedCategories = useMemo(
    () => sortYoutubeCategoriesByDisplayOrder(settings.categories),
    [settings.categories],
  );

  const defaultYoutubeCategoryId = useMemo(() => {
    if (
      search.youtubeCategoryId === "all" ||
      search.youtubeCategoryId === "favorites" ||
      search.youtubeCategoryId === undefined
    ) {
      return sortedCategories[0]?.id ?? "";
    }
    return search.youtubeCategoryId;
  }, [search.youtubeCategoryId, sortedCategories]);

  const categoriesForDialog = useMemo(() => {
    if (!defaultYoutubeCategoryId) {
      return sortedCategories;
    }
    return [...sortedCategories].sort((a, b) => {
      if (a.id === defaultYoutubeCategoryId) {
        return -1;
      }
      if (b.id === defaultYoutubeCategoryId) {
        return 1;
      }
      return a.displayOrder - b.displayOrder;
    });
  }, [sortedCategories, defaultYoutubeCategoryId]);

  const handleClose = () => {
    navigate({ to: "/youtube/channels" });
  };

  return (
    <AddChannelDialog
      onClose={handleClose}
      categories={categoriesForDialog}
      defaultYoutubeCategoryId={defaultYoutubeCategoryId}
    />
  );
}

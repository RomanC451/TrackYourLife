import { useEffect, useMemo } from "react";

import type { YoutubeCategorySettingsDto } from "@/services/openapi";

import type {
  YoutubeCategoryListFilter,
  YoutubeCategorySearchParam,
} from "./queries/youtubeQueries";

export function sortYoutubeCategoriesByDisplayOrder<
  T extends { displayOrder: number },
>(categories: T[]): T[] {
  return [...categories].sort((a, b) => a.displayOrder - b.displayOrder);
}

export function useSyncYoutubeListSearchParams(args: {
  isSettingsSuccess: boolean;
  categories: YoutubeCategorySettingsDto[];
  youtubeCategoryId: YoutubeCategorySearchParam;
  navigate: (opts: {
    to: "/youtube/channels" | "/youtube/videos";
    search: { youtubeCategoryId: YoutubeCategoryListFilter };
    replace?: boolean;
  }) => void;
  base: "/youtube/channels" | "/youtube/videos";
  /** When false, skip default/stale-id redirects (e.g. on `/youtube/channels/add` so we do not replace away from the dialog). */
  syncSearch?: boolean;
}) {
  const {
    isSettingsSuccess,
    categories,
    youtubeCategoryId,
    navigate,
    base,
    syncSearch: syncSearchArg,
  } = args;

  const sorted = useMemo(
    () => sortYoutubeCategoriesByDisplayOrder(categories),
    [categories],
  );

  const syncSearch = syncSearchArg !== false;

  useEffect(() => {
    if (!syncSearch) {
      return;
    }
    if (!isSettingsSuccess) {
      return;
    }
    if (sorted.length === 0) {
      return;
    }
    const id = youtubeCategoryId;
    const ids = new Set(sorted.map((c) => c.id));
    if (id !== undefined && id !== "all" && !ids.has(id)) {
      navigate({
        to: base,
        search: { youtubeCategoryId: "all" },
        replace: true,
      });
      return;
    }
    if (id === undefined) {
      navigate({
        to: base,
        search: { youtubeCategoryId: sorted[0].id },
        replace: true,
      });
    }
  }, [
    syncSearch,
    isSettingsSuccess,
    sorted,
    youtubeCategoryId,
    navigate,
    base,
  ]);

  return sorted;
}

export function listFilterFromSearch(
  youtubeCategoryId: YoutubeCategorySearchParam,
  isReady: boolean,
): YoutubeCategoryListFilter | null {
  if (!isReady) {
    return null;
  }
  if (youtubeCategoryId === "all") {
    return "all";
  }
  if (youtubeCategoryId === undefined) {
    return null;
  }
  return youtubeCategoryId;
}

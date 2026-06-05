import globalAxios from "axios";
import { infiniteQueryOptions } from "@tanstack/react-query";

import { BASE_PATH } from "@/services/openapi/base";

import type { PagedWatchHistory } from "../watchHistoryTypes";

export const watchHistoryQueryKeys = {
  all: ["youtube", "watchHistory"] as const,
};

const PAGE_SIZE = 20;

async function fetchWatchHistory(
  page: number,
  pageSize: number,
): Promise<PagedWatchHistory> {
  const { data } = await globalAxios.get<PagedWatchHistory>(
    `${BASE_PATH}/api/videos/watch-history`,
    {
      params: { page, pageSize },
    },
  );
  return data;
}

export const watchHistoryQueryOptions = {
  infinite: () =>
    infiniteQueryOptions({
      queryKey: watchHistoryQueryKeys.all,
      queryFn: ({ pageParam }) => fetchWatchHistory(pageParam, PAGE_SIZE),
      initialPageParam: 1,
      getNextPageParam: (lastPage) =>
        lastPage.hasNextPage ? lastPage.page + 1 : undefined,
    }),
};

export { PAGE_SIZE as WATCH_HISTORY_PAGE_SIZE };

import { infiniteQueryOptions } from "@tanstack/react-query";

import { VideosApi } from "@/services/openapi";

const videosApi = new VideosApi();

export const watchHistoryQueryKeys = {
  all: ["youtube", "watchHistory"] as const,
};

const PAGE_SIZE = 20;

export const watchHistoryQueryOptions = {
  infinite: () =>
    infiniteQueryOptions({
      queryKey: watchHistoryQueryKeys.all,
      queryFn: async ({ pageParam }) => {
        const response = await videosApi.getWatchHistory(pageParam, PAGE_SIZE);
        return response.data;
      },
      initialPageParam: 1,
      getNextPageParam: (lastPage) =>
        lastPage.hasNextPage ? lastPage.page + 1 : undefined,
    }),
};

export { PAGE_SIZE as WATCH_HISTORY_PAGE_SIZE };

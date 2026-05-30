import { useQuery } from "@tanstack/react-query";

import {
  youtubeCategoryListFilterFavorites,
  youtubeQueryOptions,
} from "../queries/youtubeQueries";

function useYoutubeFavoritesTab(enabled: boolean) {
  const favoritesQuery = useQuery({
    ...youtubeQueryOptions.channels(youtubeCategoryListFilterFavorites),
    enabled,
  });

  const hasFavoriteChannels = (favoritesQuery.data?.length ?? 0) > 0;

  return {
    hasFavoriteChannels,
    isFavoritesTabLoading: favoritesQuery.isPending && enabled,
  };
}

export default useYoutubeFavoritesTab;

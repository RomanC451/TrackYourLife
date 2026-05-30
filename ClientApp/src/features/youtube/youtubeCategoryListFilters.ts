export const youtubeCategoryListFilterAll = "all" as const;

export const youtubeCategoryListFilterFavorites = "favorites" as const;

export type YoutubeCategoryListFilter =
  | typeof youtubeCategoryListFilterAll
  | typeof youtubeCategoryListFilterFavorites
  | (string & {});

export type YoutubeCategorySearchParam = YoutubeCategoryListFilter | undefined;

export function toYoutubeCategoryApiParam(
  filter: YoutubeCategoryListFilter,
): string | undefined {
  if (
    filter === youtubeCategoryListFilterAll ||
    filter === youtubeCategoryListFilterFavorites
  ) {
    return undefined;
  }
  return filter;
}

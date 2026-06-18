import { queryOptions } from "@tanstack/react-query";

import { BookSortField, BookStatus, BooksApi } from "@/services/openapi";

const booksApi = new BooksApi();

export { BookSortField };

export const booksQueryKeys = {
  all: ["books"] as const,
  list: (params?: {
    status?: BookStatus;
    sortBy?: BookSortField;
    sortDescending?: boolean;
  }) => [...booksQueryKeys.all, "list", params ?? {}] as const,
  byId: (id: string) => [...booksQueryKeys.all, id] as const,
};

export const booksQueryOptions = {
  list: (params?: {
    status?: BookStatus;
    sortBy?: BookSortField;
    sortDescending?: boolean;
  }) =>
    queryOptions({
      queryKey: booksQueryKeys.list(params),
      queryFn: () =>
        booksApi
          .getBooks(
            params?.status,
            params?.sortBy,
            params?.sortDescending,
          )
          .then((r) => r.data),
    }),
  byId: (id: string) =>
    queryOptions({
      queryKey: booksQueryKeys.byId(id),
      queryFn: () => booksApi.getBookById(id).then((r) => r.data),
    }),
};

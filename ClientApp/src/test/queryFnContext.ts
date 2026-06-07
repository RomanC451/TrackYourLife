import type { QueryClient } from "@tanstack/react-query";

type BaseQueryFnContext<TQueryKey extends readonly unknown[]> = {
  client: QueryClient;
  queryKey: TQueryKey;
  signal: AbortSignal;
  meta: Record<string, unknown> | undefined;
};

type QueryFnContextOptions<TQueryKey extends readonly unknown[]> = {
  client: QueryClient;
  queryKey: TQueryKey;
  signal?: AbortSignal;
  meta?: Record<string, unknown>;
  pageParam?: number;
  direction?: "forward" | "backward";
};

export function createQueryFnContext<TQueryKey extends readonly unknown[]>(
  options: QueryFnContextOptions<TQueryKey> & {
    pageParam: number;
    direction: "forward" | "backward";
  },
): BaseQueryFnContext<TQueryKey> & {
  pageParam: number;
  direction: "forward" | "backward";
};

export function createQueryFnContext<TQueryKey extends readonly unknown[]>(
  options: QueryFnContextOptions<TQueryKey>,
): BaseQueryFnContext<TQueryKey>;

export function createQueryFnContext<TQueryKey extends readonly unknown[]>({
  client,
  queryKey,
  signal = new AbortController().signal,
  meta = undefined,
  pageParam,
  direction,
}: QueryFnContextOptions<TQueryKey>) {
  return {
    client,
    queryKey,
    signal,
    meta,
    ...(pageParam !== undefined && direction !== undefined
      ? { pageParam, direction }
      : {}),
  };
}

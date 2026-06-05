import { useEffect, useMemo, useRef } from "react";
import { useInfiniteQuery } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";

import WatchHistoryEntryCard from "./WatchHistoryEntryCard";
import type { WatchedVideoHistoryEntry } from "@/services/openapi";
import { watchHistoryQueryOptions } from "../queries/useWatchHistoryQuery";

function WatchHistoryList() {
  const loadMoreRef = useRef<HTMLDivElement>(null);

  const {
    data,
    error,
    fetchNextPage,
    hasNextPage,
    isError,
    isFetchingNextPage,
    isPending,
    refetch,
  } = useInfiniteQuery(watchHistoryQueryOptions.infinite());

  const entries = useMemo(
    () => data?.pages.flatMap((page) => page.items) ?? [],
    [data],
  );

  useEffect(() => {
    const node = loadMoreRef.current;
    if (!node) {
      return;
    }

    const observer = new IntersectionObserver(
      (observerEntries) => {
        if (
          observerEntries[0]?.isIntersecting &&
          hasNextPage &&
          !isFetchingNextPage
        ) {
          fetchNextPage();
        }
      },
      { rootMargin: "200px" },
    );

    observer.observe(node);
    return () => observer.disconnect();
  }, [fetchNextPage, hasNextPage, isFetchingNextPage]);

  if (isPending) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    );
  }

  if (isError) {
    return (
      <Alert variant="destructive" className="max-w-md">
        <AlertTitle>Could not load watch history</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{error instanceof Error ? error.message : "Check your connection and try again."}</p>
          <Button type="button" variant="outline" size="sm" className="w-fit" onClick={() => refetch()}>
            Retry
          </Button>
        </AlertDescription>
      </Alert>
    );
  }

  if (entries.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-12 text-center">
        <p className="text-lg text-muted-foreground">No watch history yet</p>
        <p className="mt-2 text-sm text-muted-foreground">
          Videos you watch through TrackYourLife will appear here
        </p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-1 gap-4 @lg/page-card:grid-cols-2 @3xl/page-card:grid-cols-3 @5xl/page-card:grid-cols-4">
        {entries.map((entry: WatchedVideoHistoryEntry) => (
          <WatchHistoryEntryCard key={`${entry.videoId}-${entry.watchedAtUtc}`} entry={entry} />
        ))}
      </div>

      <div ref={loadMoreRef} className="flex justify-center py-4">
        {isFetchingNextPage ? (
          <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
        ) : hasNextPage ? (
          <span className="text-sm text-muted-foreground">Scroll for more</span>
        ) : (
          <span className="text-sm text-muted-foreground">You&apos;ve reached the end</span>
        )}
      </div>
    </div>
  );
}

export default WatchHistoryList;

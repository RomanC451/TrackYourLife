import { useEffect, useMemo, useState } from "react";
import { createFileRoute, Outlet } from "@tanstack/react-router";
import { debounce } from "lodash";
import { Loader2, Play, Search } from "lucide-react";

import PageCard from "@/components/common/PageCard";
import { Input } from "@/components/ui/input";
import SearchVideosList from "@/features/youtube/components/videosList/SearchVideosList";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";
import { useCustomQuery } from "@/hooks/useCustomQuery";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/search",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const [searchValue, setSearchValue] = useState("");
  const [searchQuery, setSearchQuery] = useState("");
  const [keepPreviousResults, setKeepPreviousResults] = useState(false);

  // Debounced function to update search query state
  const debouncedUpdateSearch = useMemo(
    () =>
      debounce((value: string) => {
        setSearchQuery(value);
      }, 1000),
    [],
  );

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setSearchValue(value);

    if (value === "") {
      debouncedUpdateSearch.cancel();
      setSearchQuery("");
      setKeepPreviousResults(false);
      return;
    }

    debouncedUpdateSearch(value);
  };

  const maxResults = 10;

  const {
    query: { isFetching, isSuccess, isPlaceholderData },
  } = useCustomQuery(youtubeQueryOptions.videoSearch(searchQuery, maxResults));

  useEffect(() => {
    if (
      searchQuery.length > 0 &&
      isSuccess &&
      !isPlaceholderData
    ) {
      setKeepPreviousResults(true);
    }
  }, [searchQuery, isSuccess, isPlaceholderData]);

  const isWaitingForDebounce =
    searchValue.length > 0 && searchValue !== searchQuery;
  const isLoadingSearch = isFetching || isWaitingForDebounce;
  const hasActiveSearch = searchValue.length > 0;

  return (
    <PageCard>
      <div
        className={
          hasActiveSearch
            ? "mb-6 flex justify-center"
            : "flex min-h-[50vh] flex-col items-center justify-center gap-8 text-center"
        }
      >
        {!hasActiveSearch && (
          <div className="flex flex-col items-center gap-4">
            <div className="flex size-20 items-center justify-center rounded-full bg-muted">
              <Play className="h-10 w-10 text-muted-foreground" />
            </div>
            <h2 className="text-2xl font-semibold tracking-tight">
              Search for Videos
            </h2>
          </div>
        )}
        <div className="relative w-full max-w-2xl">
          {searchValue.length > 0 && isLoadingSearch ? (
            <Loader2 className="absolute left-3 top-1/2 h-5 w-5 -translate-y-1/2 animate-spin text-muted-foreground" />
          ) : (
            <Search className="absolute left-3 top-1/2 h-5 w-5 -translate-y-1/2 text-muted-foreground" />
          )}
          <Input
            type="text"
            placeholder="Search for videos..."
            value={searchValue}
            onChange={handleSearchChange}
            className="h-12 w-full pl-10 text-base"
          />
        </div>
      </div>
      {hasActiveSearch && (
        <SearchVideosList
          searchQuery={searchQuery}
          maxResults={maxResults}
          keepPreviousResults={keepPreviousResults}
        />
      )}
      <Outlet />
    </PageCard>
  );
}

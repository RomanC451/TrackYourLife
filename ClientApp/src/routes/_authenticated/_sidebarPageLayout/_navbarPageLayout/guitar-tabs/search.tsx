import { useEffect, useMemo, useState } from "react";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { keepPreviousData } from "@tanstack/react-query";
import { debounce } from "lodash";
import { Guitar, Loader2, Search } from "lucide-react";
import { z } from "zod";

import PageCard from "@/components/common/PageCard";
import { Input } from "@/components/ui/input";
import GuitarTabSearchResults from "@/features/guitar-tabs/components/GuitarTabSearchResults";
import { guitarTabsQueryOptions } from "@/features/guitar-tabs/queries/guitarTabsQueries";
import { useCustomQuery } from "@/hooks/useCustomQuery";

const searchDebounceMs = 1000;

const guitarTabSearchSchema = z.object({
  q: z.string().default(""),
  page: z.coerce.number().int().min(1).default(1),
});

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/guitar-tabs/search",
)({
  validateSearch: guitarTabSearchSchema,
  component: GuitarTabSearchPage,
});

function GuitarTabSearchPage() {
  const { q, page } = Route.useSearch();
  const navigate = useNavigate();
  const [searchValue, setSearchValue] = useState(q);

  const debouncedUpdateSearch = useMemo(
    () =>
      debounce((value: string) => {
        void navigate({
          to: "/guitar-tabs/search",
          search: { q: value, page: 1 },
          replace: true,
        });
      }, searchDebounceMs),
    [navigate],
  );

  useEffect(() => () => debouncedUpdateSearch.cancel(), [debouncedUpdateSearch]);

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = event.target.value;
    setSearchValue(value);

    if (value.trim() === "") {
      debouncedUpdateSearch.cancel();
      void navigate({
        to: "/guitar-tabs/search",
        search: { q: "", page: 1 },
        replace: true,
      });
      return;
    }

    debouncedUpdateSearch(value.trim());
  };

  const hasActiveSearch = searchValue.trim().length > 0;

  const {
    query: { data, isFetching, isError, isPlaceholderData },
  } = useCustomQuery({
    ...guitarTabsQueryOptions.tabSearch(q, page),
    enabled: q.length > 0,
    placeholderData: keepPreviousData,
  });

  const isWaitingForDebounce =
    searchValue.trim().length > 0 && searchValue.trim() !== q;
  const isLoadingSearch = isFetching || isWaitingForDebounce;

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
              <Guitar className="h-10 w-10 text-muted-foreground" />
            </div>
            <h2 className="text-2xl font-semibold tracking-tight">
              Search for songs
            </h2>
          </div>
        )}
        <div className="relative w-full max-w-2xl">
          {hasActiveSearch && isLoadingSearch ? (
            <Loader2 className="absolute left-3 top-1/2 h-5 w-5 -translate-y-1/2 animate-spin text-muted-foreground" />
          ) : (
            <Search className="absolute left-3 top-1/2 h-5 w-5 -translate-y-1/2 text-muted-foreground" />
          )}
          <Input
            type="text"
            placeholder="Search songs on Ultimate Guitar..."
            value={searchValue}
            onChange={handleSearchChange}
            className="h-12 w-full pl-10 text-base"
          />
        </div>
      </div>
      {hasActiveSearch && (
        <GuitarTabSearchResults
          response={q.length > 0 ? data : undefined}
          isError={isError}
          isLoading={isLoadingSearch && !isPlaceholderData}
          onPageChange={(nextPage) => {
            void navigate({
              to: "/guitar-tabs/search",
              search: { q, page: nextPage },
              replace: true,
            });
          }}
        />
      )}
    </PageCard>
  );
}

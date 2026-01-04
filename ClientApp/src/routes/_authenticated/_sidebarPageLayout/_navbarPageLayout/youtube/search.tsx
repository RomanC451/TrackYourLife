import { useMemo, useState } from "react";
import { createFileRoute, Outlet } from "@tanstack/react-router";
import { debounce } from "lodash";
import { Play, Search } from "lucide-react";

import PageCard from "@/components/common/PageCard";
import { Input } from "@/components/ui/input";
import SearchVideosList from "@/features/youtube/components/videosList/SearchVideosList";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/search",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const [searchValue, setSearchValue] = useState("");
  const [searchQuery, setSearchQuery] = useState("");

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
    debouncedUpdateSearch(value);
  };

  const maxResults = 10;

  return (
    <PageCard>
      {searchQuery && searchQuery.length > 0 ? (
        <>
          <div className="mb-6 flex justify-center">
            <div className="relative w-full max-w-2xl">
              <Search className="absolute left-3 top-1/2 h-5 w-5 -translate-y-1/2 text-muted-foreground" />
              <Input
                type="text"
                placeholder="Search for videos..."
                value={searchValue}
                onChange={handleSearchChange}
                className="h-12 w-full pl-10 text-base"
              />
            </div>
          </div>
          <SearchVideosList searchQuery={searchQuery} maxResults={maxResults} />
        </>
      ) : (
        <div className="flex min-h-[50vh] flex-col items-center justify-center gap-8 text-center">
          <div className="flex flex-col items-center gap-4">
            <div className="flex size-20 items-center justify-center rounded-full bg-muted">
              <Play className="h-10 w-10 text-muted-foreground" />
            </div>
            <div className="space-y-2">
              <h2 className="text-2xl font-semibold tracking-tight">
                Search for Videos
              </h2>
              <p className="max-w-md text-muted-foreground">
                Search a specific video by name
              </p>
            </div>
          </div>
          <div className="relative w-full max-w-2xl">
            <Search className="absolute left-3 top-1/2 h-5 w-5 -translate-y-1/2 text-muted-foreground" />
            <Input
              type="text"
              placeholder="Search for videos..."
              value={searchValue}
              onChange={handleSearchChange}
              className="h-12 w-full pl-10 text-base"
            />
          </div>
        </div>
      )}
      <Outlet />
    </PageCard>
  );
}

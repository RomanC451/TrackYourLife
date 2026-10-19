import { createFileRoute } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import GuitarTabSongList from "@/features/guitar-tabs/components/GuitarTabSongList";
import { guitarTabsQueryOptions } from "@/features/guitar-tabs/queries/guitarTabsQueries";
import { useCustomQuery } from "@/hooks/useCustomQuery";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/guitar-tabs/favorites",
)({
  component: FavoritesPage,
});

function FavoritesPage() {
  const { query } = useCustomQuery(guitarTabsQueryOptions.favorites());

  return (
    <PageCard>
      <PageTitle title="Favorites" />
      {query.isPending && (
        <p className="py-8 text-center text-muted-foreground">
          Loading favorites...
        </p>
      )}
      {query.isError && (
        <p className="py-8 text-center text-muted-foreground">
          Favorites could not be loaded.
        </p>
      )}
      {query.data && (
        <GuitarTabSongList
          songs={query.data}
          emptyMessage="No favorite tabs yet."
        />
      )}
    </PageCard>
  );
}

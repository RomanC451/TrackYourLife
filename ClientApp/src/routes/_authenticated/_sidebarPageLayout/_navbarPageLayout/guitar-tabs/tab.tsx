import { createFileRoute } from "@tanstack/react-router";
import { z } from "zod";

import PageCard from "@/components/common/PageCard";
import { Button } from "@/components/ui/button";
import GuitarTabContentView from "@/features/guitar-tabs/components/GuitarTabContent";
import GuitarTabSaveActions from "@/features/guitar-tabs/components/GuitarTabSaveActions";
import GuitarTabYoutube from "@/features/guitar-tabs/components/GuitarTabYoutube";
import { guitarTabsQueryOptions } from "@/features/guitar-tabs/queries/guitarTabsQueries";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/guitar-tabs/tab",
)({
  validateSearch: z.object({
    url: z.string().catch(""),
  }),
  component: GuitarTabPage,
});

function GuitarTabPage() {
  const { url } = Route.useSearch();
  const goBack = useNavigateBackOrDefault({ to: "/guitar-tabs/search" });
  const {
    query: { data, isError, isPending },
  } = useCustomQuery({
    ...guitarTabsQueryOptions.tab(url),
    enabled: url.length > 0,
  });

  return (
    <PageCard>
      <div>
        <Button type="button" variant="ghost" onClick={goBack}>
          Back
        </Button>
      </div>
      {url.length === 0 && (
        <p className="py-12 text-center text-muted-foreground">
          Choose a tab from search.
        </p>
      )}
      {url.length > 0 && isPending && (
        <p className="py-12 text-center text-muted-foreground">Loading tab…</p>
      )}
      {url.length > 0 && isError && (
        <p className="py-12 text-center text-muted-foreground">
          This tab is not available to view.
        </p>
      )}
      {data && (
        <GuitarTabContentView
          tab={data}
          actions={<GuitarTabSaveActions tab={data} />}
          player={<GuitarTabYoutube tab={data} />}
        />
      )}
    </PageCard>
  );
}

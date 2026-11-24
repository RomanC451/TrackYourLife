import { createFileRoute } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import ApiTokensSettings from "@/features/settings/components/ApiTokensSettings";
import { personalAccessTokensQueryOptions } from "@/features/settings/queries/personalAccessTokensQuery";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/settings/api-tokens",
)({
  component: ApiTokensPage,
  loader: async () => {
    await queryClient.ensureQueryData(personalAccessTokensQueryOptions.list());
  },
});

function ApiTokensPage() {
  return (
    <PageCard>
      <div className="space-y-6">
        <div className="space-y-1 border-b border-border pb-3 pt-2">
          <PageTitle title="API tokens" />
          <p className="text-sm text-muted-foreground">
            Connect Home Assistant and other tools to TrackYourLife.
          </p>
        </div>
        <ApiTokensSettings />
      </div>
    </PageCard>
  );
}

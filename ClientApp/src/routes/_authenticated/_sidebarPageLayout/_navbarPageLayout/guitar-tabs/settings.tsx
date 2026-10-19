import { createFileRoute } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import GuitarTabSettings from "@/features/guitar-tabs/components/GuitarTabSettings";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/guitar-tabs/settings",
)({
  component: GuitarTabSettingsPage,
});

function GuitarTabSettingsPage() {
  return (
    <PageCard>
      <PageTitle title="Guitar Tabs settings" />
      <GuitarTabSettings />
    </PageCard>
  );
}

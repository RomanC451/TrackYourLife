import { createFileRoute } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import GuitarTuner from "@/features/guitar-tabs/components/GuitarTuner";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/guitar-tabs/tuner",
)({
  component: GuitarTunerPage,
});

function GuitarTunerPage() {
  return (
    <PageCard>
      <PageTitle title="Tuner" />
      <GuitarTuner />
    </PageCard>
  );
}

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { CancelSubscriptionSection } from "@/features/payments/components/CancelSubscriptionSection";
import { CurrentPlanSection } from "@/features/payments/components/CurrentPlanSection";

export default function BillingPage() {
  return (
    <PageCard>
      <PageTitle title="Billing" />
      <div className="space-y-8">
        <CurrentPlanSection />
        <CancelSubscriptionSection />
      </div>
    </PageCard>
  );
}

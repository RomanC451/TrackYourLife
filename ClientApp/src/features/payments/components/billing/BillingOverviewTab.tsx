import type { BillingSummaryDto } from "@/services/openapi";
import { BillingDetailsCard } from "./BillingDetailsCard";
import { CurrentPlanCard } from "./CurrentPlanCard";
import { PaymentMethodCard } from "./PaymentMethodCard";
import { PlanManagementCard } from "./PlanManagementCard";
import { SecurityFooterRow } from "./SecurityFooterRow";

export function BillingOverviewTab(data: BillingSummaryDto) {
  return (
    <div className="space-y-4">
      <div className="grid gap-4 lg:grid-cols-2">
        <CurrentPlanCard subscription={data.subscription} />
        <PaymentMethodCard paymentMethod={data.paymentMethod} />
      </div>

      <PlanManagementCard />
      <BillingDetailsCard billingDetails={data.billingDetails} />
      <SecurityFooterRow />
    </div>
  );
}

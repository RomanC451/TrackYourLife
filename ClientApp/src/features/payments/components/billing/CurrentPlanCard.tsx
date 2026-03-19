import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { BillingPortalButton } from "@/features/payments/components/BillingPortalButton";
import type { BillingSummaryDtoSubscription } from "@/services/openapi";
import { ShieldCheck } from "lucide-react";
import { CardHeaderWithIcon } from "./CardHeaderWithIcon";

type CurrentPlanCardProps = {
  subscription: BillingSummaryDtoSubscription | null | undefined;
};

export function CurrentPlanCard({ subscription }: CurrentPlanCardProps) {
  const planName = subscription?.planName ?? "Free";
  const amount =
    subscription?.unitAmount != null && subscription?.currency
      ? subscription.unitAmount.toLocaleString(undefined, {
          style: "currency",
          currency: subscription.currency,
        })
      : null;
  const interval = subscription?.interval ? `/${subscription.interval}` : "";
  const nextBilling = subscription?.currentPeriodEndUtc
    ? new Date(subscription.currentPeriodEndUtc).toLocaleDateString(undefined, {
        dateStyle: "long",
      })
    : null;
  const status = subscription?.status ?? "none";
  const statusBadge = (() => {
    if (status === "active") {
      return {
        label: "Active",
        className: "border-emerald-500/30 bg-emerald-500/15 text-emerald-400",
      };
    }
    if (status === "trialing") {
      return {
        label: "Trial",
        className: "border-sky-500/30 bg-sky-500/15 text-sky-400",
      };
    }
    if (status === "past_due" || status === "unpaid") {
      return {
        label: "Past due",
        className: "border-amber-500/30 bg-amber-500/15 text-amber-400",
      };
    }
    return { label: status, className: "border-muted-foreground/20" };
  })();

  return (
    <Card className="@container">
      <CardHeaderWithIcon
        icon={<ShieldCheck className="h-4 w-4" />}
        title="Current Plan"
        description="Your subscription details"
        right={
          subscription ? (
            <Badge variant="outline" className={statusBadge.className}>
              {statusBadge.label}
            </Badge>
          ) : null
        }
      />
      <CardContent className="space-y-4">
        <div className="space-y-1">
          <p className="text-3xl font-semibold tracking-tight">{planName}</p>
          {amount ? (
            <div className="flex items-baseline gap-1">
              <p className="text-2xl font-semibold">{amount}</p>
              <p className="text-sm text-muted-foreground">{interval}</p>
            </div>
          ) : (
            <p className="text-sm text-muted-foreground">No active subscription.</p>
          )}
        </div>

        <Separator />

        <div className="text-sm text-muted-foreground">
          {nextBilling ? (
            <p>
              Next billing: <span className="text-foreground">{nextBilling}</span>
            </p>
          ) : (
            <p>Upgrade to Pro to unlock all features.</p>
          )}
        </div>

        <div className="flex flex-col gap-2 @lg:flex-row">
          <BillingPortalButton className="@lg:flex-1">Manage Subscription</BillingPortalButton>
          <Button variant="outline" className="@lg:flex-1" asChild>
            <a href="/upgrade">Change Plan</a>
          </Button>
        </div>

        <p className="text-xs text-muted-foreground">
          Your subscription renews automatically. You can cancel anytime.
        </p>
      </CardContent>
    </Card>
  );
}

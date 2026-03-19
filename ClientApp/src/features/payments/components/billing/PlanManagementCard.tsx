import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { BillingPortalButton } from "@/features/payments/components/BillingPortalButton";
import { ArrowDownRight, ArrowUpRight, ExternalLink, ShieldCheck, Tag } from "lucide-react";
import { CardHeaderWithIcon } from "./CardHeaderWithIcon";

export function PlanManagementCard() {
  return (
    <Card className="@container">
      <CardHeaderWithIcon
        icon={<ShieldCheck className="h-4 w-4" />}
        title="Plan Management"
        description="Change, upgrade, or cancel your subscription"
      />
      <CardContent className="space-y-3">
        <div className="grid gap-2 @lg:grid-cols-2">
          <Button variant="outline" className="w-full justify-start gap-2" asChild>
            <a href="/upgrade">
              <ArrowUpRight className="h-4 w-4" />
              Upgrade Plan
            </a>
          </Button>

          <BillingPortalButton className="w-full justify-start gap-2">
            <ArrowDownRight className="h-4 w-4" />
            Downgrade Plan
          </BillingPortalButton>

          <BillingPortalButton className="w-full justify-start gap-2">
            <Tag className="h-4 w-4" />
            Apply Promo Code
          </BillingPortalButton>

          <BillingPortalButton className="w-full justify-start gap-2 border-destructive/40 text-destructive hover:text-destructive">
            <ExternalLink className="h-4 w-4" />
            Cancel Subscription
          </BillingPortalButton>
        </div>

        <Separator />

        <p className="text-xs text-muted-foreground">
          Changes take effect on your next billing cycle. When canceling, you retain access until the end
          of the current period.
        </p>
      </CardContent>
    </Card>
  );
}

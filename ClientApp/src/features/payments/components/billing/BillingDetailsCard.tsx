import { Card, CardContent } from "@/components/ui/card";
import { BillingPortalButton } from "@/features/payments/components/BillingPortalButton";
import type { BillingSummaryDtoBillingDetails } from "@/services/openapi";
import { Building2 } from "lucide-react";
import { CardHeaderWithIcon } from "./CardHeaderWithIcon";

type BillingDetailsCardProps = {
  billingDetails: BillingSummaryDtoBillingDetails | null | undefined;
};

export function BillingDetailsCard({ billingDetails }: BillingDetailsCardProps) {
  const address = billingDetails?.billingAddress;
  const addressLines = [
    address?.line1,
    address?.line2,
    [address?.city, address?.state, address?.postalCode].filter(Boolean).join(", ").trim(),
    address?.country,
  ].filter((value) => typeof value === "string" && value.trim().length > 0);

  return (
    <Card>
      <CardHeaderWithIcon
        icon={<Building2 className="h-4 w-4" />}
        title="Billing Details"
        description="Tax and billing address information"
        right={
          <BillingPortalButton className="shrink-0" urlRoute="/customer/update">
            Edit
          </BillingPortalButton>
        }
      />
      <CardContent className="grid gap-4 sm:grid-cols-2">
        <div className="space-y-1">
          <p className="text-sm font-medium">Billing Address</p>
          {addressLines.length > 0 ? (
            <div className="text-sm text-muted-foreground">
              {addressLines.map((line) => (
                <p key={line}>{line}</p>
              ))}
            </div>
          ) : (
            <p className="text-sm text-muted-foreground">Managed in Stripe billing portal.</p>
          )}
        </div>
        <div className="space-y-3">
          <div className="space-y-1">
            <p className="text-sm font-medium">Name</p>
            <p className="text-sm text-muted-foreground">
              {billingDetails?.companyName?.trim()
                ? billingDetails.companyName
                : "Managed in Stripe billing portal."}
            </p>
          </div>
          <div className="space-y-1">
            <p className="text-sm font-medium">VAT / Tax ID</p>
            <p className="text-sm text-muted-foreground">
              {billingDetails?.vatId?.trim()
                ? billingDetails.vatId
                : "Managed in Stripe billing portal."}
            </p>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

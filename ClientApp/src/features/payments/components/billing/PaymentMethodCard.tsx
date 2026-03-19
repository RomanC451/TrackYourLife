import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { BillingPortalButton } from "@/features/payments/components/BillingPortalButton";
import type { BillingSummaryDtoPaymentMethod } from "@/services/openapi";
import { CreditCard } from "lucide-react";
import { CardHeaderWithIcon } from "./CardHeaderWithIcon";

type PaymentMethodCardProps = {
  paymentMethod: BillingSummaryDtoPaymentMethod | null | undefined;
};

export function PaymentMethodCard({ paymentMethod }: PaymentMethodCardProps) {
  const expMonth = paymentMethod?.expMonth ? String(paymentMethod.expMonth).padStart(2, "0") : null;
  const expYear = paymentMethod?.expYear ? String(paymentMethod.expYear) : null;

  return (
    <Card>
      <CardHeaderWithIcon
        icon={<CreditCard className="h-4 w-4" />}
        title="Payment Method"
        description="Your default payment method"
      />
      <CardContent className="space-y-3">
        <div className="rounded-xl border bg-muted/20 p-3">
          <div className="flex items-center gap-3">
            <div className="flex h-11 w-11 items-center justify-center rounded-lg bg-muted">
              <CreditCard className="h-4 w-4 text-muted-foreground" />
            </div>

            <div className="min-w-0 flex-1">
              {paymentMethod ? (
                <>
                  <p className="truncate text-sm font-semibold text-foreground">
                    {paymentMethod.brand.toUpperCase()} ending in {paymentMethod.last4}
                  </p>
                  <p className="truncate text-xs text-muted-foreground">
                    {expMonth && expYear ? `Expires ${expMonth}/${expYear}` : "Expires —"}
                    {paymentMethod.billingName ? ` · ${paymentMethod.billingName}` : ""}
                  </p>
                </>
              ) : (
                <>
                  <p className="text-sm font-semibold text-foreground">No payment method on file</p>
                  <p className="text-xs text-muted-foreground">
                    Add a payment method in the billing portal to manage your subscription.
                  </p>
                </>
              )}
            </div>

            {paymentMethod?.isExpiringSoon ? (
              <Badge
                variant="outline"
                className="shrink-0 border-amber-500/30 bg-amber-500/15 text-amber-400"
              >
                Expiring soon
              </Badge>
            ) : null}
          </div>
        </div>

        <BillingPortalButton className="w-full" urlRoute="/payment-methods">
          {paymentMethod ? "Update Payment Method" : "Add Payment Method"}
        </BillingPortalButton>
      </CardContent>
    </Card>
  );
}

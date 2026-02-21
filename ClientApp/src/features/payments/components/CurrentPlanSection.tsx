import { Crown } from "lucide-react";
import { Link } from "@tanstack/react-router";

import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";
import { BillingPortalButton } from "./BillingPortalButton";

export function CurrentPlanSection() {
  const { userData, isPro } = useAuthenticationContext();

  const subscriptionEndsAt = userData?.subscriptionEndsAtUtc
    ? new Date(userData.subscriptionEndsAtUtc)
    : null;
  const isFuture = subscriptionEndsAt ? subscriptionEndsAt > new Date() : false;
  const renewalCanceled = Boolean(userData?.subscriptionCancelAtPeriodEnd);

  let periodLabel: string | null = null;
  if (subscriptionEndsAt) {
    if (renewalCanceled && isFuture) {
      periodLabel = "Access until ";
    } else if (isFuture) {
      periodLabel = "Renews ";
    } else {
      periodLabel = "Ends ";
    }
  }

  return (
    <section>
      <h2 className="text-lg font-medium mb-4">Current plan</h2>
      {isPro ? (
        <div className="rounded-lg border bg-card p-5 space-y-4">
          {renewalCanceled && isFuture && (
            <Alert variant="default" className="border-amber-500/50 bg-amber-500/10">
              <AlertDescription>
                Your subscription renewal has been canceled. You’ll keep Pro access until{" "}
                {subscriptionEndsAt?.toLocaleDateString(undefined, {
                  dateStyle: "long",
                })}
                .
              </AlertDescription>
            </Alert>
          )}
          <div className="flex items-start justify-between gap-4">
            <div className="flex items-center gap-3">
              <div className="rounded-full bg-primary/10 p-2">
                <Crown className="h-5 w-5 text-primary" />
              </div>
              <div>
                <p className="font-semibold text-card-foreground">Pro</p>
                <p className="text-sm text-muted-foreground">
                  {periodLabel === null ? (
                    "Active subscription"
                  ) : (
                    <>
                      {periodLabel}
                      {subscriptionEndsAt?.toLocaleDateString(undefined, {
                        dateStyle: "long",
                      })}
                    </>
                  )}
                </p>
              </div>
            </div>
            <BillingPortalButton className="shrink-0">
              Manage or cancel subscription
            </BillingPortalButton>
          </div>
        </div>
      ) : (
        <div className="rounded-lg border bg-muted/30 p-5">
          <p className="font-medium text-card-foreground">Free</p>
          <p className="text-sm text-muted-foreground mb-4">
            Upgrade to Pro to unlock all features.
          </p>
          <div className="flex gap-2">
            <Link to="/upgrade">
              <Button variant="default"> Upgrade to Pro</Button>
            </Link>
          </div>
        </div>
      )}
    </section>
  );
}

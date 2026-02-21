import { AlertCircle } from "lucide-react";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";
import { BillingPortalButton } from "./BillingPortalButton";

export function CancelSubscriptionSection() {
  const { isPro } = useAuthenticationContext();

  if (!isPro) return null;

  return (
    <section>
      <h2 className="text-lg font-medium mb-4">Cancel subscription</h2>
      <Alert variant="default" className="border-muted-foreground/20">
        <AlertCircle className="h-4 w-4" />
        <AlertTitle>Cancel or change your plan</AlertTitle>
        <AlertDescription>
          <p className="mb-3">
            To cancel your subscription or update your payment method, open the
            billing portal. You can cancel at any time; you'll keep Pro until the
            end of your current billing period.
          </p>
          <BillingPortalButton className="mt-2">
            Open billing portal to cancel
          </BillingPortalButton>
        </AlertDescription>
      </Alert>
    </section>
  );
}

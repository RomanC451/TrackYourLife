import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { BillingPortalButton } from "@/features/payments/components/BillingPortalButton";

export function UsageTab() {
  return (
    <Card>
      <CardHeader className="space-y-1">
        <CardTitle className="text-base">Usage</CardTitle>
        <CardDescription>Usage metrics are not tracked in TrackYourLife yet.</CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <p className="text-sm text-muted-foreground">
          For now, use the billing portal for plan limits and usage-related billing info.
        </p>
        <BillingPortalButton className="sm:shrink-0">Open billing portal</BillingPortalButton>
      </CardContent>
    </Card>
  );
}

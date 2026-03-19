import { Card, CardContent } from "@/components/ui/card";
import { ExternalLink, Lock, ShieldCheck } from "lucide-react";

export function SecurityFooterRow() {
  return (
    <Card>
      <CardContent className="flex flex-col gap-3 py-4 text-sm text-muted-foreground sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-2">
          <Lock className="h-4 w-4" />
          Payments securely processed by Stripe
        </div>
        <div className="flex items-center gap-2">
          <ShieldCheck className="h-4 w-4" />
          We do not store your card details
        </div>
        <div className="flex items-center gap-2">
          <ExternalLink className="h-4 w-4" />
          Need help with billing?
        </div>
      </CardContent>
    </Card>
  );
}

import { Button } from "@/components/ui/button";
import { ExternalLink } from "lucide-react";
import { billingPortalUrlQueryOptions } from "../queries/useGetBillingPortalUrlQuery";
import { useSuspenseQuery } from "@tanstack/react-query";

type BillingPortalButtonProps = {
  children?: React.ReactNode;
  className?: string;
};

export function BillingPortalButton({
  children,
  className,
}: BillingPortalButtonProps) {
  const { data: url, isPending } = useSuspenseQuery(
    billingPortalUrlQueryOptions.byReturnPath("/billing"),
  );

  const handleOpenPortal = async () => {
    globalThis.location.href = url;
  };

  return (
    <Button
      onClick={handleOpenPortal}
      disabled={isPending}
      variant="outline"
      className={className}
    >
      {children ?? "Manage Billing"}
      <ExternalLink className="ml-2 h-4 w-4" />
    </Button>
  );
}

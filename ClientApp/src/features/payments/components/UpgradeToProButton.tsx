import { Button } from "@/components/ui/button";
import { useCreateCheckoutSessionMutation } from "../mutations/useCreateCheckoutSessionMutation";
import { toast } from "sonner";
import { StatusCodes } from "http-status-codes";
import { apiPaymentsErrors } from "../data/apiPaymentsErrors";

type UpgradeToProButtonProps = {
  priceId: string;
  successUrl?: string;
  cancelUrl?: string;
  children?: React.ReactNode;
  className?: string;
};

export function UpgradeToProButton({
  priceId,
  successUrl = `${globalThis.location.origin}/subscription-success`,
  cancelUrl = `${globalThis.location.origin}/settings?upgrade=cancelled`,
  children,
  className,
}: UpgradeToProButtonProps) {
  const mutation = useCreateCheckoutSessionMutation({
    errorHandlers: {
      [StatusCodes.BAD_REQUEST]: {
        [apiPaymentsErrors.Checkout.AlreadySubscribed]: () => {
          toast.error("You are already subscribed to the pro plan");
        },
      },
    },
  });

  const handleUpgrade = () => {
    mutation.mutate(
      { successUrl, cancelUrl, priceId },
      {
        onSuccess: (url) => {
          if (url) {
            globalThis.location.href = url;
          }
        },
      },
    );
  };

  return (
    <Button
      onClick={handleUpgrade}
      disabled={mutation.isPending}
      className={className}
    >
      {mutation.isPending ? "Redirecting..." : children ?? "Upgrade to Pro"}
    </Button>
  );
}

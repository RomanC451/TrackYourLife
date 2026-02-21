import { Link } from "@tanstack/react-router";
import { useEffect } from "react";
import { CheckCircle2, Crown } from "lucide-react";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";

export default function SubscriptionSuccessPage() {
  const { refetchUser } = useAuthenticationContext();

  useEffect(() => {
    refetchUser();
  }, [refetchUser]);

  return (
    <PageCard>
      <div className="flex flex-col items-center justify-center py-12 text-center">
        <div className="mb-6 rounded-full bg-primary/10 p-5">
          <CheckCircle2 className="h-16 w-16 text-primary" />
        </div>
        <PageTitle title="You're Pro!" />
        <p className="mb-2 text-lg text-muted-foreground">
          Thank you for upgrading. Your subscription is now active.
        </p>
        <p className="mb-8 text-sm text-muted-foreground">
          You have access to all Pro features. Manage your subscription anytime in settings.
        </p>
        <div className="flex flex-wrap justify-center gap-3">
          <Link to="/billing">
            <Button variant="default" className="gap-2">
              <Crown className="h-4 w-4" />
              Manage subscription
            </Button>
          </Link>
          <Link to="/home">
            <Button variant="outline">Go to Home</Button>
          </Link>
        </div>
      </div>
    </PageCard>
  );
}

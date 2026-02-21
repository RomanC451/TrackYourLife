import { Link } from "@tanstack/react-router";
import { Check, Crown, Sparkles } from "lucide-react";

import PageCard from "@/components/common/PageCard";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";
import { UpgradeToProButton } from "@/features/payments/components/UpgradeToProButton";

const proFeatures = [
  "Unlimited exercises and workouts",
  "Advanced analytics and insights",
  "Priority customer support",
  "Export your data",
  "Custom workout templates",
  "Nutrition tracking integration",
  "Ad-free experience",
  "Early access to new features",
];

export default function UpgradePage() {
  const { isPro } = useAuthenticationContext();

  if (isPro) {
    return (
      <PageCard>
        <div className="flex flex-col items-center justify-center py-12 text-center">
          <div className="mb-4 rounded-full bg-primary/10 p-4">
            <Crown className="h-12 w-12 text-primary" />
          </div>
          <h2 className="mb-2 text-2xl font-bold">You're already a Pro member!</h2>
          <p className="mb-6 text-muted-foreground">
            Thank you for your support. Enjoy all the Pro features.
          </p>
        </div>
      </PageCard>
    );
  }

  return (
    <PageCard>
      <div className="space-y-8">
        {/* Hero Section */}
        <div className="text-center space-y-4">
          <div className="flex justify-center">
            <div className="rounded-full bg-primary/10 p-4">
              <Sparkles className="h-12 w-12 text-primary" />
            </div>
          </div>
          <h1 className="text-4xl font-bold">Unlock Your Full Potential</h1>
          <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
            Get access to all Pro features and take your fitness journey to the next level
          </p>
        </div>

        {/* Pricing Card */}
        <div className="flex justify-center">
          <Card className="w-full max-w-md border-2 border-primary">
            <CardHeader className="text-center">
              <CardTitle className="text-2xl">Pro Plan</CardTitle>
              <CardDescription className="text-lg">
                Everything you need to succeed
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              {/* Features List */}
              <div className="space-y-3">
                {proFeatures.map((feature) => (
                  <div key={feature} className="flex items-start gap-3">
                    <Check className="h-5 w-5 text-primary mt-0.5 shrink-0" />
                    <span className="text-sm">{feature}</span>
                  </div>
                ))}
              </div>
            </CardContent>
            <CardFooter className="flex flex-col gap-3">
              <UpgradeToProButton
                priceId={import.meta.env.VITE_STRIPE_PRICE_ID_MONTHLY ?? "price_monthly"}
                successUrl={`${globalThis.location.origin}/subscription-success`}
                cancelUrl={`${globalThis.location.origin}/upgrade?upgrade=cancelled`}
                className="w-full"
              >
                Upgrade to Pro
              </UpgradeToProButton>
              <p className="text-xs text-center text-muted-foreground">
                Secure payment powered by Stripe. Cancel anytime.
              </p>
            </CardFooter>
          </Card>
        </div>

        {/* Additional Info */}
        <div className="rounded-lg border bg-muted/50 p-6 text-center">
          <h3 className="font-semibold mb-2">Questions?</h3>
          <p className="text-sm text-muted-foreground">
            Need help deciding? Contact our support team or check out our{" "}
            <Link to="/billing" className="text-primary hover:underline">
              billing page
            </Link>{" "}
            for more information.
          </p>
        </div>
      </div>
    </PageCard>
  );
}

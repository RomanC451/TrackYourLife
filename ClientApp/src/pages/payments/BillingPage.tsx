"use no memo";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import {
  BillingInvoicesTab,
  BillingOverviewTab,
  UsageTab,
} from "@/features/payments/components/billing/index";
import { billingSummaryQueryOptions } from "@/features/payments/queries/useBillingSummaryQuery";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useSuspenseQuery } from "@tanstack/react-query";

export default function BillingPage() {
  const { data } = useSuspenseQuery(billingSummaryQueryOptions.get());

  return (
    <PageCard>
      <div className="space-y-6">
        <div className="space-y-1 border-b border-border pb-3 pt-2">
          <PageTitle title="Billing" />
          <p className="text-sm text-muted-foreground">
            Manage your subscription, payment methods, and invoices.
          </p>
        </div>

        <Tabs defaultValue="overview">
          <TabsList className="w-full justify-start">
            <TabsTrigger value="overview">Overview</TabsTrigger>
            <TabsTrigger value="invoices">Invoices</TabsTrigger>
            <TabsTrigger value="usage">Usage</TabsTrigger>
          </TabsList>

          <TabsContent value="overview" className="mt-4">
            <BillingOverviewTab {...data} />
          </TabsContent>

          <TabsContent value="invoices" className="mt-4">
            <BillingInvoicesTab invoices={data.invoices} />
          </TabsContent>

          <TabsContent value="usage" className="mt-4">
            <UsageTab />
          </TabsContent>
        </Tabs>
      </div>
    </PageCard>
  );
}

import type { InvoiceSummaryDto } from "@/services/openapi";
import { InvoiceHistoryCard } from "./InvoiceHistoryCard";

type BillingInvoicesTabProps = {
  invoices: InvoiceSummaryDto[];
};

export function BillingInvoicesTab({ invoices }: BillingInvoicesTabProps) {
  return <InvoiceHistoryCard invoices={invoices} />;
}

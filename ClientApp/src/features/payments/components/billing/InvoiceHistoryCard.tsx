import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { BillingPortalButton } from "@/features/payments/components/BillingPortalButton";
import type { InvoiceSummaryDto } from "@/services/openapi";
import { Download, Eye, FileText } from "lucide-react";

type InvoiceHistoryCardProps = {
  invoices: InvoiceSummaryDto[];
};

export function InvoiceHistoryCard({ invoices }: InvoiceHistoryCardProps) {
  return (
    <Card>
      <CardHeader className="flex flex-col gap-2 sm:flex-row sm:items-start sm:justify-between">
        <div className="space-y-1">
          <CardTitle className="flex items-center gap-2">
            <FileText className="h-4 w-4" />
            Invoice history
          </CardTitle>
          <CardDescription>Your recent billing transactions.</CardDescription>
        </div>

        <BillingPortalButton className="sm:shrink-0">View in Stripe</BillingPortalButton>
      </CardHeader>
      <CardContent>
        {invoices.length === 0 ? (
          <div className="rounded-md border bg-muted/30 p-4 text-sm text-muted-foreground">
            No invoices found yet. If you have an active subscription, you can view and download invoices in
            the billing portal.
          </div>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Date</TableHead>
                  <TableHead>Amount</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {invoices.map((invoice) => (
                  <TableRow
                    key={invoice.id}
                    className={invoice.status === "open" ? "bg-amber-500/5 hover:bg-amber-500/10" : undefined}
                  >
                    <TableCell className="whitespace-nowrap">
                      {new Date(invoice.createdUtc).toLocaleDateString(undefined, { dateStyle: "medium" })}
                    </TableCell>
                    <TableCell className="whitespace-nowrap">
                      {invoice.amount.toLocaleString(undefined, {
                        style: "currency",
                        currency: invoice.currency,
                      })}
                    </TableCell>
                    <TableCell>
                      <Badge
                        variant="outline"
                        className={
                          invoice.status === "open"
                            ? "border-amber-500/30 bg-amber-500/15 text-amber-400"
                            : "border-muted-foreground/20"
                        }
                      >
                        {invoice.status}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-right">
                      <div className="flex justify-end gap-2">
                        {invoice.invoicePdf ? (
                          <Button variant="outline" size="sm" asChild>
                            <a href={invoice.invoicePdf} target="_blank" rel="noreferrer">
                              <Download className="mr-2 h-4 w-4" />
                              Download
                            </a>
                          </Button>
                        ) : (
                          <Button variant="outline" size="sm" disabled>
                            <Download className="mr-2 h-4 w-4" />
                            Download
                          </Button>
                        )}

                        {invoice.hostedInvoiceUrl ? (
                          <Button variant="outline" size="sm" asChild>
                            <a href={invoice.hostedInvoiceUrl} target="_blank" rel="noreferrer">
                              <Eye className="mr-2 h-4 w-4" />
                              {invoice.status === "open" ? "Pay invoice" : "View invoice"}
                            </a>
                          </Button>
                        ) : null}
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </CardContent>
    </Card>
  );
}

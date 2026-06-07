import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { billingSummary } from "@/features/payments/__tests__/fixtures";

vi.mock("../CurrentPlanCard", () => ({
  CurrentPlanCard: () => <div data-testid="current-plan-card" />,
}));
vi.mock("../PaymentMethodCard", () => ({
  PaymentMethodCard: () => <div data-testid="payment-method-card" />,
}));
vi.mock("../PlanManagementCard", () => ({
  PlanManagementCard: () => <div data-testid="plan-management-card" />,
}));
vi.mock("../BillingDetailsCard", () => ({
  BillingDetailsCard: () => <div data-testid="billing-details-card" />,
}));
vi.mock("../SecurityFooterRow", () => ({
  SecurityFooterRow: () => <div data-testid="security-footer-row" />,
}));

import { BillingOverviewTab } from "../BillingOverviewTab";

describe("BillingOverviewTab", () => {
  it("composes billing overview sections", () => {
    render(<BillingOverviewTab {...billingSummary()} />);

    expect(screen.getByTestId("current-plan-card")).toBeInTheDocument();
    expect(screen.getByTestId("payment-method-card")).toBeInTheDocument();
    expect(screen.getByTestId("plan-management-card")).toBeInTheDocument();
    expect(screen.getByTestId("billing-details-card")).toBeInTheDocument();
    expect(screen.getByTestId("security-footer-row")).toBeInTheDocument();
  });
});

import { describe, expect, it } from "vitest";

import {
  BillingInvoicesTab,
  BillingOverviewTab,
  UsageTab,
} from "../index";

describe("billing index exports", () => {
  it("re-exports billing tab components", () => {
    expect(BillingInvoicesTab).toBeTypeOf("function");
    expect(BillingOverviewTab).toBeTypeOf("function");
    expect(UsageTab).toBeTypeOf("function");
  });
});

import { beforeEach, describe, expect, it, vi } from "vitest";

import { billingSummary } from "@/features/payments/__tests__/fixtures";
import { queryClient } from "@/queryClient";
import { createQueryFnContext } from "@/test/queryFnContext";

const { mockGetBillingSummary } = vi.hoisted(() => ({
  mockGetBillingSummary: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockPaymentsApi {
    getBillingSummary = mockGetBillingSummary;
  }
  return { ...actual, PaymentsApi: MockPaymentsApi };
});

import {
  billingSummaryQueryKeys,
  billingSummaryQueryOptions,
} from "../useBillingSummaryQuery";

describe("useBillingSummaryQuery", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("defines stable query keys", () => {
    expect(billingSummaryQueryKeys.all).toEqual(["billingSummary"]);
  });

  it("fetches billing summary from the API", async () => {
    const summary = billingSummary();
    mockGetBillingSummary.mockResolvedValue({ data: summary });

    const options = billingSummaryQueryOptions.get();
    const result = await options.queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: options.queryKey,
      }),
    );
    expect(options.queryKey).toEqual(["billingSummary"]);
    expect(mockGetBillingSummary).toHaveBeenCalledOnce();
    expect(result).toEqual(summary);
  });
});

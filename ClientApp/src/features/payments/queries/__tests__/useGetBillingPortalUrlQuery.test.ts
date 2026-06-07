import { beforeEach, describe, expect, it, vi } from "vitest";

import { queryClient } from "@/queryClient";
import { createQueryFnContext } from "@/test/queryFnContext";

const { mockGetBillingPortalUrl } = vi.hoisted(() => ({
  mockGetBillingPortalUrl: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockPaymentsApi {
    getBillingPortalUrl = mockGetBillingPortalUrl;
  }
  return { ...actual, PaymentsApi: MockPaymentsApi };
});

import {
  billingPortalUrlQueryKeys,
  billingPortalUrlQueryOptions,
} from "../useGetBillingPortalUrlQuery";

describe("useGetBillingPortalUrlQuery", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("builds query keys by return path", () => {
    expect(billingPortalUrlQueryKeys.all).toEqual(["billingPortalUrl"]);
    expect(billingPortalUrlQueryKeys.byReturnPath("/billing")).toEqual([
      "billingPortalUrl",
      "/billing",
    ]);
  });

  it("fetches the billing portal URL for a return path", async () => {
    mockGetBillingPortalUrl.mockResolvedValue({
      data: "https://billing.stripe.com/portal",
    });

    const options = billingPortalUrlQueryOptions.byReturnPath("/billing");
    const result = await options.queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: options.queryKey,
      }),
    );
    expect(options.queryKey).toEqual(["billingPortalUrl", "/billing"]);
    expect(mockGetBillingPortalUrl).toHaveBeenCalledWith(
      `${globalThis.location.origin}/billing`,
    );
    expect(result).toBe("https://billing.stripe.com/portal");
  });
});

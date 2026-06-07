import { act, renderHook } from "@testing-library/react";
import { StatusCodes } from "http-status-codes";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockCreateCheckoutSession, mockHandleApiError } = vi.hoisted(() => ({
  mockCreateCheckoutSession: vi.fn(),
  mockHandleApiError: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockPaymentsApi {
    createCheckoutSession = mockCreateCheckoutSession;
  }
  return { ...actual, PaymentsApi: MockPaymentsApi };
});

vi.mock("@/services/openapi/handleApiError", () => ({
  handleApiError: mockHandleApiError,
}));

import { useCreateCheckoutSessionMutation } from "../useCreateCheckoutSessionMutation";

describe("useCreateCheckoutSessionMutation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockCreateCheckoutSession.mockResolvedValue({
      data: "https://checkout.stripe.com/session",
    });
  });

  it("creates a checkout session", async () => {
    const { result } = renderHook(() => useCreateCheckoutSessionMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        priceId: "price_monthly",
        successUrl: "https://app.test/success",
        cancelUrl: "https://app.test/cancel",
      });
    });

    expect(mockCreateCheckoutSession).toHaveBeenCalledWith({
      priceId: "price_monthly",
      successUrl: "https://app.test/success",
      cancelUrl: "https://app.test/cancel",
    });
  });

  it("delegates API errors to handleApiError", async () => {
    const error = {
      response: { status: StatusCodes.BAD_REQUEST, data: { type: "Checkout.AlreadySubscribed" } },
    };
    mockCreateCheckoutSession.mockRejectedValue(error);

    const errorHandlers = { [StatusCodes.BAD_REQUEST]: {} };
    const { result } = renderHook(
      () => useCreateCheckoutSessionMutation({ errorHandlers }),
      { wrapper: createQueryClientWrapper() },
    );

    await act(async () => {
      await result.current
        .mutateAsync({
          priceId: "price_monthly",
          successUrl: "https://app.test/success",
          cancelUrl: "https://app.test/cancel",
        })
        .catch(() => undefined);
    });

    expect(mockHandleApiError).toHaveBeenCalledWith({
      error,
      errorHandlers,
    });
  });
});

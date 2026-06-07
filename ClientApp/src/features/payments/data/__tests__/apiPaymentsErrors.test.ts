import { describe, expect, it } from "vitest";

import { apiPaymentsErrors } from "../apiPaymentsErrors";

describe("apiPaymentsErrors", () => {
  it("exposes checkout error codes", () => {
    expect(apiPaymentsErrors.Checkout.AlreadySubscribed).toBe(
      "Checkout.AlreadySubscribed",
    );
  });
});

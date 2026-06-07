import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { SecurityFooterRow } from "../SecurityFooterRow";

describe("SecurityFooterRow", () => {
  it("renders billing security messaging", () => {
    render(<SecurityFooterRow />);

    expect(screen.getByText(/Payments securely processed by Stripe/i)).toBeInTheDocument();
    expect(screen.getByText(/We do not store your card details/i)).toBeInTheDocument();
    expect(screen.getByText(/Need help with billing/i)).toBeInTheDocument();
  });
});

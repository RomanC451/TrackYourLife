import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { CardHeaderWithIcon } from "../CardHeaderWithIcon";

describe("CardHeaderWithIcon", () => {
  it("renders title, description, icon, and optional action", () => {
    render(
      <CardHeaderWithIcon
        icon={<span data-testid="icon">*</span>}
        title="Billing Details"
        description="Tax and billing address information"
        right={<button type="button">Edit</button>}
      />,
    );

    expect(screen.getByText("Billing Details")).toBeInTheDocument();
    expect(
      screen.getByText("Tax and billing address information"),
    ).toBeInTheDocument();
    expect(screen.getByTestId("icon")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Edit" })).toBeInTheDocument();
  });
});

import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { ChartLoadingOverlay } from "../ChartLoadingOverlay";

describe("ChartLoadingOverlay", () => {
  it("renders nothing when show is false", () => {
    const { container } = render(<ChartLoadingOverlay show={false} />);

    expect(container).toBeEmptyDOMElement();
  });

  it("renders a busy overlay with a spinner when show is true", () => {
    render(<ChartLoadingOverlay show />);

    expect(screen.getByRole("status")).toBeInTheDocument();
    expect(screen.getByText("Loading...")).toBeInTheDocument();
  });
});

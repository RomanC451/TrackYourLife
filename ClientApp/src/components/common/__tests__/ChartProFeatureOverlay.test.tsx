import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { ChartProFeatureOverlay } from "../ChartProFeatureOverlay";

vi.mock("@tanstack/react-router", () => ({
  Link: ({ children, to }: { children: React.ReactNode; to: string }) => (
    <a href={to}>{children}</a>
  ),
}));

describe("ChartProFeatureOverlay", () => {
  it("renders nothing when show is false", () => {
    const { container } = render(
      <ChartProFeatureOverlay
        show={false}
        title="Pro chart"
        description="Upgrade to unlock"
      />,
    );

    expect(container).toBeEmptyDOMElement();
  });

  it("renders the upsell content and upgrade link", () => {
    render(
      <ChartProFeatureOverlay
        show
        title="Pro chart"
        description="Upgrade to unlock"
      />,
    );

    expect(screen.getByText("Pro chart")).toBeInTheDocument();
    expect(screen.getByText("Upgrade to unlock")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Upgrade to Pro" })).toHaveAttribute(
      "href",
      "/upgrade",
    );
  });
});

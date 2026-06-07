import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import PageCard from "../PageCard";

describe("PageCard", () => {
  it("renders children inside the card container", () => {
    render(
      <PageCard>
        <p>Page content</p>
      </PageCard>,
    );

    expect(screen.getByText("Page content")).toBeInTheDocument();
  });
});

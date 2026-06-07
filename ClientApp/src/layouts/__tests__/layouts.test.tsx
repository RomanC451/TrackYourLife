import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import AbsoluteCenterChildrenLayout from "../AbsoluteCenterChildrenLayout";
import FullSizeCenteredLayout from "../FullSizeCenteredLayout";
import FullSizeLayout from "../FullSizeLayout";

describe("layouts", () => {
  it("AbsoluteCenterChildrenLayout centers children", () => {
    render(
      <AbsoluteCenterChildrenLayout>
        <span>Centered</span>
      </AbsoluteCenterChildrenLayout>,
    );

    expect(screen.getByText("Centered")).toBeInTheDocument();
  });

  it("FullSizeCenteredLayout renders children", () => {
    render(
      <FullSizeCenteredLayout>
        <span>Full centered</span>
      </FullSizeCenteredLayout>,
    );

    expect(screen.getByText("Full centered")).toBeInTheDocument();
  });

  it("FullSizeLayout renders children", () => {
    render(
      <FullSizeLayout>
        <span>Full size</span>
      </FullSizeLayout>,
    );

    expect(screen.getByText("Full size")).toBeInTheDocument();
  });
});

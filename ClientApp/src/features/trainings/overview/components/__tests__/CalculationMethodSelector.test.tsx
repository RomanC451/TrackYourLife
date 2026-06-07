import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { CalculationMethodSelector } from "../CalculationMethodSelector";

vi.mock("@/components/ui/hybrid-tooltip", () => ({
  HybridTooltip: ({ children }: { children: React.ReactNode }) => children,
  HybridTooltipTrigger: ({ children }: { children: React.ReactNode }) => children,
  HybridTooltipContent: ({ children }: { children: React.ReactNode }) => children,
  TouchProvider: ({ children }: { children: React.ReactNode }) => children,
}));

describe("CalculationMethodSelector", () => {
  it("renders the current method and forwards changes", () => {
    const onValueChange = vi.fn();
    render(
      <CalculationMethodSelector
        value="Sequential"
        onValueChange={onValueChange}
      />,
    );

    expect(screen.getByText("Calculation Method:")).toBeInTheDocument();
    expect(screen.getByRole("combobox")).toBeInTheDocument();
  });

  it("disables the select while loading", () => {
    render(
      <CalculationMethodSelector
        value="FirstVsLast"
        onValueChange={vi.fn()}
        loading
      />,
    );

    expect(screen.getByRole("combobox")).toBeDisabled();
  });
});

import { fireEvent, render, screen } from "@testing-library/react";
import type { ReactNode } from "react";
import { describe, expect, it, vi } from "vitest";

import FitnessCalculator from "../FitnessCalculator";

const mockScreenSize = vi.hoisted(() => ({ width: 1200, height: 800 }));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({
    screenSize: mockScreenSize,
  }),
}));

vi.mock("../CalculateNutritionGoalsForm", () => ({
  default: ({ onSuccess }: { onSuccess: () => void }) => (
    <button type="button" onClick={onSuccess}>
      Run calculator
    </button>
  ),
}));

vi.mock("../CalculateNutritionGoalsFormResults", () => ({
  default: () => <div data-testid="calculator-results" />,
}));

vi.mock("@/components/ui/dialog", () => ({
  Dialog: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  DialogTrigger: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  DialogContent: ({ children }: { children: ReactNode }) => (
    <div data-testid="dialog-content">{children}</div>
  ),
  DialogHeader: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  DialogTitle: ({ children }: { children: ReactNode }) => <h1>{children}</h1>,
  DialogDescription: () => null,
}));

vi.mock("@/components/ui/sheet", () => ({
  Sheet: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  SheetTrigger: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  SheetContent: ({ children }: { children: ReactNode }) => (
    <div data-testid="sheet-content">{children}</div>
  ),
  SheetHeader: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  SheetTitle: ({ children }: { children: ReactNode }) => <h1>{children}</h1>,
  SheetDescription: () => null,
}));

describe("FitnessCalculator", () => {
  it("renders sheet calculator on large screens", () => {
    render(<FitnessCalculator buttonText="Open calculator" />);

    expect(screen.getByRole("button", { name: "Open calculator" })).toBeInTheDocument();
    expect(screen.getByText("Nutrition goals calculator")).toBeInTheDocument();
  });

  it("switches to results after calculation", () => {
    render(<FitnessCalculator />);

    fireEvent.click(screen.getByRole("button", { name: "Run calculator" }));
    expect(screen.getByTestId("calculator-results")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Return to Calculator" }));
    expect(screen.getByRole("button", { name: "Run calculator" })).toBeInTheDocument();
  });

  it("renders dialog calculator on small screens", () => {
    mockScreenSize.width = 400;

    render(<FitnessCalculator />);

    expect(
      screen.getByRole("button", { name: "Fitness calculator" }),
    ).toBeInTheDocument();
    expect(screen.getByTestId("dialog-content")).toBeInTheDocument();

    mockScreenSize.width = 1200;
  });
});

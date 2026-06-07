import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { macroGoals } from "@/features/nutrition/__tests__/fixtures";
import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";

import CaloriesGraph from "../CaloriesGraph";

vi.mock("@/components/charts/CircleProgressBar", () => ({
  default: ({ completionPercentage }: { completionPercentage: number }) => (
    <div data-testid="circle-progress" data-value={completionPercentage} />
  ),
}));

vi.mock("@/assets/nutrition/DottedSemiCircleBorder.svg?react", () => ({
  default: () => <div data-testid="dotted-border" />,
}));

vi.mock("@/components/ui/hybrid-tooltip", () => ({
  HybridTooltip: ({ children }: { children: React.ReactNode }) => children,
  HybridTooltipTrigger: ({ children }: { children: React.ReactNode }) =>
    children,
  HybridTooltipContent: ({ children }: { children: React.ReactNode }) =>
    children,
}));

describe("CaloriesGraph", () => {
  it("renders calorie progress from goals and overview", () => {
    const nutrition = createEmptyNutritionalContent();
    nutrition.energy = { unit: "calories", value: 1500 };

    render(
      <CaloriesGraph goals={macroGoals()} nutritionOverview={nutrition} />,
    );

    expect(screen.getByTestId("circle-progress")).toHaveAttribute(
      "data-value",
      "75",
    );
    expect(screen.getByText("1500")).toBeInTheDocument();
  });

  it("handles missing data with zero progress", () => {
    render(<CaloriesGraph goals={undefined} nutritionOverview={undefined} />);

    expect(screen.getByTestId("circle-progress")).toHaveAttribute(
      "data-value",
      "0",
    );
  });
});

import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";

import MacrosGraph from "../MacrosGraph";

vi.mock("@/components/charts/DoughnutChart", () => ({
  default: ({
    userData,
    labels,
  }: {
    userData: number[];
    labels: string[];
  }) => (
    <div
      data-testid="doughnut-chart"
      data-values={userData.join(",")}
      data-labels={labels.join(",")}
    />
  ),
}));

describe("MacrosGraph", () => {
  it("renders scaled calories and macro chart data", () => {
    const nutrition = createEmptyNutritionalContent();
    nutrition.energy = { unit: "calories", value: 180 };

    render(
      <MacrosGraph
        nutritionalContents={nutrition}
        nutritionalPercentages={[30, 50, 20]}
        nutritionMultiplier={1.5}
      />,
    );

    expect(screen.getByText("270.0")).toBeInTheDocument();
    expect(screen.getByText("Cal")).toBeInTheDocument();

    const chart = screen.getByTestId("doughnut-chart");
    expect(chart).toHaveAttribute("data-values", "30,50,20");
    expect(chart).toHaveAttribute("data-labels", "Protein,Carbohydrates,Fat");
  });
});

import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { macroGoals } from "@/features/nutrition/__tests__/fixtures";

import MacroProgress from "../MacrosProgress";

describe("MacroProgress", () => {
  it("renders loading skeleton when isLoading is true", () => {
    const { container } = render(
      <MacroProgress goals={macroGoals()} nutritionOverview={undefined} isLoading />,
    );

    expect(container.querySelector('[class*="animate-pulse"]')).toBeTruthy();
  });

  it("renders macro progress with capped percentages", () => {
    const nutrition = createEmptyNutritionalContent();
    nutrition.protein = 200;
    nutrition.carbohydrates = 300;
    nutrition.fat = 100;

    render(
      <MacroProgress
        goals={macroGoals()}
        nutritionOverview={nutrition}
        isLoading={false}
      />,
    );

    expect(screen.getAllByText("100%")).toHaveLength(3);
    expect(screen.getByText("/ 150g Protein")).toBeInTheDocument();
    expect(screen.getByText("/ 220g Carbs")).toBeInTheDocument();
    expect(screen.getByText("/ 70g Fat")).toBeInTheDocument();
  });

  it("shows loading when goals or overview are missing", () => {
    const { container } = render(
      <MacroProgress goals={undefined} nutritionOverview={undefined} isLoading={false} />,
    );

    expect(container.querySelector('[class*="animate-pulse"]')).toBeTruthy();
  });
});

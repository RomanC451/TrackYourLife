import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";

import MacrosDialogHeader from "../MacrosDialogHeader";

vi.mock("../MacrosGraph", () => ({
  default: () => <div data-testid="macros-graph" />,
}));

describe("MacrosDialogHeader", () => {
  it("renders macro overview values from nutritional content", () => {
    const nutrition = createEmptyNutritionalContent();
    nutrition.protein = 20;
    nutrition.carbohydrates = 40;
    nutrition.fat = 10;

    render(<MacrosDialogHeader nutritionalContents={nutrition} />);

    expect(screen.getByTestId("macros-graph")).toBeInTheDocument();
    expect(screen.getByText("Protein")).toBeInTheDocument();
    expect(screen.getByText("Carbs")).toBeInTheDocument();
    expect(screen.getByText("Fat")).toBeInTheDocument();
    expect(screen.getByText("20.0g")).toBeInTheDocument();
  });

  it("renders a loading skeleton", () => {
    const { container } = render(<MacrosDialogHeader.Loading />);

    expect(container.querySelectorAll('[class*="animate-pulse"]').length).toBeGreaterThan(0);
  });
});

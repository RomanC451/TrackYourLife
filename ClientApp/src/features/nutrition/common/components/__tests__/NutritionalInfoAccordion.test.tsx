import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";

import NutritionalInfoAccordion from "../NutritionalInfoAccordion";

describe("NutritionalInfoAccordion", () => {
  it("renders scaled nutritional values when expanded", () => {
    const nutrition = createEmptyNutritionalContent();
    nutrition.energy = { unit: "calories", value: 250 };
    nutrition.fat = 10;
    nutrition.protein = 20;

    render(
      <NutritionalInfoAccordion
        nutritionalContents={nutrition}
        nutritionalMultiplier={2}
      />,
    );

    fireEvent.click(
      screen.getByRole("button", {
        name: /nutritional info per serving size/i,
      }),
    );

    expect(screen.getByText("500.0")).toBeInTheDocument();
    expect(screen.getByText("20.0 g")).toBeInTheDocument();
    expect(screen.getByText("40.0 g")).toBeInTheDocument();
  });

  it("shows placeholders for zero values", () => {
    const nutrition = createEmptyNutritionalContent();

    render(
      <NutritionalInfoAccordion
        nutritionalContents={nutrition}
        nutritionalMultiplier={1}
      />,
    );

    fireEvent.click(
      screen.getByRole("button", {
        name: /nutritional info per serving size/i,
      }),
    );

    expect(screen.getAllByText("--").length).toBeGreaterThan(0);
  });
});

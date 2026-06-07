import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import FoodListElementOverview from "../FoodListElementOverview";

describe("FoodListElementOverview", () => {
  it("renders food overview details", () => {
    const nutrition = createEmptyNutritionalContent();
    nutrition.energy = { unit: "calories", value: 180 };
    nutrition.protein = 12;

    render(
      <FoodListElementOverview
        name="Oats"
        brandName="Brand"
        quantity="100 g"
        nutritionalContents={nutrition}
      />,
    );

    expect(screen.getByText("Oats")).toBeInTheDocument();
    expect(screen.getByText(/Brand/)).toBeInTheDocument();
    expect(screen.getByText(/100 g/)).toBeInTheDocument();
  });
});

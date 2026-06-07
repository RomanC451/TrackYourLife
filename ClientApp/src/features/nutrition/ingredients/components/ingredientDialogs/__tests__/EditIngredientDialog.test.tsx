import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { food, ingredient, recipe } from "@/features/nutrition/__tests__/fixtures";

import EditIngredientDialog from "../EditIngredientDialog";

vi.mock("../IngredientDialog", () => ({
  default: ({ dialogType }: { dialogType: string }) => (
    <div data-testid="ingredient-dialog">{dialogType}</div>
  ),
}));

describe("EditIngredientDialog", () => {
  it("renders the edit ingredient dialog", () => {
    const oats = food("food-1", "Oats");

    render(
      <EditIngredientDialog
        recipe={recipe("recipe-1", "Soup")}
        ingredient={ingredient("ing-1", oats)}
        onSuccess={vi.fn()}
        onClose={vi.fn()}
      />,
    );

    expect(screen.getByTestId("ingredient-dialog")).toHaveTextContent("edit");
  });
});

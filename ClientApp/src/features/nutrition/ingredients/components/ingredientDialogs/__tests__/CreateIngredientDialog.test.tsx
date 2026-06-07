import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { food, recipe } from "@/features/nutrition/__tests__/fixtures";

import CreateIngredientDialog from "../CreateIngredientDialog";

vi.mock("../IngredientDialog", () => ({
  default: ({ dialogType }: { dialogType: string }) => (
    <div data-testid="ingredient-dialog">{dialogType}</div>
  ),
}));

describe("CreateIngredientDialog", () => {
  it("renders the create ingredient dialog", () => {
    render(
      <CreateIngredientDialog
        recipe={recipe("recipe-1", "Soup")}
        food={food("food-1", "Oats")}
        onSuccess={vi.fn()}
        onClose={vi.fn()}
      />,
    );

    expect(screen.getByTestId("ingredient-dialog")).toHaveTextContent("create");
  });
});

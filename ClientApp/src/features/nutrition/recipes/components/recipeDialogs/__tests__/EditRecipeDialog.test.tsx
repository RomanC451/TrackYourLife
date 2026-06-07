import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import EditRecipeDialog from "../EditRecipeDialog";

vi.mock("../RecipeDialog", () => ({
  default: ({ dialogType, recipeId }: { dialogType: string; recipeId: string }) => (
    <div data-testid="recipe-dialog">
      {dialogType}:{recipeId}
    </div>
  ),
}));

describe("EditRecipeDialog", () => {
  it("renders the edit recipe dialog", () => {
    render(<EditRecipeDialog recipeId="recipe-1" onClose={vi.fn()} />);

    expect(screen.getByTestId("recipe-dialog")).toHaveTextContent(
      "edit:recipe-1",
    );
  });
});

import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { recipe } from "@/features/nutrition/__tests__/fixtures";

import RecipeSearch from "../RecipeSearch";

const mockUseCustomQuery = vi.fn();

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

vi.mock("../../recipesList/RecipesList", () => ({
  default: () => <div data-testid="recipes-list" />,
}));

const AddButton = () => <button type="button">Add recipe</button>;

describe("RecipeSearch", () => {
  it("filters recipes and shows the results list on focus", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: [recipe("recipe-1", "Soup"), recipe("recipe-2", "Salad")],
      },
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <RecipeSearch
        AddRecipeButton={AddButton}
        onRecipeSelected={vi.fn()}
        onHoverRecipe={vi.fn()}
        onTouchRecipe={vi.fn()}
        placeHolder="Search recipes"
      />,
    );

    const input = screen.getByPlaceholderText("Search recipes");
    fireEvent.change(input, { target: { value: "sou" } });
    fireEvent.focus(input);

    expect(screen.getByTestId("recipes-list")).toBeInTheDocument();
  });

  it("shows an error when no recipes match", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [recipe("recipe-1", "Soup")] },
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <RecipeSearch
        AddRecipeButton={AddButton}
        onRecipeSelected={vi.fn()}
        onHoverRecipe={vi.fn()}
        onTouchRecipe={vi.fn()}
      />,
    );

    fireEvent.change(screen.getByPlaceholderText("Search recipe..."), {
      target: { value: "pizza" },
    });

    expect(screen.getByText("No recipes found")).toBeInTheDocument();
  });

  it("closes results when clicking outside the search card", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: [recipe("recipe-1", "Soup")],
        isError: false,
      },
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <RecipeSearch
        AddRecipeButton={AddButton}
        onRecipeSelected={vi.fn()}
        onHoverRecipe={vi.fn()}
        onTouchRecipe={vi.fn()}
      />,
    );

    fireEvent.focus(screen.getByPlaceholderText("Search recipe..."));
    expect(screen.getByTestId("recipes-list")).toBeInTheDocument();

    fireEvent.mouseDown(document.body);
    expect(screen.queryByTestId("recipes-list")).not.toBeInTheDocument();
  });

  it("clears the error when a matching recipe is found", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [recipe("recipe-1", "Soup")] },
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <RecipeSearch
        AddRecipeButton={AddButton}
        onRecipeSelected={vi.fn()}
        onHoverRecipe={vi.fn()}
        onTouchRecipe={vi.fn()}
      />,
    );

    const input = screen.getByPlaceholderText("Search recipe...");
    fireEvent.change(input, { target: { value: "pizza" } });
    expect(screen.getByText("No recipes found")).toBeInTheDocument();

    fireEvent.change(input, { target: { value: "sou" } });
    expect(screen.queryByText("No recipes found")).not.toBeInTheDocument();
  });
});

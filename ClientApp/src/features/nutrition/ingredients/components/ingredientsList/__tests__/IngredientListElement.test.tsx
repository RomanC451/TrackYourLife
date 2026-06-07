import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { food, ingredient, recipe } from "@/features/nutrition/__tests__/fixtures";
import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";

import IngredientListElement from "../IngredientListElement";

const { mockMutate, mockNavigate, mockPreloadRoute } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
  mockNavigate: vi.fn(),
  mockPreloadRoute: vi.fn(),
}));

vi.mock("../../../mutations/useRemoveIngredientsMutation", () => ({
  default: () => ({
    mutate: mockMutate,
    isPending: false,
  }),
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("@/App", () => ({
  router: { preloadRoute: mockPreloadRoute },
}));

describe("IngredientListElement", () => {
  const oats = food("food-1", "Oats");
  oats.nutritionalContents = createEmptyNutritionalContent();
  oats.nutritionalContents.energy = { unit: "calories", value: 120 };

  const soup = recipe("recipe-1", "Soup");
  const oatsIngredient = ingredient("ing-1", oats, 2);

  it("renders ingredient details and handles selection", () => {
    const onSelect = vi.fn();

    render(
      <IngredientListElement
        recipe={soup}
        ingredient={oatsIngredient}
        selected={false}
        onSelect={onSelect}
      />,
    );

    expect(screen.getByText("Oats")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("checkbox"));
    expect(onSelect).toHaveBeenCalledWith("ing-1");
  });

  it("removes an ingredient when delete is clicked", () => {
    render(
      <IngredientListElement
        recipe={soup}
        ingredient={oatsIngredient}
        selected={false}
        onSelect={vi.fn()}
      />,
    );

    fireEvent.click(screen.getAllByRole("button").at(-1)!);
    expect(mockMutate).toHaveBeenCalledWith({
      ingredients: [oatsIngredient],
      recipe: soup,
    });
  });

  it("navigates to edit and preloads the route", () => {
    render(
      <IngredientListElement
        recipe={soup}
        ingredient={oatsIngredient}
        selected={false}
        onSelect={vi.fn()}
      />,
    );

    const buttons = screen.getAllByRole("button");
    const editButton = buttons[buttons.length - 2];
    fireEvent.mouseEnter(editButton);
    expect(mockPreloadRoute).toHaveBeenCalledWith({
      to: "/nutrition/recipes/$recipeId/ingredients/edit/$ingredientId",
      params: { recipeId: "recipe-1", ingredientId: "ing-1" },
    });

    fireEvent.click(editButton);
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/nutrition/recipes/$recipeId/ingredients/edit/$ingredientId",
      params: { recipeId: "recipe-1", ingredientId: "ing-1" },
    });
  });

  it("preloads the edit route on touch", () => {
    render(
      <IngredientListElement
        recipe={soup}
        ingredient={oatsIngredient}
        selected={false}
        onSelect={vi.fn()}
      />,
    );

    const buttons = screen.getAllByRole("button");
    const editButton = buttons[buttons.length - 2];

    fireEvent.touchStart(editButton);
    expect(mockPreloadRoute).toHaveBeenCalledWith({
      to: "/nutrition/recipes/$recipeId/ingredients/edit/$ingredientId",
      params: { recipeId: "recipe-1", ingredientId: "ing-1" },
    });
  });

  it("disables actions while deleting", () => {
    render(
      <IngredientListElement
        recipe={soup}
        ingredient={{ ...oatsIngredient, isDeleting: true }}
        selected={false}
        onSelect={vi.fn()}
      />,
    );

    expect(screen.getByRole("checkbox")).toBeDisabled();

    const buttons = screen.getAllByRole("button");
    expect(buttons[buttons.length - 2]).toBeDisabled();
    expect(buttons[buttons.length - 1]).toBeDisabled();
  });

  it("renders a loading skeleton", () => {
    const { container } = render(<IngredientListElement.Loading />);
    expect(container.querySelector(".mb-4")).toBeInTheDocument();
  });
});

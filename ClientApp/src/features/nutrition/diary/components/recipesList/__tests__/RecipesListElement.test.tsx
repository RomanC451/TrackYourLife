import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { recipe, servingSize } from "@/features/nutrition/__tests__/fixtures";

import RecipesListElement from "../RecipesListElement";

const mockNavigate = vi.fn();
const mockPreloadRoute = vi.hoisted(() => vi.fn());
const AddButton = ({ recipe: item }: { recipe: { name: string } }) => (
  <button type="button">Add {item.name}</button>
);

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("@/App", () => ({
  router: { preloadRoute: mockPreloadRoute },
}));

vi.mock(
  "@/features/nutrition/common/components/foodSearch/FoodListElementOverview",
  () => ({
    default: ({ name }: { name: string }) => <div>{name}</div>,
  }),
);

describe("RecipesListElement", () => {
  const soup = recipe("recipe-1", "Soup", {
    servingSizes: [servingSize("ss-1")],
  });

  it("selects a recipe and navigates to edit", () => {
    const onSelected = vi.fn();
    const onHover = vi.fn();
    const onTouch = vi.fn();

    render(
      <RecipesListElement
        recipe={soup}
        AddRecipeButton={AddButton}
        onRecipeSelected={onSelected}
        onHoverRecipe={onHover}
        onTouchRecipe={onTouch}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Soup" }));
    expect(onSelected).toHaveBeenCalledWith(soup);

    fireEvent.mouseEnter(screen.getByRole("button", { name: "Soup" }));
    expect(onHover).toHaveBeenCalledWith(soup);

    const editButton = screen.getAllByRole("button")[1];
    fireEvent.click(editButton);
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/nutrition/recipes/edit/$recipeId",
      params: { recipeId: "recipe-1" },
    });

    expect(screen.getByRole("button", { name: "Add Soup" })).toBeInTheDocument();
  });

  it("preloads the edit route on hover and touch", () => {
    render(
      <RecipesListElement
        recipe={soup}
        AddRecipeButton={AddButton}
        onRecipeSelected={vi.fn()}
        onHoverRecipe={vi.fn()}
        onTouchRecipe={vi.fn()}
      />,
    );

    const editButton = screen.getAllByRole("button")[1];

    fireEvent.mouseEnter(editButton);
    fireEvent.touchStart(editButton);

    expect(mockPreloadRoute).toHaveBeenCalledTimes(2);
    expect(mockPreloadRoute).toHaveBeenCalledWith({
      to: "/nutrition/recipes/edit/$recipeId",
      params: { recipeId: "recipe-1" },
    });
  });

  it("fires touch selection handlers on the recipe row", () => {
    const onTouch = vi.fn();

    render(
      <RecipesListElement
        recipe={soup}
        AddRecipeButton={AddButton}
        onRecipeSelected={vi.fn()}
        onHoverRecipe={vi.fn()}
        onTouchRecipe={onTouch}
      />,
    );

    fireEvent.touchStart(screen.getByRole("button", { name: "Soup" }));
    expect(onTouch).toHaveBeenCalledWith(soup);
  });

  it("renders a loading skeleton", () => {
    const { container } = render(<RecipesListElement.Loading />);
    expect(container.querySelector(".relative")).toBeInTheDocument();
  });
});

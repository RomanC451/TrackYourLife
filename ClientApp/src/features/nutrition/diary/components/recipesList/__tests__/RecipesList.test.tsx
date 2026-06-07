import { fireEvent, render, screen } from "@testing-library/react";
import React from "react";
import { describe, expect, it, vi } from "vitest";

import { recipe } from "@/features/nutrition/__tests__/fixtures";

import RecipesList from "../RecipesList";

vi.mock("@/components/ui/scroll-area", () => ({
  ScrollArea: React.forwardRef(
    (
      { children }: { children: React.ReactNode },
      ref: React.Ref<HTMLDivElement>,
    ) => {
      React.useImperativeHandle(ref, () =>
        ({
          scrollTo: vi.fn(),
        }) as unknown as HTMLDivElement,
      );
      return <div>{children}</div>;
    },
  ),
}));

vi.mock("@/components/ui/separator", () => ({
  Separator: () => <hr data-testid="recipe-separator" />,
}));

vi.mock("../RecipesListElement", () => {
  function RecipesListElement({
    recipe: item,
  }: {
    recipe: { name: string };
  }) {
    return <div>{item.name}</div>;
  }

  RecipesListElement.Loading = function RecipesListElementLoading() {
    return <div data-testid="recipe-list-loading" />;
  };

  return { default: RecipesListElement };
});

const AddButton = () => <button type="button">Add</button>;

describe("RecipesList", () => {
  it("filters and renders matching recipes", () => {
    const cardRef = { current: null };

    render(
      <RecipesList
        recipesList={[recipe("recipe-1", "Soup"), recipe("recipe-2", "Salad")]}
        searchValue="sou"
        pendingState={{ isPending: false, isDelayedPending: false }}
        AddRecipeButton={AddButton}
        onRecipeSelected={vi.fn()}
        cardRef={cardRef}
        onHoverRecipe={vi.fn()}
        onTouchRecipe={vi.fn()}
      />,
    );

    expect(screen.getByText("Soup")).toBeInTheDocument();
    expect(screen.queryByText("Salad")).not.toBeInTheDocument();
  });

  it("renders loading state while delayed pending", () => {
    const cardRef = { current: document.createElement("div") };

    render(
      <RecipesList
        recipesList={[recipe("recipe-1", "Soup")]}
        searchValue=""
        pendingState={{ isPending: true, isDelayedPending: true }}
        AddRecipeButton={AddButton}
        onRecipeSelected={vi.fn()}
        cardRef={cardRef}
        onHoverRecipe={vi.fn()}
        onTouchRecipe={vi.fn()}
      />,
    );

    expect(screen.getAllByTestId("recipe-list-loading").length).toBeGreaterThan(
      0,
    );
  });

  it("returns null during immediate pending", () => {
    const { container } = render(
      <RecipesList
        recipesList={[recipe("recipe-1", "Soup")]}
        searchValue=""
        pendingState={{ isPending: true, isDelayedPending: false }}
        AddRecipeButton={AddButton}
        onRecipeSelected={vi.fn()}
        cardRef={{ current: null }}
        onHoverRecipe={vi.fn()}
        onTouchRecipe={vi.fn()}
      />,
    );

    expect(container).toBeEmptyDOMElement();
  });

  it("renders separators between multiple recipes", () => {
    const cardRef = { current: null };

    render(
      <RecipesList
        recipesList={[recipe("recipe-1", "Soup"), recipe("recipe-2", "Salad")]}
        searchValue=""
        pendingState={{ isPending: false, isDelayedPending: false }}
        AddRecipeButton={AddButton}
        onRecipeSelected={vi.fn()}
        cardRef={cardRef}
        onHoverRecipe={vi.fn()}
        onTouchRecipe={vi.fn()}
      />,
    );

    expect(screen.getByText("Soup")).toBeInTheDocument();
    expect(screen.getByText("Salad")).toBeInTheDocument();
    expect(screen.getAllByTestId("recipe-separator")).toHaveLength(1);
  });

  it("stops mouse down propagation on the loading card", () => {
    const onParentMouseDown = vi.fn();
    const cardRef = { current: document.createElement("div") };

    const { container } = render(
      <div onMouseDown={onParentMouseDown}>
        <RecipesList
          recipesList={[recipe("recipe-1", "Soup")]}
          searchValue=""
          pendingState={{ isPending: true, isDelayedPending: true }}
          AddRecipeButton={AddButton}
          onRecipeSelected={vi.fn()}
          cardRef={cardRef}
          onHoverRecipe={vi.fn()}
          onTouchRecipe={vi.fn()}
        />
      </div>,
    );

    fireEvent.mouseDown(container.querySelector("[class*='backdrop-blur']")!);
    expect(onParentMouseDown).not.toHaveBeenCalled();
  });
});

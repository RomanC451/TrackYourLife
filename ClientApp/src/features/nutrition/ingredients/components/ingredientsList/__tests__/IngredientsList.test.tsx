import { fireEvent, render, screen } from "@testing-library/react";

import { describe, expect, it, vi } from "vitest";



import { ingredient, food, recipe } from "@/features/nutrition/__tests__/fixtures";



import IngredientsList from "../IngredientsList";



const {

  mockMutateAsync,

  mockClearSelection,

  mockHandleSelectAll,

} = vi.hoisted(() => ({

  mockMutateAsync: vi.fn(),

  mockClearSelection: vi.fn(),

  mockHandleSelectAll: vi.fn(),

}));



vi.mock("@/features/nutrition/common/components/foodSearch/FoodSearch", () => {

  function FoodSearch() {

    return <div data-testid="food-search" />;

  }

  FoodSearch.Loading = () => <div data-testid="food-search-loading" />;

  return { default: FoodSearch };

});



vi.mock(

  "@/features/nutrition/common/components/foodSearch/useFoodSearchContext",

  () => ({

    FoodSearchContextProvider: ({ children }: { children: React.ReactNode }) =>

      children,

  }),

);



vi.mock("../../../hooks/useIngredientsSelection", () => ({

  useIngredientsSelection: () => ({

    selectedIds: ["ing-1"],

    toggle: vi.fn(),

    selectedIngredients: [{ id: "ing-1" }],

    isAllSelected: false,

    handleSelectAll: mockHandleSelectAll,

    clearSelection: mockClearSelection,

  }),

}));



vi.mock("../../../mutations/useRemoveIngredientsMutation", () => ({

  default: () => ({

    mutateAsync: mockMutateAsync,

    isPending: false,

    isDelayedPending: false,

  }),

}));



vi.mock("../IngredientListElement", () => {

  function IngredientListElement({

    ingredient: item,

  }: {

    ingredient: { food: { name: string } };

  }) {

    return <div>{item.food.name}</div>;

  }

  IngredientListElement.Loading = () => (

    <div data-testid="ingredient-loading" />

  );

  return { default: IngredientListElement };

});



describe("IngredientsList", () => {

  const oats = food("food-1", "Oats");

  const soup = recipe("recipe-1", "Soup", {

    ingredients: [ingredient("ing-1", oats)],

  });



  beforeEach(() => {

    vi.clearAllMocks();

    mockMutateAsync.mockImplementation(

      async (_vars, options?: { onSuccess?: () => void }) => {

        options?.onSuccess?.();

      },

    );

  });



  it("renders search, header, and ingredient items", () => {

    render(<IngredientsList recipe={soup} />);



    expect(screen.getByTestId("food-search")).toBeInTheDocument();

    expect(screen.getByText("Oats")).toBeInTheDocument();

    expect(screen.getByText("Select all")).toBeInTheDocument();

  });



  it("deletes selected ingredients and clears selection", async () => {

    render(<IngredientsList recipe={soup} />);



    fireEvent.click(

      screen.getByRole("button", { name: /delete selected : 1/i }),

    );



    await vi.waitFor(() => {

      expect(mockMutateAsync).toHaveBeenCalled();

      expect(mockClearSelection).toHaveBeenCalled();

    });

  });



  it("selects all ingredients from the footer checkbox", () => {

    render(<IngredientsList recipe={soup} />);



    fireEvent.click(screen.getByRole("checkbox"));

    expect(mockHandleSelectAll).toHaveBeenCalledWith(true);

  });



  it("renders subcomponents and loading state", () => {

    render(

      <IngredientsList.Header

        onDelete={vi.fn()}

        selectedCount={2}

        isPending={false}

        isDelayedPending={false}

      />,

    );

    expect(screen.getByText("Delete selected : 2")).toBeInTheDocument();



    render(

      <IngredientsList.Footer

        hasIngredients

        isAllSelected={false}

        isPending={false}

        onSelectAll={vi.fn()}

      />,

    );

    expect(screen.getAllByText("Select all").length).toBeGreaterThan(0);



    render(<IngredientsList.Loading />);

    expect(screen.getByTestId("food-search-loading")).toBeInTheDocument();

    expect(screen.getAllByTestId("ingredient-loading").length).toBeGreaterThan(

      0,

    );

  });

});



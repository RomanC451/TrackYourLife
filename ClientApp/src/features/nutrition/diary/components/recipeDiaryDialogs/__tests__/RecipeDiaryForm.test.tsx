import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { recipe, servingSize } from "@/features/nutrition/__tests__/fixtures";
import { MealTypes } from "@/services/openapi";

import { RecipeDiaryForm } from "../RecipeDiaryForm";

const mockNutritionalInfoAccordion = vi.hoisted(() => vi.fn());

vi.mock(
  "@/features/nutrition/common/components/formFields/QuantityFormField",
  () => ({
    default: () => <div data-testid="quantity-field" />,
  }),
);

vi.mock(
  "@/features/nutrition/common/components/formFields/ServingSizeFormField",
  () => ({
    default: () => <div data-testid="serving-size-field" />,
  }),
);

vi.mock(
  "@/features/nutrition/common/components/formFields/MealTypeFormField",
  () => ({
    default: () => <div data-testid="meal-type-field" />,
  }),
);

vi.mock("@/features/nutrition/common/components/NutritionalInfoAccordion", () => ({
  default: (props: {
    nutritionalMultiplier: number;
    nutritionalContents: unknown;
  }) => {
    mockNutritionalInfoAccordion(props);
    return <div data-testid="nutritional-info" />;
  },
}));

vi.mock("@/components/linear-progress", () => ({
  default: () => <div data-testid="linear-progress" />,
}));

describe("RecipeDiaryForm", () => {
  const breakfastBowl = recipe("recipe-1", "Breakfast bowl");
  breakfastBowl.servingSizes = [servingSize("ss-1")];

  const formValues = {
    servingSizeId: "ss-1",
    quantity: 2,
    mealType: MealTypes.Breakfast,
    recipeId: "recipe-1",
    entryDate: "2026-06-05",
  };

  const form = {
    getValues: vi.fn((key?: keyof typeof formValues) =>
      key ? formValues[key] : formValues,
    ),
  } as never;

  it("renders recipe diary fields and submit button", () => {
    render(
      <RecipeDiaryForm
        form={form}
        handleCustomSubmit={vi.fn()}
        submitButtonText="Add recipe"
        pendingState={{ isPending: false, isDelayedPending: false }}
        recipe={breakfastBowl}
      />,
    );

    expect(screen.getByTestId("quantity-field")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Add recipe" })).toBeEnabled();
  });

  it("shows progress while delayed pending", () => {
    render(
      <RecipeDiaryForm
        form={form}
        handleCustomSubmit={vi.fn()}
        submitButtonText="Save"
        pendingState={{ isPending: false, isDelayedPending: true }}
        recipe={breakfastBowl}
      />,
    );

    expect(screen.getByTestId("linear-progress")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Save" }));
    expect(screen.getByRole("button", { name: "Save" })).toBeEnabled();
  });

  it("forwards submit when the button is clicked", () => {
    const handleSubmit = vi.fn();

    render(
      <RecipeDiaryForm
        form={form}
        handleCustomSubmit={handleSubmit}
        submitButtonText="Add recipe"
        pendingState={{ isPending: false, isDelayedPending: false }}
        recipe={breakfastBowl}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Add recipe" }));
    expect(handleSubmit).toHaveBeenCalledTimes(1);
  });

  it("disables submit while pending and hides progress when not delayed", () => {
    render(
      <RecipeDiaryForm
        form={form}
        handleCustomSubmit={vi.fn()}
        submitButtonText="Save"
        pendingState={{ isPending: true, isDelayedPending: false }}
        recipe={breakfastBowl}
      />,
    );

    expect(screen.getByRole("button", { name: "Save" })).toBeDisabled();
    expect(screen.queryByTestId("linear-progress")).not.toBeInTheDocument();
  });

  it("passes the selected serving size nutrition multiplier to NutritionalInfoAccordion", () => {
    const multiServingRecipe = recipe("recipe-2", "Porridge");
    multiServingRecipe.servingSizes = [
      servingSize("ss-1", 1),
      servingSize("ss-2", 3),
    ];

    const formWithSecondServing = {
      getValues: vi.fn((key?: keyof typeof formValues) =>
        key === "servingSizeId"
          ? "ss-2"
          : key
            ? formValues[key]
            : { ...formValues, servingSizeId: "ss-2" },
      ),
    } as never;

    render(
      <RecipeDiaryForm
        form={formWithSecondServing}
        handleCustomSubmit={vi.fn()}
        submitButtonText="Add"
        pendingState={{ isPending: false, isDelayedPending: false }}
        recipe={multiServingRecipe}
      />,
    );

    expect(mockNutritionalInfoAccordion).toHaveBeenCalledWith(
      expect.objectContaining({
        nutritionalMultiplier: 3,
        nutritionalContents: multiServingRecipe.nutritionalContents,
      }),
    );
  });
});

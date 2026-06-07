import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { food, servingSize } from "@/features/nutrition/__tests__/fixtures";
import { MealTypes } from "@/services/openapi";

import FoodDiaryForm from "../FoodDiaryForm";

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

describe("FoodDiaryForm", () => {
  const oats = food("food-1", "Oats");
  const handleCustomSubmit = vi.fn((event) => event.preventDefault());

  const formValues = {
    servingSizeId: "ss-1",
    quantity: 2,
    mealType: MealTypes.Breakfast,
    foodId: "food-1",
    entryDate: "2026-06-05",
  };

  const form = {
    getValues: vi.fn((key?: keyof typeof formValues) =>
      key ? formValues[key] : formValues,
    ),
  } as never;

  it("renders form fields and submit button", () => {
    render(
      <FoodDiaryForm
        form={form}
        handleCustomSubmit={handleCustomSubmit}
        submitButtonText="Add to diary"
        pendingState={{ isPending: false, isDelayedPending: false }}
        food={oats}
      />,
    );

    expect(screen.getByTestId("quantity-field")).toBeInTheDocument();
    expect(screen.getByTestId("serving-size-field")).toBeInTheDocument();
    expect(screen.getByTestId("meal-type-field")).toBeInTheDocument();
    expect(screen.getByTestId("nutritional-info")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: "Add to diary" }),
    ).toBeEnabled();
  });

  it("disables submit while pending and forwards submit handler", () => {
    render(
      <FoodDiaryForm
        form={form}
        handleCustomSubmit={handleCustomSubmit}
        submitButtonText="Save"
        pendingState={{ isPending: true, isDelayedPending: false }}
        food={oats}
      />,
    );

    const submitButton = screen.getByRole("button", { name: "Save" });
    expect(submitButton).toBeDisabled();

    fireEvent.submit(submitButton.closest("form")!);
    expect(handleCustomSubmit).toHaveBeenCalled();
  });

  it("forwards submit when the button is clicked", () => {
    const handleSubmit = vi.fn((event) => event.preventDefault());

    render(
      <FoodDiaryForm
        form={form}
        handleCustomSubmit={handleSubmit}
        submitButtonText="Add to diary"
        pendingState={{ isPending: false, isDelayedPending: false }}
        food={oats}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Add to diary" }));
    expect(handleSubmit).toHaveBeenCalledTimes(1);
  });

  it("passes the selected serving size nutrition multiplier to NutritionalInfoAccordion", () => {
    const multiServingFood = food("food-2", "Rice");
    multiServingFood.servingSizes = [
      servingSize("ss-1", 1),
      servingSize("ss-2", 2.5),
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
      <FoodDiaryForm
        form={formWithSecondServing}
        handleCustomSubmit={handleCustomSubmit}
        submitButtonText="Add"
        pendingState={{ isPending: false, isDelayedPending: false }}
        food={multiServingFood}
      />,
    );

    expect(mockNutritionalInfoAccordion).toHaveBeenCalledWith(
      expect.objectContaining({
        nutritionalMultiplier: 2.5,
        nutritionalContents: multiServingFood.nutritionalContents,
      }),
    );
  });
});

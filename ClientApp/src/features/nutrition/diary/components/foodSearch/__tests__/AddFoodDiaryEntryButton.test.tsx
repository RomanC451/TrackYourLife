import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { food } from "@/features/nutrition/__tests__/fixtures";
import { MealTypes } from "@/services/openapi";

import AddFoodDiaryEntryButton from "../AddFoodDiaryEntryButton";

const mockMutate = vi.fn();

vi.mock("../../../mutations/useAddFoodDiaryMutation", () => ({
  default: () => ({
    mutate: mockMutate,
    pendingState: { isPending: false, isDelayedPending: false },
  }),
}));

vi.mock(
  "@/features/nutrition/common/components/formFields/MealTypeDropDownMenu",
  () => ({
    default: ({
      selectCallback,
    }: {
      selectCallback: (meal: MealTypes) => void;
    }) => (
      <button type="button" onClick={() => selectCallback(MealTypes.Breakfast)}>
        Pick meal
      </button>
    ),
  }),
);

describe("AddFoodDiaryEntryButton", () => {
  it("delegates meal selection to the food diary mutation", () => {
    const oats = food("food-1", "Oats");

    render(<AddFoodDiaryEntryButton food={oats} date="2026-06-05" />);

    screen.getByRole("button", { name: "Pick meal" }).click();
    expect(mockMutate).toHaveBeenCalledWith({
      foodId: "food-1",
      mealType: MealTypes.Breakfast,
      servingSizeId: "ss-1",
      quantity: 1,
      entryDate: "2026-06-05",
    });
  });
});

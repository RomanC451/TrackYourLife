import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { recipe, servingSize } from "@/features/nutrition/__tests__/fixtures";
import { MealTypes } from "@/services/openapi";

import AddRecipeDiaryEntryButton from "../AddRecipeDiaryEntryButton";

const mockMutate = vi.fn();

vi.mock("../../../mutations/useAddRecipeDiaryMutation", () => ({
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
      <button type="button" onClick={() => selectCallback(MealTypes.Lunch)}>
        Pick meal
      </button>
    ),
  }),
);

describe("AddRecipeDiaryEntryButton", () => {
  it("delegates meal selection to the recipe diary mutation", () => {
    const soup = recipe("recipe-1", "Soup", {
      servingSizes: [servingSize("ss-1")],
    });

    render(<AddRecipeDiaryEntryButton recipe={soup} date="2026-06-05" />);

    screen.getByRole("button", { name: "Pick meal" }).click();
    expect(mockMutate).toHaveBeenCalledWith({
      recipeId: "recipe-1",
      mealType: MealTypes.Lunch,
      quantity: 1,
      entryDate: "2026-06-05",
      servingSizeId: "ss-1",
    });
  });
});

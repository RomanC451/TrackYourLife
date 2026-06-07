import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { food } from "@/features/nutrition/__tests__/fixtures";
import { MealTypes } from "@/services/openapi";

import { EditFoodDiaryDialog } from "../EditFoodDiaryDialog";

const oats = food("food-1", "Oats");

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: () => ({
      data: {
        food: oats,
        servingSize: oats.servingSizes[0],
        quantity: 2,
        date: "2026-06-05",
        mealType: MealTypes.Breakfast,
      },
    }),
  };
});

vi.mock("../../../mutations/useUpdateFoodDiaryMutation", () => ({
  default: () => ({
    mutate: vi.fn(),
    pendingState: { isPending: false },
  }),
}));

vi.mock("../FoodDiaryDialog", () => ({
  FoodDiaryDialog: ({ dialogType }: { dialogType: string }) => (
    <div data-testid="food-diary-dialog">{dialogType}</div>
  ),
}));

describe("EditFoodDiaryDialog", () => {
  it("renders the edit food diary dialog", () => {
    render(
      <EditFoodDiaryDialog
        foodDiaryId="diary-1"
        onSuccess={vi.fn()}
        onClose={vi.fn()}
      />,
    );

    expect(screen.getByTestId("food-diary-dialog")).toHaveTextContent("edit");
  });
});

import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { food } from "@/features/nutrition/__tests__/fixtures";
import { MealTypes } from "@/services/openapi";

import { CreateFoodDiaryDialog } from "../CreateFoodDiaryDialog";

const oats = food("food-1", "Oats");
oats.lastServingSizeUsedId = "ss-1";
oats.lastQuantityUsed = 2;

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: () => ({ data: oats }),
  };
});

vi.mock("usehooks-ts", () => ({
  useLocalStorage: () => [MealTypes.Lunch, vi.fn()],
}));

vi.mock("../../../mutations/useAddFoodDiaryMutation", () => ({
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

describe("CreateFoodDiaryDialog", () => {
  it("renders the create food diary dialog with loaded food", () => {
    render(
      <CreateFoodDiaryDialog
        foodId="food-1"
        onSuccess={vi.fn()}
        onClose={vi.fn()}
      />,
    );

    expect(screen.getByTestId("food-diary-dialog")).toHaveTextContent("create");
  });
});

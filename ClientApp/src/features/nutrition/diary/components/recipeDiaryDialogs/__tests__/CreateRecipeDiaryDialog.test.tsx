import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { recipe, servingSize } from "@/features/nutrition/__tests__/fixtures";
import { MealTypes } from "@/services/openapi";

import { CreateRecipeDiaryDialog } from "../CreateRecipeDiaryDialog";

const soup = recipe("recipe-1", "Soup", {
  servingSizes: [servingSize("ss-1")],
});

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: () => ({ data: soup }),
  };
});

vi.mock("usehooks-ts", () => ({
  useReadLocalStorage: () => MealTypes.Dinner,
}));

vi.mock("../../../mutations/useAddRecipeDiaryMutation", () => ({
  default: () => ({
    mutate: vi.fn(),
    pendingState: { isPending: false },
  }),
}));

vi.mock("../RecipeDiaryDialog", () => ({
  RecipeDiaryDialog: ({ dialogType }: { dialogType: string }) => (
    <div data-testid="recipe-diary-dialog">{dialogType}</div>
  ),
}));

describe("CreateRecipeDiaryDialog", () => {
  it("renders the create recipe diary dialog", () => {
    render(
      <CreateRecipeDiaryDialog
        recipeId="recipe-1"
        onSuccess={vi.fn()}
        onClose={vi.fn()}
      />,
    );

    expect(screen.getByTestId("recipe-diary-dialog")).toHaveTextContent("create");
  });
});

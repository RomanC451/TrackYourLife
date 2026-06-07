import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { food, recipe } from "@/features/nutrition/__tests__/fixtures";

import AddIngredientButton from "../AddIngredientButton";

const mockMutate = vi.fn();

const mockUseAddIngredientMutation = vi.hoisted(() =>
  vi.fn(() => ({
    mutate: mockMutate,
    isPending: false,
    pendingState: { isPending: false },
  })),
);

vi.mock("@/components/ui/spinner", () => ({
  default: () => <div data-testid="spinner" />,
}));

vi.mock("../../mutations/useAddIngredientMutation", () => ({
  default: () => mockUseAddIngredientMutation(),
}));

describe("AddIngredientButton", () => {
  it("adds the food as an ingredient on click", () => {
    const oats = food("food-1", "Oats");
    const soup = recipe("recipe-1", "Soup");

    render(<AddIngredientButton food={oats} recipe={soup} />);

    fireEvent.click(screen.getByRole("button"));
    expect(mockMutate).toHaveBeenCalledWith({
      foodId: "food-1",
      servingSizeId: "ss-1",
      quantity: 1,
    });
  });

  it("shows a spinner while the mutation is pending", () => {
    mockUseAddIngredientMutation.mockReturnValue({
      mutate: mockMutate,
      isPending: true,
      pendingState: { isPending: true },
    });

    const oats = food("food-1", "Oats");
    const soup = recipe("recipe-1", "Soup");

    render(<AddIngredientButton food={oats} recipe={soup} />);

    expect(screen.getByTestId("spinner")).toBeInTheDocument();
  });
});

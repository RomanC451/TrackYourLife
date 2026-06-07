import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { food } from "@/features/nutrition/__tests__/fixtures";
import { foodQueryOptions } from "@/features/nutrition/common/queries/useFoodQuery";
import { queryClient } from "@/queryClient";
import FoodListElement, { LoadingFoodListElement } from "../FoodListElement";

const mockNavigate = vi.fn();
const AddButton = () => <button type="button">Add</button>;

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("../useFoodSearchContext", () => ({
  useFoodSearchContext: () => ({
    AddFoodButtonComponent: AddButton,
    onSelectedFoodToOptions: { to: "/nutrition/diary/foodDiary/create" },
    setAddFoodButtonComponent: vi.fn(),
    setOnSelectedFoodToOptions: vi.fn(),
  }),
}));

describe("FoodListElement", () => {
  it("navigates with the selected food id", () => {
    const oats = food("food-1", "Oats");

    render(<FoodListElement food={oats} />);

    fireEvent.click(screen.getByRole("button", { name: /oats/i }));
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/nutrition/diary/foodDiary/create",
      search: { foodId: "food-1" },
    });
    expect(screen.getByRole("button", { name: "Add" })).toBeInTheDocument();
  });

  it("renders a loading skeleton", () => {
    const { container } = render(<LoadingFoodListElement />);
    expect(container.querySelector(".relative")).toBeInTheDocument();
  });

  it("preloads food details on hover and touch", () => {
    const oats = food("food-1", "Oats");

    render(<FoodListElement food={oats} />);

    const button = screen.getByRole("button", { name: /oats/i });
    fireEvent.mouseEnter(button);
    expect(
      queryClient.getQueryData(foodQueryOptions.byId("food-1").queryKey),
    ).toEqual(oats);

    queryClient.removeQueries({
      queryKey: foodQueryOptions.byId("food-1").queryKey,
    });
    fireEvent.touchStart(button);
    expect(
      queryClient.getQueryData(foodQueryOptions.byId("food-1").queryKey),
    ).toEqual(oats);
  });
});

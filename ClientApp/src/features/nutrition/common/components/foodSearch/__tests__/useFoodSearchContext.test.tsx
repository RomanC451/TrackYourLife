import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { food } from "@/features/nutrition/__tests__/fixtures";

import {
  FoodSearchContextProvider,
  useFoodSearchContext,
} from "../useFoodSearchContext";

function TestConsumer() {
  const {
    AddFoodButtonComponent,
    setAddFoodButtonComponent,
    onSelectedFoodToOptions,
    setOnSelectedFoodToOptions,
  } = useFoodSearchContext();

  return (
    <div>
      <button
        type="button"
        onClick={() =>
          setAddFoodButtonComponent(() => function AddButton() {
            return <span data-testid="custom-add-button">Add</span>;
          })
        }
      >
        Set button
      </button>
      <button
        type="button"
        onClick={() =>
          setOnSelectedFoodToOptions({ to: "/nutrition/diary" as never })
        }
      >
        Set route
      </button>
      {AddFoodButtonComponent ? (
        <AddFoodButtonComponent food={food("food-1", "Oats")} />
      ) : null}
      <span data-testid="route">{JSON.stringify(onSelectedFoodToOptions)}</span>
    </div>
  );
}

describe("FoodSearchContextProvider", () => {
  it("provides add-button and navigation state to consumers", () => {
    render(
      <FoodSearchContextProvider>
        <TestConsumer />
      </FoodSearchContextProvider>,
    );

    fireEvent.click(screen.getByRole("button", { name: "Set button" }));
    expect(screen.getByTestId("custom-add-button")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Set route" }));
    expect(screen.getByTestId("route")).toHaveTextContent(
      JSON.stringify({ to: "/nutrition/diary" }),
    );
  });
});

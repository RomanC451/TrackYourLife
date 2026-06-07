import { fireEvent, render, screen } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { food } from "@/features/nutrition/__tests__/fixtures";
import { FoodSearchContextProvider } from "../useFoodSearchContext";
import FoodSearch from "../FoodSearch";

const mockUseFoodSearch = vi.fn();
const mockRemoveFoodSearchQuery = vi.hoisted(() => vi.fn());

vi.mock("../../../queries/useFoodSearchQuery", () => ({
  default: () => mockUseFoodSearch(),
  removeFoodSearchQuery: mockRemoveFoodSearchQuery,
}));

vi.mock("../FoodList", () => ({
  default: ({ searchValue }: { searchValue: string }) => (
    <div data-testid="food-list">{searchValue}</div>
  ),
}));

const AddButton = ({ food: foodItem }: { food: { name: string } }) => (
  <button type="button">Add {foodItem.name}</button>
);

describe("FoodSearch", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    mockRemoveFoodSearchQuery.mockClear();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("opens results when focused and shows the food list", () => {
    mockUseFoodSearch.mockReturnValue({
      searchQuery: {
        isError: false,
        data: { pages: [{ items: [food("food-1", "Oats")] }] },
        fetchNextPage: vi.fn(),
        isFetchingNextPage: false,
        hasNextPage: false,
      },
      searchValue: "oats",
      setSearchValue: vi.fn(),
      error: "",
      resetError: vi.fn(),
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <FoodSearchContextProvider>
        <FoodSearch
          addFoodButtonComponent={AddButton}
          onSelectedFoodToOptions={{ to: "/nutrition/diary/foodDiary/create" }}
          placeHolder="Find food"
        />
      </FoodSearchContextProvider>,
    );

    fireEvent.focus(screen.getByPlaceholderText("Find food"));
    expect(screen.getByTestId("food-list")).toHaveTextContent("oats");
  });

  it("renders a disabled loading state", () => {
    render(<FoodSearch.Loading />);
    expect(screen.getByPlaceholderText("Search food...")).toBeDisabled();
  });

  it("shows search errors and closes results on outside click", () => {
    mockUseFoodSearch.mockReturnValue({
      searchQuery: {
        isError: false,
        data: { pages: [{ items: [food("food-1", "Oats")] }] },
        fetchNextPage: vi.fn(),
        isFetchingNextPage: false,
        hasNextPage: false,
      },
      searchValue: "oats",
      setSearchValue: vi.fn(),
      error: "No foods found",
      resetError: vi.fn(),
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <FoodSearchContextProvider>
        <FoodSearch
          addFoodButtonComponent={AddButton}
          onSelectedFoodToOptions={{ to: "/nutrition/diary/foodDiary/create" }}
        />
      </FoodSearchContextProvider>,
    );

    fireEvent.focus(screen.getByPlaceholderText("Search food..."));
    expect(screen.getByTestId("food-list")).toBeInTheDocument();

    fireEvent.mouseDown(document.body);
    expect(screen.queryByTestId("food-list")).not.toBeInTheDocument();
  });

  it("does not open results when the query has no items", () => {
    mockUseFoodSearch.mockReturnValue({
      searchQuery: {
        isError: false,
        data: { pages: [{ items: [] }] },
        fetchNextPage: vi.fn(),
        isFetchingNextPage: false,
        hasNextPage: false,
      },
      searchValue: "",
      setSearchValue: vi.fn(),
      error: "",
      resetError: vi.fn(),
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <FoodSearchContextProvider>
        <FoodSearch
          addFoodButtonComponent={AddButton}
          onSelectedFoodToOptions={{ to: "/nutrition/diary/foodDiary/create" }}
        />
      </FoodSearchContextProvider>,
    );

    fireEvent.focus(screen.getByPlaceholderText("Search food..."));
    expect(screen.queryByTestId("food-list")).not.toBeInTheDocument();
  });

  it("debounces search input, clears errors, and removes stale queries", () => {
    const setSearchValue = vi.fn();
    const resetError = vi.fn();

    mockUseFoodSearch.mockReturnValue({
      searchQuery: {
        isError: false,
        data: { pages: [{ items: [food("food-1", "Oats")] }] },
        fetchNextPage: vi.fn(),
        isFetchingNextPage: false,
        hasNextPage: false,
      },
      searchValue: "",
      setSearchValue,
      error: "",
      resetError,
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <FoodSearchContextProvider>
        <FoodSearch
          addFoodButtonComponent={AddButton}
          onSelectedFoodToOptions={{ to: "/nutrition/diary/foodDiary/create" }}
        />
      </FoodSearchContextProvider>,
    );

    fireEvent.change(screen.getByPlaceholderText("Search food..."), {
      target: { value: "oats" },
    });

    expect(setSearchValue).not.toHaveBeenCalled();

    vi.advanceTimersByTime(1000);

    expect(resetError).toHaveBeenCalled();
    expect(setSearchValue).toHaveBeenCalledWith("oats");
    expect(mockRemoveFoodSearchQuery).toHaveBeenCalledWith("");
  });

  it("hides results when the search query is in an error state", () => {
    mockUseFoodSearch.mockReturnValue({
      searchQuery: {
        isError: true,
        data: { pages: [{ items: [food("food-1", "Oats")] }] },
        fetchNextPage: vi.fn(),
        isFetchingNextPage: false,
        hasNextPage: false,
      },
      searchValue: "oats",
      setSearchValue: vi.fn(),
      error: "",
      resetError: vi.fn(),
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <FoodSearchContextProvider>
        <FoodSearch
          addFoodButtonComponent={AddButton}
          onSelectedFoodToOptions={{ to: "/nutrition/diary/foodDiary/create" }}
        />
      </FoodSearchContextProvider>,
    );

    fireEvent.focus(screen.getByPlaceholderText("Search food..."));
    expect(screen.queryByTestId("food-list")).not.toBeInTheDocument();
  });

  it("keeps results open when clicking inside the results card", () => {
    mockUseFoodSearch.mockReturnValue({
      searchQuery: {
        isError: false,
        data: { pages: [{ items: [food("food-1", "Oats")] }] },
        fetchNextPage: vi.fn(),
        isFetchingNextPage: false,
        hasNextPage: false,
      },
      searchValue: "oats",
      setSearchValue: vi.fn(),
      error: "",
      resetError: vi.fn(),
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <FoodSearchContextProvider>
        <FoodSearch
          addFoodButtonComponent={AddButton}
          onSelectedFoodToOptions={{ to: "/nutrition/diary/foodDiary/create" }}
        />
      </FoodSearchContextProvider>,
    );

    fireEvent.focus(screen.getByPlaceholderText("Search food..."));
    fireEvent.mouseDown(screen.getByTestId("food-list"));
    expect(screen.getByTestId("food-list")).toBeInTheDocument();
  });
});

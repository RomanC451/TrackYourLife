import { fireEvent, render, screen } from "@testing-library/react";
import React from "react";
import { describe, expect, it, vi } from "vitest";

const mockScrollTo = vi.hoisted(() => vi.fn());

vi.mock("@/components/ui/scroll-area", () => ({
  ScrollArea: React.forwardRef(
    (
      {
        children,
        onScroll,
      }: {
        children: React.ReactNode;
        onScroll?: (event: React.UIEvent<HTMLDivElement>) => void;
      },
      ref: React.Ref<HTMLDivElement>,
    ) => {
      React.useImperativeHandle(ref, () =>
        ({
          scrollTo: mockScrollTo,
          scrollHeight: 500,
          scrollTop: 0,
          clientHeight: 100,
        }) as unknown as HTMLDivElement,
      );
      return <div onScroll={onScroll}>{children}</div>;
    },
  ),
}));

import { food } from "@/features/nutrition/__tests__/fixtures";
import FoodList from "../FoodList";

vi.mock("../FoodListElement", () => ({
  default: ({ food: foodItem }: { food: { name: string } }) => (
    <div>{foodItem.name}</div>
  ),
  LoadingFoodListElement: () => <div data-testid="loading-item" />,
}));

vi.mock("@/components/ui/spinner", () => ({
  default: () => <div data-testid="spinner" />,
}));

describe("FoodList", () => {
  const fetchNextPage = vi.fn();

  it("renders food items", () => {
    render(
      <FoodList
        foodList={[food("food-1", "Oats"), food("food-2", "Rice")]}
        fetchNextPage={fetchNextPage}
        isFetchingNextPage={false}
        hasNextPage={false}
        searchValue="oats"
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    expect(screen.getByText("Oats")).toBeInTheDocument();
    expect(screen.getByText("Rice")).toBeInTheDocument();
  });

  it("shows loading skeletons while delayed pending", () => {
    render(
      <FoodList
        foodList={[]}
        fetchNextPage={fetchNextPage}
        isFetchingNextPage={false}
        hasNextPage={false}
        searchValue=""
        pendingState={{ isPending: false, isDelayedPending: true }}
      />,
    );

    expect(screen.getAllByTestId("loading-item").length).toBeGreaterThan(0);
  });

  it("shows a spinner while fetching the next page", () => {
    render(
      <FoodList
        foodList={[food("food-1", "Oats")]}
        fetchNextPage={fetchNextPage}
        isFetchingNextPage={true}
        hasNextPage={true}
        searchValue="oats"
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    expect(screen.getByTestId("spinner")).toBeInTheDocument();
  });

  it("fetches the next page when scrolled to the bottom", () => {
    render(
      <FoodList
        foodList={[food("food-1", "Oats")]}
        fetchNextPage={fetchNextPage}
        isFetchingNextPage={false}
        hasNextPage={true}
        searchValue="oats"
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    const scrollArea = screen.getByText("Oats").parentElement!.parentElement!;
    Object.defineProperty(scrollArea, "scrollHeight", {
      value: 500,
      configurable: true,
    });
    Object.defineProperty(scrollArea, "clientHeight", {
      value: 100,
      configurable: true,
    });
    Object.defineProperty(scrollArea, "scrollTop", {
      value: 400,
      configurable: true,
    });

    fireEvent.scroll(scrollArea);

    expect(fetchNextPage).toHaveBeenCalledTimes(1);
  });

  it("scrolls back to the top when the search value changes", () => {
    mockScrollTo.mockClear();

    const { rerender } = render(
      <FoodList
        foodList={[food("food-1", "Oats")]}
        fetchNextPage={fetchNextPage}
        isFetchingNextPage={false}
        hasNextPage={false}
        searchValue="oats"
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    rerender(
      <FoodList
        foodList={[food("food-2", "Rice")]}
        fetchNextPage={fetchNextPage}
        isFetchingNextPage={false}
        hasNextPage={false}
        searchValue="rice"
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    expect(mockScrollTo).toHaveBeenCalledWith({ top: 0 });
  });
});

import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeCategory } from "@/features/youtube/__tests__/fixtures";

import CategoryTabs from "../CategoryTabs";

describe("CategoryTabs", () => {
  beforeEach(() => {
    class ResizeObserverMock {
      observe() {}
      unobserve() {}
      disconnect() {}
    }
    vi.stubGlobal("ResizeObserver", ResizeObserverMock);
  });
  it("renders loading skeletons", () => {
    render(
      <CategoryTabs
        categories={[]}
        value="all"
        onValueChange={vi.fn()}
        isLoading
      />,
    );

    expect(document.querySelectorAll("[data-slot='skeleton'], .animate-pulse").length).toBeGreaterThan(0);
  });

  it("changes tabs", () => {
    const onValueChange = vi.fn();

    render(
      <CategoryTabs
        categories={[youtubeCategory("cat-1", { name: "Fitness" })]}
        value="all"
        onValueChange={onValueChange}
        showFavoritesTab
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: /Favorites/i }));
    fireEvent.click(screen.getByRole("button", { name: "Fitness" }));

    expect(onValueChange).toHaveBeenCalledWith("favorites");
    expect(onValueChange).toHaveBeenCalledWith(
      "11111111-1111-4111-8111-111111111111",
    );
  });
});

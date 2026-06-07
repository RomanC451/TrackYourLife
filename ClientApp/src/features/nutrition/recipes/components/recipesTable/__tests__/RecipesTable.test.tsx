import { act, fireEvent, render, screen } from "@testing-library/react";
import { renderHook } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { recipe } from "@/features/nutrition/__tests__/fixtures";

import RecipesTable from "../RecipesTable";
import useRecipesTable from "../useRecipesTable";

const mockUseSuspenseQuery = vi.fn();
const mockNavigate = vi.fn();
const mockMutate = vi.fn();

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

vi.mock("@tanstack/react-router", () => ({
  Link: ({ children, to }: { children: React.ReactNode; to: string }) => (
    <a href={to}>{children}</a>
  ),
  useNavigate: () => mockNavigate,
}));

vi.mock("../../../mutations/useDeleteRecipesMutation", () => ({
  default: () => ({
    mutate: mockMutate,
    isDelayedPending: false,
    isPending: false,
  }),
}));

vi.mock("../RecipesTableRowActionsMenu", () => ({
  default: () => null,
}));

vi.mock("@/components/ui/spinner", () => ({
  default: () => null,
}));

vi.mock("@/components/ui/checkbox", () => ({
  Checkbox: ({
    checked,
    onCheckedChange,
    "aria-label": ariaLabel,
  }: {
    checked?: boolean | "indeterminate";
    onCheckedChange?: (value: boolean) => void;
    "aria-label"?: string;
  }) => (
    <input
      type="checkbox"
      aria-label={ariaLabel}
      checked={checked === "indeterminate" ? false : !!checked}
      onChange={(event) => onCheckedChange?.(event.target.checked)}
    />
  ),
}));

describe("RecipesTable", () => {
  it("renders recipe rows when data exists", () => {
    mockUseSuspenseQuery.mockReturnValue({
      data: [recipe("recipe-1", "Soup"), recipe("recipe-2", "Salad")],
    });

    render(<RecipesTable />);

    expect(screen.getByText("Recipes")).toBeInTheDocument();
    expect(screen.getByText("Soup")).toBeInTheDocument();
    expect(screen.getByText("Salad")).toBeInTheDocument();
  });

  it("renders the empty state when there are no recipes", () => {
    mockUseSuspenseQuery.mockReturnValue({ data: [] });

    render(<RecipesTable />);

    expect(screen.getByText("You have no recipes")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Create a new recipe" }));
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/nutrition/recipes/create",
    });
  });

  it("disables bulk delete when no rows are selected", () => {
    render(
      <RecipesTable.Header
        rowSelection={{}}
        resetRowSelection={vi.fn()}
      />,
    );

    expect(
      screen.getByRole("button", { name: "Delete selected : 0" }),
    ).toBeDisabled();
  });

  it("deletes selected recipes and clears the selection on success", () => {
    const resetRowSelection = vi.fn();

    render(
      <RecipesTable.Header
        rowSelection={{ "recipe-1": true, "recipe-2": true }}
        resetRowSelection={resetRowSelection}
      />,
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Delete selected : 2" }),
    );

    expect(mockMutate).toHaveBeenCalledWith(
      { ids: ["recipe-1", "recipe-2"] },
      expect.objectContaining({ onSuccess: expect.any(Function) }),
    );

    const onSuccess = mockMutate.mock.calls[0][1]?.onSuccess;
    onSuccess?.();
    expect(resetRowSelection).toHaveBeenCalled();
  });

  it("renders a create recipe link in the header", () => {
    render(
      <RecipesTable.Header rowSelection={{}} resetRowSelection={vi.fn()} />,
    );

    expect(screen.getByRole("link", { name: "Create Recipe" })).toHaveAttribute(
      "href",
      "/nutrition/recipes/create",
    );
  });

  it("applies deleting opacity to recipe rows", () => {
    const { result } = renderHook(() =>
      useRecipesTable([recipe("recipe-1", "Soup", { isDeleting: true })]),
    );

    const { container } = render(
      <RecipesTable.Content table={result.current.table} />,
    );

    expect(container.querySelector(".opacity-50")).toBeInTheDocument();
  });

  it("clears row selection after a successful bulk delete", () => {
    mockUseSuspenseQuery.mockReturnValue({
      data: [recipe("recipe-1", "Soup"), recipe("recipe-2", "Salad")],
    });

    render(<RecipesTable />);

    fireEvent.click(screen.getByRole("checkbox", { name: "Select all" }));
    expect(
      screen.getByRole("button", { name: "Delete selected : 2" }),
    ).toBeEnabled();

    fireEvent.click(
      screen.getByRole("button", { name: "Delete selected : 2" }),
    );

    const onSuccess = mockMutate.mock.calls.at(-1)?.[1]?.onSuccess;
    act(() => {
      onSuccess?.();
    });

    expect(
      screen.getByRole("button", { name: "Delete selected : 0" }),
    ).toBeDisabled();
  });

  it("renders an empty table body row when there are no rows", () => {
    const { result } = renderHook(() => useRecipesTable([]));

    const { container } = render(
      <RecipesTable.Content table={result.current.table} />,
    );

    expect(container.querySelector("tbody tr td[colspan]")).toBeInTheDocument();
  });
});

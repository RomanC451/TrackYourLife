import { fireEvent, render, screen } from "@testing-library/react";
import type { CellContext, HeaderContext } from "@tanstack/react-table";
import { describe, expect, it, vi } from "vitest";

import { recipe } from "@/features/nutrition/__tests__/fixtures";
import type { RecipeDto } from "@/services/openapi";

import { recipesTableColumns } from "../recipesTableColumns";

vi.mock("../RecipesTableRowActionsMenu", () => ({
  default: () => <div data-testid="row-actions" />,
}));

vi.mock("@/components/ui/spinner", () => ({
  default: () => <div data-testid="spinner" />,
}));

function renderCell(accessorKey: string, original: RecipeDto) {
  const column = recipesTableColumns.find(
    (entry) => "accessorKey" in entry && entry.accessorKey === accessorKey,
  );
  const Cell = column?.cell as (
    context: CellContext<RecipeDto, unknown>,
  ) => React.ReactNode;

  render(
    <Cell
      row={{ original } as CellContext<RecipeDto, unknown>["row"]}
      {...({} as Omit<CellContext<RecipeDto, unknown>, "row">)}
    />,
  );
}

describe("recipesTableColumns", () => {
  it("includes selection, actions, and recipe detail columns", () => {
    expect(
      recipesTableColumns.map((column) =>
        "accessorKey" in column ? column.accessorKey : column.id,
      ),
    ).toEqual([
      "select",
      "actions",
      "name",
      "portions",
      "calories",
      "carbs",
      "fat",
      "protein",
    ]);
  });

  it("renders a spinner while a recipe is deleting", () => {
    const selectColumn = recipesTableColumns.find(
      (column) => column.id === "select",
    );
    const Cell = selectColumn?.cell as (
      context: CellContext<RecipeDto, unknown>,
    ) => React.ReactNode;

    render(
      <Cell
        row={
          {
            original: recipe("recipe-1", "Soup", { isDeleting: true }),
            getIsSelected: () => false,
            toggleSelected: vi.fn(),
          } as unknown as CellContext<RecipeDto, unknown>["row"]
        }
        {...({} as Omit<CellContext<RecipeDto, unknown>, "row">)}
      />,
    );

    expect(screen.getByTestId("spinner")).toBeInTheDocument();
  });

  it("renders recipe name in the name column", () => {
    renderCell("name", recipe("recipe-1", "Overnight oats"));
    expect(screen.getByText("Overnight oats")).toBeInTheDocument();
  });

  it("toggles sorting from the name column header", () => {
    const toggleSorting = vi.fn();
    const nameColumn = recipesTableColumns.find(
      (column) => "accessorKey" in column && column.accessorKey === "name",
    );
    const Header = nameColumn?.header as (
      context: HeaderContext<RecipeDto, unknown>,
    ) => React.ReactNode;

    render(
      <Header
        column={
          {
            toggleSorting,
            getIsSorted: () => false,
          } as unknown as HeaderContext<RecipeDto, unknown>["column"]
        }
        {...({} as Omit<HeaderContext<RecipeDto, unknown>, "column">)}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: /name/i }));
    expect(toggleSorting).toHaveBeenCalled();
  });
});

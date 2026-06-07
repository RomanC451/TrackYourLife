import { act, renderHook } from "@testing-library/react";
import type { ColumnFiltersState, SortingState } from "@tanstack/react-table";
import { useState } from "react";
import { describe, expect, it, vi } from "vitest";

vi.mock(
  "@/features/nutrition/recipes/components/recipesTable/RecipesTableRowActionsMenu",
  () => ({
    default: () => null,
  }),
);

vi.mock("@/components/ui/spinner", () => ({
  default: () => null,
}));

import { recipe } from "@/features/nutrition/__tests__/fixtures";
import { recipesTableColumns } from "@/features/nutrition/recipes/components/recipesTable/recipesTableColumns";

import useTable from "../useTable";

describe("useTable", () => {
  const data = [
    recipe("recipe-2", "Salad", { portions: 1 }),
    recipe("recipe-1", "Soup", { portions: 3 }),
  ];

  function useTestTable() {
    const [sorting, setSorting] = useState<SortingState>([]);
    const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
    const [columnVisibility, setColumnVisibility] = useState({});
    const [rowSelection, setRowSelection] = useState({});

    return useTable({
      data,
      columns: recipesTableColumns,
      setSorting,
      setColumnFilters,
      sorting,
      columnFilters,
      columnVisibility,
      setColumnVisibility,
      rowSelection,
      onRowSelectionChange: setRowSelection,
    });
  }

  it("exposes all rows with stable ids", () => {
    const { result } = renderHook(() => useTestTable());

    expect(result.current.getRowModel().rows).toHaveLength(2);
    expect(result.current.getRowModel().rows.map((row) => row.id)).toEqual([
      "recipe-2",
      "recipe-1",
    ]);
  });

  it("updates row selection state", () => {
    const { result } = renderHook(() => useTestTable());

    act(() => {
      result.current.getRow("recipe-1")?.toggleSelected(true);
    });

    expect(result.current.getState().rowSelection).toEqual({
      "recipe-1": true,
    });
  });
});

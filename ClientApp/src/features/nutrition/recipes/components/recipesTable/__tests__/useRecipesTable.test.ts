import { act, renderHook } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("../RecipesTableRowActionsMenu", () => ({
  default: () => null,
}));

vi.mock("@/components/ui/spinner", () => ({
  default: () => null,
}));

import { recipe } from "@/features/nutrition/__tests__/fixtures";

import useRecipesTable from "../useRecipesTable";

describe("useRecipesTable", () => {
  const data = [recipe("recipe-1", "Soup"), recipe("recipe-2", "Salad")];

  it("returns a table with recipe rows", () => {
    const { result } = renderHook(() => useRecipesTable(data));

    expect(result.current.table.getRowModel().rows).toHaveLength(2);
    expect(
      result.current.table.getRowModel().rows[0].original.name,
    ).toBe("Soup");
  });

  it("tracks row selection separately from the table instance", () => {
    const { result } = renderHook(() => useRecipesTable(data));

    act(() => {
      result.current.table.getRow("recipe-2")?.toggleSelected(true);
    });

    expect(result.current.rowSelection).toEqual({ "recipe-2": true });
  });
});

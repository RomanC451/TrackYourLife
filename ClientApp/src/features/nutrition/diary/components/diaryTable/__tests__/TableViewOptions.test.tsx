import { fireEvent, render, screen } from "@testing-library/react";
import type { ColumnFiltersState, SortingState } from "@tanstack/react-table";
import { useState } from "react";
import { describe, expect, it, vi } from "vitest";

import { recipe } from "@/features/nutrition/__tests__/fixtures";
import useTable from "@/features/nutrition/common/hooks/useTable";
import { recipesTableColumns } from "@/features/nutrition/recipes/components/recipesTable/recipesTableColumns";

import { TableViewOptions } from "../TableViewOptions";

vi.mock(
  "@/features/nutrition/recipes/components/recipesTable/RecipesTableRowActionsMenu",
  () => ({ default: () => null }),
);

vi.mock("@/components/ui/spinner", () => ({ default: () => null }));

vi.mock("@/components/ui/dropdown-menu", () => ({
  DropdownMenu: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuTrigger: ({
    children,
    asChild,
  }: {
    children: React.ReactNode;
    asChild?: boolean;
  }) => (asChild ? children : <div>{children}</div>),
  DropdownMenuContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuLabel: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuCheckboxItem: ({
    children,
    checked,
    onCheckedChange,
  }: {
    children: React.ReactNode;
    checked: boolean;
    onCheckedChange: (value: boolean) => void;
  }) => (
    <button type="button" onClick={() => onCheckedChange(!checked)}>
      {children}
    </button>
  ),
  DropdownMenuSeparator: () => <hr />,
}));

function TableHarness() {
  const data = [recipe("recipe-1", "Soup")];
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [columnVisibility, setColumnVisibility] = useState({});
  const [rowSelection, setRowSelection] = useState({});

  const table = useTable({
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

  return <TableViewOptions table={table} disabled={false} />;
}

describe("TableViewOptions", () => {
  it("renders the view options trigger", () => {
    render(<TableHarness />);
    expect(screen.getByRole("button", { name: /view/i })).toBeInTheDocument();
  });

  it("toggles column visibility from the menu", () => {
    render(<TableHarness />);

    const columnToggle = screen.getByRole("button", { name: "name" });
    expect(columnToggle).toBeInTheDocument();
    fireEvent.click(columnToggle);
  });

  it("disables the trigger when requested", () => {
    function DisabledHarness() {
      const data = [recipe("recipe-1", "Soup")];
      const [sorting, setSorting] = useState<SortingState>([]);
      const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
      const [columnVisibility, setColumnVisibility] = useState({});
      const [rowSelection, setRowSelection] = useState({});

      const table = useTable({
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

      return <TableViewOptions table={table} disabled />;
    }

    render(<DisabledHarness />);
    expect(screen.getByRole("button", { name: /view/i })).toBeDisabled();
  });
});

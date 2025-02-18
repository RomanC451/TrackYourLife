import { useMemo, useState } from "react";
import {
  ColumnFiltersState,
  RowSelectionState,
  SortingState,
  VisibilityState,
} from "@tanstack/react-table";
import { useLocalStorage } from "usehooks-ts";

import useTable from "@/features/nutrition/common/hooks/useTable";
import { RecipeDto } from "@/services/openapi";

import { recipesTableColumns } from "./recipesTableColumns";

function useRecipesTable(data: RecipeDto[]) {
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [columnVisibility, setColumnVisibility] =
    useLocalStorage<VisibilityState>("recipesTableVisibility", {});
  const [rowSelection, setRowSelection] = useState<RowSelectionState>({});

  const tableData = useMemo(() => data, [data]);

  const table = useTable({
    data: tableData,
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

  return { table, rowSelection };
}

export default useRecipesTable;

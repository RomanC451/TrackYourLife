import {
  ColumnFiltersState,
  SortingState,
  VisibilityState,
} from "@tanstack/react-table";
import { useMemo, useState } from "react";
import { useLocalStorage } from "usehooks-ts";
import { RecipeDto } from "~/services/openapi";
import { recipesTableColumns } from "../../components/recipes/recipesTable/recipesTableColumns";
import useTable from "../useTable";

function useRecipesTable(data: RecipeDto[]) {
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [columnVisibility, setColumnVisibility] =
    useLocalStorage<VisibilityState>("recipesTableVisibility", {});

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
  });

  return { table };
}

export default useRecipesTable;

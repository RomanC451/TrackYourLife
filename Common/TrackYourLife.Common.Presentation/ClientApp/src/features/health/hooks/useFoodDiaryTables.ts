import React from "react";
import { useLocalStorage } from "usehooks-ts";

import {
  ColumnDef,
  ColumnFiltersState,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  SortingState,
  useReactTable,
  VisibilityState
} from "@tanstack/react-table";

interface UseDiaryTableProps<TData> {
  data: TData[];
  columns: ColumnDef<TData>[];
  setSorting: React.Dispatch<React.SetStateAction<SortingState>>;
  setColumnFilters: React.Dispatch<React.SetStateAction<ColumnFiltersState>>;
  sorting: SortingState;
  columnFilters: ColumnFiltersState;
  columnVisibility: VisibilityState;
  setColumnVisibility: React.Dispatch<React.SetStateAction<VisibilityState>>;
}

const useDiaryTable = <TData>({
  data,
  columns,
  setSorting,
  setColumnFilters,
  sorting,
  columnFilters,
  columnVisibility,
  setColumnVisibility
}: UseDiaryTableProps<TData>) => {
  const table = useReactTable({
    data,
    columns,
    onSortingChange: setSorting,
    onColumnFiltersChange: setColumnFilters,
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    onColumnVisibilityChange: setColumnVisibility,
    state: {
      sorting,
      columnFilters,
      columnVisibility
    }
  });
  return table;
};

type TDiaryData<TData> = {
  breakfast: TData[];
  lunch: TData[];
  dinner: TData[];
  snacks: TData[];
};

const useFoodDiaryTables = <TData>(
  diaryData: TDiaryData<TData>,
  columns: ColumnDef<TData>[]
) => {
  const [sorting, setSorting] = React.useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = React.useState<ColumnFiltersState>(
    []
  );
  const [columnVisibility, setColumnVisibility] =
    useLocalStorage<VisibilityState>("foodDiaryVisbility", {
      carbs: false,
      fat: false,
      protein: false
    });

  const breakfastTable = useDiaryTable<TData>({
    data: diaryData.breakfast,
    columns,
    setSorting,
    setColumnFilters,
    sorting,
    columnFilters,
    columnVisibility,
    setColumnVisibility
  });

  const lunchTable = useDiaryTable<TData>({
    data: diaryData.lunch,
    columns,
    setSorting,
    setColumnFilters,
    sorting,
    columnFilters,
    columnVisibility,
    setColumnVisibility
  });

  const dinnerTable = useDiaryTable<TData>({
    data: diaryData.dinner,
    columns,
    setSorting,
    setColumnFilters,
    sorting,
    columnFilters,
    columnVisibility,
    setColumnVisibility
  });

  const snacksTable = useDiaryTable<TData>({
    data: diaryData.snacks,
    columns,
    setSorting,
    setColumnFilters,
    sorting,
    columnFilters,
    columnVisibility,
    setColumnVisibility
  });

  return {
    breakfastTable,
    lunchTable,
    dinnerTable,
    snacksTable
  };
};

export default useFoodDiaryTables;

"use no memo";

import { useMemo, useState } from "react";
import {
  ColumnFiltersState,
  SortingState,
  VisibilityState,
} from "@tanstack/react-table";
import { useLocalStorage } from "usehooks-ts";

import useTable from "@/features/nutrition/common/hooks/useTable";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { DateOnly } from "@/lib/date";
import { NutritionDiaryDto } from "@/services/openapi";

import { nutritionDiariesQueryOptions } from "../../queries/useDiaryQuery";
import { foodDiaryTableColumns } from "./foodDiaryTableColumns";

const useFoodDiaryTables = (date: DateOnly) => {
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [columnVisibility, setColumnVisibility] =
    useLocalStorage<VisibilityState>("foodDiaryVisibility", {
      carbs: true,
      fat: true,
      protein: true,
    });

  const { query: foodDiariesQuery, pendingState } = useCustomQuery(
    nutritionDiariesQueryOptions.byDate(date),
  );

  function useCreateTable(data: NutritionDiaryDto[]) {
    return useTable({
      data,
      columns: foodDiaryTableColumns,
      setSorting,
      setColumnFilters,
      sorting,
      columnFilters,
      columnVisibility,
      setColumnVisibility,
      onRowSelectionChange: () => {},
      rowSelection: {},
    });
  }

  const tableData = useMemo(() => {
    return {
      Breakfast: foodDiariesQuery.data?.diaries.Breakfast ?? [],
      Lunch: foodDiariesQuery.data?.diaries.Lunch ?? [],
      Dinner: foodDiariesQuery.data?.diaries.Dinner ?? [],
      Snacks: foodDiariesQuery.data?.diaries.Snacks ?? [],
    };
  }, [foodDiariesQuery.data]);

  const breakfastTable = useCreateTable(tableData.Breakfast ?? []);

  const lunchTable = useCreateTable(tableData.Lunch ?? []);

  const dinnerTable = useCreateTable(tableData.Dinner ?? []);

  const snacksTable = useCreateTable(tableData.Snacks ?? []);

  return {
    tables: {
      breakfastTable,
      lunchTable,
      dinnerTable,
      snacksTable,
    },
    pendingState,
  };
};

export default useFoodDiaryTables;

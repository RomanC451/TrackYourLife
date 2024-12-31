import { useMemo, useState } from "react";
import { useLocalStorage } from "usehooks-ts";

import {
  ColumnFiltersState,
  SortingState,
  VisibilityState,
} from "@tanstack/react-table";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { NutritionDiaryDto } from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import { foodDiaryTableColumns } from "../../components/foodDiary/foodDiaryTable/foodDiaryTableColumns";
import useNutritionDiariesQuery from "../../queries/foodDiaries/useNutritionDiariesQuery";
import useTable from "../useTable";

const useFoodDiaryTables = (date: DateOnly) => {
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [columnVisibility, setColumnVisibility] =
    useLocalStorage<VisibilityState>("foodDiaryVisibility", {
      carbs: false,
      fat: false,
      protein: false,
    });

  const foodDiariesQuery = useNutritionDiariesQuery(date);

  const loadingState = useDelayedLoading(foodDiariesQuery.data);

  function createTable(data: NutritionDiaryDto[]) {
    return useTable({
      data,
      columns: foodDiaryTableColumns,
      setSorting,
      setColumnFilters,
      sorting,
      columnFilters,
      columnVisibility,
      setColumnVisibility,
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

  const breakfastTable = createTable(tableData.Breakfast ?? []);

  const lunchTable = createTable(tableData.Lunch ?? []);

  const dinnerTable = createTable(tableData.Dinner ?? []);

  const snacksTable = createTable(tableData.Snacks ?? []);

  return {
    tables: {
      breakfastTable,
      lunchTable,
      dinnerTable,
      snacksTable,
    },
    loadingState,
  };
};

export default useFoodDiaryTables;

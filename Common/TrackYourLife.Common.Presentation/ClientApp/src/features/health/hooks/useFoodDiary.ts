import { useEffect, useState } from "react";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { DateOnly, getDateOnly } from "~/utils/date";

import { useQuery } from "@tanstack/react-query";

import { FoodDiaryApi, FoodDiaryEntryListResponse } from "~/services/openapi";
import {
  FoodDiaryEntry,
  columns,
} from "../components/calories/foodDiary/diaryTable/columns";
import useFoodDiaryTables from "./useFoodDiaryTables";

type TFoodDiaryData = {
  breakfast: FoodDiaryEntry[];
  lunch: FoodDiaryEntry[];
  dinner: FoodDiaryEntry[];
  snacks: FoodDiaryEntry[];
};

const foodDiaryApi = new FoodDiaryApi();

const useFoodDiary = (date?: Date) => {
  const [tableData, setTableData] = useState<TFoodDiaryData>({
    breakfast: [],
    lunch: [],
    dinner: [],
    snacks: [],
  });

  const query = useQuery({
    queryKey: ["foodDiary", getDateOnly(date)],
    queryFn: () =>
      foodDiaryApi.getByDate(getDateOnly(date)).then((res) => res.data),
    // getFoodDiaryListRequest({ fetchRequest, date: getDateOnly(date) }),

    staleTime: Infinity,
  });

  const loadingState = useDelayedLoading(100, query.data);

  useEffect(() => {
    if (query.data === undefined) return;

    const data: TFoodDiaryData = Object.keys(query.data).reduce(
      (acc, key) => {
        const items = query.data[key as keyof FoodDiaryEntryListResponse];
        if (Array.isArray(items)) {
          acc[key as keyof TFoodDiaryData] = query.data[
            key as keyof FoodDiaryEntryListResponse
          ].map((item) => {
            const nutritionMultiplier =
              item.servingSize.nutritionMultiplier * item.quantity;

            return {
              name: item.food.name,
              brandName: item.food.brandName,
              calories: parseFloat(
                (
                  item.food.nutritionalContents.energy.value *
                  nutritionMultiplier
                ).toFixed(1),
              ),
              quantity: `${item.quantity * item.servingSize.value} ${
                item.servingSize.unit
              }`,
              carbs: parseFloat(
                (
                  nutritionMultiplier *
                  item.food.nutritionalContents.carbohydrates
                ).toFixed(1),
              ),
              fat: parseFloat(
                (
                  nutritionMultiplier * item.food.nutritionalContents.fat
                ).toFixed(1),
              ),
              protein: parseFloat(
                (
                  nutritionMultiplier * item.food.nutritionalContents.protein
                ).toFixed(1),
              ),
              meal: item.mealType,
              id: item.id,
              foodId: item.food.id,
              servingSize: item.servingSize,
              nrOfServings: item.quantity,
              food: item.food,
              date: item.entryDate as DateOnly,
            };
          });
        }
        return acc;
      },
      {
        breakfast: [],
        lunch: [],
        dinner: [],
        snacks: [],
      } as TFoodDiaryData,
    );

    setTableData(data);
  }, [query.data]);

  const tables = useFoodDiaryTables(
    {
      breakfast: tableData?.breakfast,
      lunch: tableData?.lunch,
      dinner: tableData?.dinner,
      snacks: tableData?.snacks,
    },
    columns,
  );

  return { tables, loadingState };
};

export default useFoodDiary;

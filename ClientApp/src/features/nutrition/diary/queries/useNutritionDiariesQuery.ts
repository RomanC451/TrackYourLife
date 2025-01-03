import { useQuery } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import {
  GetNutritionDiariesByDateResponse,
  MealTypes,
  NutritionDiariesApi,
  NutritionDiaryDto,
} from "@/services/openapi";
import { PartialWithRequired } from "@/types/defaultTypes";

import { QUERY_KEYS } from "../../common/data/queryKeys";

const nutritionDiariesApi = new NutritionDiariesApi();

const useNutritionDiariesQuery = (date: DateOnly) => {
  const nutritionDiariesQuery = useQuery({
    queryKey: [QUERY_KEYS.nutritionDiaries, date],
    queryFn: () =>
      nutritionDiariesApi
        .getNutritionDiariesByDate(date)
        .then((res) => res.data),
  });

  return nutritionDiariesQuery;
};

export const prefetchNutritionDiariesQuery = (date: DateOnly) => {
  queryClient.prefetchQuery({
    queryKey: [QUERY_KEYS.nutritionDiaries, date],
    queryFn: () =>
      nutritionDiariesApi
        .getNutritionDiariesByDate(date)
        .then((res) => res.data),
  });
};

export const invalidateNutritionDiariesQuery = (date?: DateOnly) => {
  queryClient.invalidateQueries({
    queryKey: date
      ? [QUERY_KEYS.nutritionDiaries, date]
      : [QUERY_KEYS.nutritionDiaries],
  });
};

type SetNutritionDiariesQueryDataProps = {
  date: DateOnly;
  mealType: MealTypes;
  filter?: (entry: NutritionDiaryDto) => boolean;
  newDiary?: NutritionDiaryDto;
  updatedDiary?: PartialWithRequired<NutritionDiaryDto, "id">;
  invalidate?: boolean;
};

export const setNutritionDiariesQueryData = ({
  date,
  mealType,
  filter,
  newDiary,
  updatedDiary,
  invalidate = false,
}: SetNutritionDiariesQueryDataProps) => {
  queryClient.setQueryData(
    [QUERY_KEYS.nutritionDiaries, date],
    (oldData: GetNutritionDiariesByDateResponse) => {
      const newData = {
        diaries: {
          ...oldData.diaries,
        },
      };

      if (filter)
        newData.diaries[mealType] = [
          ...oldData.diaries[mealType].filter(filter),
        ];

      if (newDiary)
        newData.diaries[mealType] = [...oldData.diaries[mealType], newDiary];

      if (updatedDiary)
        newData.diaries[mealType] = [
          ...oldData.diaries[mealType].map((entry) =>
            entry.id === updatedDiary.id
              ? { ...entry, ...updatedDiary }
              : entry,
          ),
        ];

      return newData;
    },
  );

  if (invalidate) invalidateNutritionDiariesQuery(date);
};

export default useNutritionDiariesQuery;

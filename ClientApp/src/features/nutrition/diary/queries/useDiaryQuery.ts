import { queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import {
  FoodDiariesApi,
  GetNutritionDiariesByDateResponse,
  MealTypes,
  NutritionDiariesApi,
  NutritionDiaryDto,
  RecipeDiariesApi,
} from "@/services/openapi";
import { PartialWithRequired } from "@/types/defaultTypes";

const foodDiariesApi = new FoodDiariesApi();

export const foodDiariesQueryKeys = {
  all: ["foodDiaries"] as const,
  byId: (id: string) => [...foodDiariesQueryKeys.all, id] as const,
};

export const foodDiariesQueryOptions = {
  byId: (id: string) =>
    queryOptions({
      queryKey: foodDiariesQueryKeys.byId(id),
      queryFn: () =>
        foodDiariesApi.getFoodDiaryById(id).then((res) => res.data),
    }),
};

const nutritionDiariesApi = new NutritionDiariesApi();

export const nutritionDiariesQueryKeys = {
  all: ["nutritionDiaries"] as const,
  byDate: (date: DateOnly) => [...nutritionDiariesQueryKeys.all, date] as const,
  overview: (startDate: DateOnly, endDate: DateOnly) =>
    [...nutritionDiariesQueryKeys.all, "overview", startDate, endDate] as const,
};

export const nutritionDiariesQueryOptions = {
  byDate: (date: DateOnly) =>
    queryOptions({
      queryKey: nutritionDiariesQueryKeys.byDate(date),
      queryFn: () =>
        nutritionDiariesApi
          .getNutritionDiariesByDate(date)
          .then((res) => res.data),
    }),
  overview: (startDate: DateOnly, endDate: DateOnly) =>
    queryOptions({
      queryKey: nutritionDiariesQueryKeys.overview(startDate, endDate),
      queryFn: () =>
        nutritionDiariesApi
          .getNutritionOverviewByPeriod(startDate, endDate)
          .then((res) => res.data),
      placeholderData: (prev) => prev,
    }),
};

const recipeDiariesApi = new RecipeDiariesApi();

export const recipeDiariesQueryKeys = {
  all: ["recipeDiaries"] as const,
  byId: (id: string) => [...recipeDiariesQueryKeys.all, id] as const,
};

export const recipeDiariesQueryOptions = {
  byId: (id: string) =>
    queryOptions({
      queryKey: recipeDiariesQueryKeys.byId(id),
      queryFn: () =>
        recipeDiariesApi.getRecipeDiaryById(id).then((res) => res.data),
    }),
};

type SetNutritionDiariesQueryDataProps = {
  date: DateOnly;
  mealType: MealTypes;
  deleteDiaryId?: string;
  newDiary?: NutritionDiaryDto;
  updatedDiary?: PartialWithRequired<NutritionDiaryDto, "id">;
};

export const setNutritionDiariesQueryData = ({
  date,
  mealType,
  deleteDiaryId,
  newDiary,
  updatedDiary,
}: SetNutritionDiariesQueryDataProps) => {
  queryClient.setQueryData(
    nutritionDiariesQueryKeys.byDate(date),
    (oldData: GetNutritionDiariesByDateResponse) => {
      const newData = {
        diaries: {
          ...oldData.diaries,
        },
      };

      if (deleteDiaryId)
        newData.diaries[mealType] = [
          ...oldData.diaries[mealType].map((entry) =>
            entry.id !== deleteDiaryId ? entry : { ...entry, isDeleting: true },
          ),
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
};

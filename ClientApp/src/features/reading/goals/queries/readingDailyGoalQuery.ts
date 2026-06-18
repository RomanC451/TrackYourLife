import { queryOptions } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";

import { getDateOnly } from "@/lib/date";
import { GoalType, GoalsApi, type GoalDto } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

const goalsApi = new GoalsApi();

export const readingDailyGoalQueryKeys = {
  all: ["readingDailyGoal"] as const,
  byDate: (date: string) => [...readingDailyGoalQueryKeys.all, date] as const,
};

export const readingDailyGoalQueryOptions = {
  current: () => {
    const today = getDateOnly(new Date());
    return queryOptions({
      queryKey: readingDailyGoalQueryKeys.byDate(today),
      queryFn: async (): Promise<GoalDto | null> => {
        try {
          const res = await goalsApi.getGoal(GoalType.ReadingPages, today);
          return res.data;
        } catch (e) {
          const err = e as ApiError;
          if (err.response?.status === StatusCodes.NOT_FOUND) {
            return null;
          }
          throw e;
        }
      },
    });
  },
};

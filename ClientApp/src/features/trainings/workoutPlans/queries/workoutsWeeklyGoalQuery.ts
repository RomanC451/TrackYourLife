import { queryOptions } from "@tanstack/react-query";
import { endOfWeek, startOfWeek } from "date-fns";
import { StatusCodes } from "http-status-codes";

import type { DateOnly } from "@/lib/date";
import { getDateOnly } from "@/lib/date";
import { GoalType, GoalsApi, type GoalDto } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

const goalsApi = new GoalsApi();

export function getCurrentWeekDateRange(): { weekStart: DateOnly; weekEnd: DateOnly } {
  const today = new Date();
  return {
    weekStart: getDateOnly(startOfWeek(today, { weekStartsOn: 1 })),
    weekEnd: getDateOnly(endOfWeek(today, { weekStartsOn: 1 })),
  };
}

export const workoutsWeeklyGoalQueryKeys = {
  all: ["workoutsWeeklyGoal"] as const,
  byDate: (date: DateOnly) => [...workoutsWeeklyGoalQueryKeys.all, date] as const,
};

export const workoutsWeeklyGoalQueryOptions = {
  current: () => {
    const today = getDateOnly(new Date());
    return queryOptions({
      queryKey: workoutsWeeklyGoalQueryKeys.byDate(today),
      queryFn: async (): Promise<GoalDto | null> => {
        try {
          const res = await goalsApi.getGoal(GoalType.Workouts, today);
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

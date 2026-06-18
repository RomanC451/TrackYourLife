import { useCustomMutation } from "@/hooks/useCustomMutation";
import { getDateOnly } from "@/lib/date";
import { GoalPeriod, GoalType, GoalsApi } from "@/services/openapi";

import { readingDashboardQueryKeys } from "../../queries/readingQueries";
import { readingDailyGoalQueryKeys } from "../queries/readingDailyGoalQuery";

const goalsApi = new GoalsApi();

const useSetReadingDailyGoalMutation = () =>
  useCustomMutation({
    mutationFn: (variables: { value: number; existingGoalId?: string }) => {
      if (variables.existingGoalId) {
        return goalsApi
          .updateGoal({
            id: variables.existingGoalId,
            type: GoalType.ReadingPages,
            value: variables.value,
            period: GoalPeriod.Day,
            startDate: getDateOnly(new Date()),
          })
          .then(() => undefined);
      }
      return goalsApi
        .addGoal({
          value: variables.value,
          type: GoalType.ReadingPages,
          period: GoalPeriod.Day,
          startDate: getDateOnly(new Date()),
          force: false,
        })
        .then(() => undefined);
    },
    meta: {
      invalidateQueries: [
        readingDailyGoalQueryKeys.all,
        readingDashboardQueryKeys.all,
      ],
      onSuccessToast: {
        type: "success",
        message: "Daily reading goal saved.",
      },
    },
  });

export default useSetReadingDailyGoalMutation;

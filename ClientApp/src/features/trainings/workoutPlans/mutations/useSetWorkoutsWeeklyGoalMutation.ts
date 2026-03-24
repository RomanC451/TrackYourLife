import { useCustomMutation } from "@/hooks/useCustomMutation";
import { getDateOnly } from "@/lib/date";
import { GoalPeriod, GoalType, GoalsApi } from "@/services/openapi";

import { allTrainingsOverviewKeys } from "../../overview/queries/trainingsOverviewKeys";
import { workoutsWeeklyGoalQueryKeys } from "../queries/workoutsWeeklyGoalQuery";

const goalsApi = new GoalsApi();

const useSetWorkoutsWeeklyGoalMutation = () => {
  return useCustomMutation({
    mutationFn: (variables: { value: number; existingGoalId?: string }) => {
      if (variables.existingGoalId) {
        return goalsApi
          .updateGoal({
            id: variables.existingGoalId,
            type: GoalType.Workouts,
            value: variables.value,
            period: GoalPeriod.Week,
            startDate: getDateOnly(new Date()),
          })
          .then(() => undefined);
      }
      return goalsApi
        .addGoal({
          value: variables.value,
          type: GoalType.Workouts,
          period: GoalPeriod.Week,
          startDate: getDateOnly(new Date()),
          force: false,
        })
        .then(() => undefined);
    },
    meta: {
      invalidateQueries: [workoutsWeeklyGoalQueryKeys.all, ...allTrainingsOverviewKeys],
      onSuccessToast: {
        type: "success",
        message: "Weekly workout goal saved.",
      },
    },
  });
};

export default useSetWorkoutsWeeklyGoalMutation;

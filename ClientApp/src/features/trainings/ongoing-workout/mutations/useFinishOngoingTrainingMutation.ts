import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { OngoingTrainingDto, OngoingTrainingsApi } from "@/services/openapi";

import { exercisesQueryKeys } from "../../exercises/queries/exercisesQuery";
import { allTrainingsOverviewKeys } from "../../overview/queries/trainingsOverviewKeys";
import { trainingsQueryKeys } from "../../trainings/queries/trainingsQueries";
import { workoutPlansQueryKeys } from "../../workoutPlans/queries/workoutPlansQueries";
import { workoutStreakQueryKeys } from "../../workoutPlans/queries/workoutStreakQuery";
import { ongoingTrainingsQueryKeys } from "../queries/ongoingTrainingsQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

type Variables = {
  ongoingTraining: OngoingTrainingDto;
  caloriesBurned?: number;
};

const useFinishOngoingTrainingMutation = () => {
  const finishOngoingTrainingMutation = useCustomMutation({
    mutationFn: ({ ongoingTraining, caloriesBurned }: Variables) =>
      ongoingTrainingsApi.finishOngoingTraining(ongoingTraining.id, {
        caloriesBurned: caloriesBurned,
      }),
    meta: {
      awaitInvalidationQuery: ongoingTrainingsQueryKeys.active,
      invalidateQueries: [
        exercisesQueryKeys.all,
        trainingsQueryKeys.all,
        workoutPlansQueryKeys.all,
        workoutStreakQueryKeys.all,
        ...allTrainingsOverviewKeys,
      ],
    },

    onSuccess: (_, { ongoingTraining }) => {
      queryClient.removeQueries({
        queryKey: ongoingTrainingsQueryKeys.byId(ongoingTraining.id),
      });
    },
  });

  return finishOngoingTrainingMutation;
};

export default useFinishOngoingTrainingMutation;

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { OngoingTrainingDto, OngoingTrainingsApi } from "@/services/openapi";

import { exercisesQueryKeys } from "../../exercises/queries/exercisesQuery";
import { trainingsQueryKeys } from "../../trainings/queries/trainingsQueries";
import { ongoingTrainingsQueryKeys } from "../queries/ongoingTrainingsQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

type Variables = {
  ongoingTraining: OngoingTrainingDto;
};

const useFinishOngoingTrainingMutation = () => {
  const finishOngoingTrainingMutation = useCustomMutation({
    mutationFn: ({ ongoingTraining }: Variables) =>
      ongoingTrainingsApi.finishOngoingTraining(ongoingTraining.id),

    meta: {
      invalidateQueries: [
        ongoingTrainingsQueryKeys.active,
        exercisesQueryKeys.all,
        trainingsQueryKeys.all,
      ],
    },

    onSuccess: () => {
      queryClient.setQueryData(
        ongoingTrainingsQueryKeys.active,
        (oldData: OngoingTrainingDto) => ({
          ...oldData,
          finishedOnUtc: new Date().toISOString(),
        }),
      );
    },
  });

  return finishOngoingTrainingMutation;
};

export default useFinishOngoingTrainingMutation;

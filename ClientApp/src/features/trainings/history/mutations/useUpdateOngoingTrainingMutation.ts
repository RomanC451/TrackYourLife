import { useCustomMutation } from "@/hooks/useCustomMutation";
import {
  OngoingTrainingsApi,
  type UpdateOngoingTrainingRequest,
} from "@/services/openapi";

import { allTrainingsOverviewKeys } from "../../overview/queries/trainingsOverviewKeys";
import { workoutHistoryQueryKeys } from "../queries/useWorkoutHistoryQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

export type UpdateOngoingTrainingVariables = {
  ongoingTrainingId: string;
  request: UpdateOngoingTrainingRequest;
};

const useUpdateOngoingTrainingMutation = () => {
  return useCustomMutation({
    mutationFn: ({ ongoingTrainingId, request }: UpdateOngoingTrainingVariables) =>
      ongoingTrainingsApi.updateOngoingTraining(ongoingTrainingId, request),
    meta: {
      onSuccessToast: {
        message: "Workout updated",
        type: "success",
      },
      invalidateQueries: [workoutHistoryQueryKeys.all, ...allTrainingsOverviewKeys],
    },
  });
};

export default useUpdateOngoingTrainingMutation;

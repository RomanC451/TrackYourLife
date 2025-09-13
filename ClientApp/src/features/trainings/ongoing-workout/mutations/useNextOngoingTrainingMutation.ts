import { useCustomMutation } from "@/hooks/useCustomMutation";
import {
  OngoingTrainingDto,
  OngoingTrainingsApi,
} from "@/services/openapi/api";

import {
  ongoingTrainingsQueryKeys,
  setActiveOngoingTrainingQueryDataByAction,
} from "../queries/ongoingTrainingsQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

const useNextOngoingTrainingMutation = () => {
  const nextOngoingTrainingMutation = useCustomMutation({
    mutationFn: ({
      ongoingTraining,
    }: {
      ongoingTraining: OngoingTrainingDto;
    }) =>
      ongoingTrainingsApi.nextOngoingTraining({
        ongoingTrainingId: ongoingTraining.id,
      }),

    meta: {
      invalidateQueries: [ongoingTrainingsQueryKeys.active],
    },

    onSuccess: () => {
      setActiveOngoingTrainingQueryDataByAction({
        action: "next",
      });
    },
  });

  return nextOngoingTrainingMutation;
};

export default useNextOngoingTrainingMutation;

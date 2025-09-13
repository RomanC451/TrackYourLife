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

type Variables = {
  ongoingTraining: OngoingTrainingDto;
};

const usePreviousOngoingTrainingMutation = () => {
  const previousOngoingTrainingMutation = useCustomMutation({
    mutationFn: ({ ongoingTraining }: Variables) =>
      ongoingTrainingsApi.previousOngoingTraining({
        ongoingTrainingId: ongoingTraining.id,
      }),
    meta: {
      invalidateQueries: [ongoingTrainingsQueryKeys.active],
    },

    onSuccess: () => {
      setActiveOngoingTrainingQueryDataByAction({
        action: "previous",
      });
    },
  });

  return previousOngoingTrainingMutation;
};

export default usePreviousOngoingTrainingMutation;

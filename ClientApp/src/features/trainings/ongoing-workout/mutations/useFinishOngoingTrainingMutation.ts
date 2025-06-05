import { useMutation } from "@tanstack/react-query";

import { OngoingTrainingDto, OngoingTrainingsApi } from "@/services/openapi";
import useDelayedLoading from "@/hooks/useDelayedLoading";
import {  setActiveOngoingTrainingQueryData } from "../queries/useActiveOngoingTrainingQuery";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

const ongoingTrainingsApi = new OngoingTrainingsApi();

const useFinishOngoingTrainingMutation = () => {
  const finishOngoingTrainingMutation = useMutation({
    mutationFn: ({ ongoingTraining }: { ongoingTraining: OngoingTrainingDto }) =>
      ongoingTrainingsApi.finishOngoingTraining({
        id: ongoingTraining.id,
      }),

    onSuccess: () => {
      setActiveOngoingTrainingQueryData(
        {
          setter: (oldData) => {
            return {
              ...oldData,
              finishedOnUtc: new Date().toISOString(),
            };
          },
        }
      );
    },
    onError: (error) => {
      toastDefaultServerError(error);
    },
});

  const isPending = useDelayedLoading(finishOngoingTrainingMutation.isPending);

  return { finishOngoingTrainingMutation, isPending };
};

export default useFinishOngoingTrainingMutation;
import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import {
  OngoingTrainingDto,
  OngoingTrainingsApi,
} from "@/services/openapi/api";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

import {
  invalidateActiveOngoingTrainingQuery,
  setActiveOngoingTrainingQueryData,
  setActiveOngoingTrainingQueryDataByAction,
} from "../queries/useActiveOngoingTrainingQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

const usePreviousOngoingTrainingMutation = () => {
  const previousOngoingTrainingMutation = useMutation({
    mutationFn: ({
      ongoingTraining,
    }: {
      ongoingTraining: OngoingTrainingDto;
    }) => {
      console.log("previousOngoingTrainingMutation", ongoingTraining);
      return ongoingTrainingsApi.previousOngoingTraining({
        ongoingTrainingId: ongoingTraining.id,
      });
    },

    onMutate: ({ ongoingTraining }) => {
      const previousData = ongoingTraining;

      setActiveOngoingTrainingQueryDataByAction({
        action: "previous",
      });

      return { previousData };
    },

    onError: (error, _, context) => {
      toastDefaultServerError(error);

      if (!context) {
        return;
      }

      const { previousData } = context;
      if (!previousData) {
        return;
      }

      setActiveOngoingTrainingQueryData({
        setter: (oldData) => ({
          ...oldData,
          isLoading: true,
        }),
      });
    },
    onSettled: () => {
      invalidateActiveOngoingTrainingQuery();
    },
  });

  const isPending = useDelayedLoading(
    previousOngoingTrainingMutation.isPending,
  );

  return { previousOngoingTrainingMutation, isPending };
};

export default usePreviousOngoingTrainingMutation;

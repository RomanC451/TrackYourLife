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

const useNextOngoingTrainingMutation = () => {
  const nextOngoingTrainingMutation = useMutation({
    mutationFn: ({
      ongoingTraining,
    }: {
      ongoingTraining: OngoingTrainingDto;
    }) =>
      ongoingTrainingsApi.nextOngoingTraining({
        ongoingTrainingId: ongoingTraining.id,
      }),
    onMutate: ({ ongoingTraining }) => {
      const previousData = ongoingTraining;

      setActiveOngoingTrainingQueryDataByAction({
        action: "next",
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

  const isPending = useDelayedLoading(nextOngoingTrainingMutation.isPending);

  return { nextOngoingTrainingMutation, isPending };
};

export default useNextOngoingTrainingMutation;

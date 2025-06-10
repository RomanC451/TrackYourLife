import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { OngoingTrainingsApi } from "@/services/openapi/api";
import { invalidateActiveOngoingTrainingQuery } from "../queries/useActiveOngoingTrainingQuery";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

const ongoingTrainingsApi = new OngoingTrainingsApi();

const useCreateOngoingTrainingMutation = () => {

  const createOngoingTrainingMutation = useMutation({
    mutationFn: ({ trainingId }: { trainingId: string }) => {
      return ongoingTrainingsApi.createOngoingTraining({ trainingId });
    },
    onError: (error) => {
      toastDefaultServerError(error);
    },
    onSettled: () => {
      invalidateActiveOngoingTrainingQuery();
    },
  });

  const isPending = useDelayedLoading(createOngoingTrainingMutation.isPending);

  return { createOngoingTrainingMutation, isPending };
};

export default useCreateOngoingTrainingMutation;

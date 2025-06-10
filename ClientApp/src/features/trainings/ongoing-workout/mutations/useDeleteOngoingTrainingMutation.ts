import { useMutation } from "@tanstack/react-query";

import { OngoingTrainingsApi } from "@/services/openapi/api";
import useDelayedLoading from "@/hooks/useDelayedLoading";
import { resetActiveOngoingTrainingQuery } from "../queries/useActiveOngoingTrainingQuery";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

const ongoingTrainingsApi = new OngoingTrainingsApi();

const useDeleteOngoingTrainingMutation = () => {
  const deleteOngoingTrainingMutation = useMutation({
    mutationFn: ({ ongoingTrainingId }: { ongoingTrainingId: string }) => {
      return ongoingTrainingsApi.deleteOngoingTraining(ongoingTrainingId);
    },
    onSuccess: () => {
        resetActiveOngoingTrainingQuery();
    },
    onError: (error) => {
      toastDefaultServerError(error);
    },
  });

  const isPending = useDelayedLoading(deleteOngoingTrainingMutation.isPending);

  return { deleteOngoingTrainingMutation, isPending };
};

export default useDeleteOngoingTrainingMutation;            
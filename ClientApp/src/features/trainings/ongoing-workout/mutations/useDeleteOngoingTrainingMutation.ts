import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { OngoingTrainingsApi } from "@/services/openapi/api";

import { ongoingTrainingsQueryKeys } from "../queries/ongoingTrainingsQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

type Variables = {
  ongoingTrainingId: string;
};

const useDeleteOngoingTrainingMutation = () => {
  const deleteOngoingTrainingMutation = useCustomMutation({
    mutationFn: ({ ongoingTrainingId }: Variables) => {
      return ongoingTrainingsApi.deleteOngoingTraining(ongoingTrainingId);
    },
    meta: {
      invalidateQueries: [ongoingTrainingsQueryKeys.active],
    },
    onSuccess: () => {
      queryClient.setQueryData(ongoingTrainingsQueryKeys.active, null);
    },
  });

  return deleteOngoingTrainingMutation;
};

export default useDeleteOngoingTrainingMutation;

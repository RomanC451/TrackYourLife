import { useCustomMutation } from "@/hooks/useCustomMutation";
import { OngoingTrainingsApi } from "@/services/openapi/api";

import { ongoingTrainingsQueryKeys } from "../queries/ongoingTrainingsQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

type Variables = {
  trainingId: string;
};

const useCreateOngoingTrainingMutation = () => {
  const createOngoingTrainingMutation = useCustomMutation({
    mutationFn: ({ trainingId }: Variables) => {
      return ongoingTrainingsApi.createOngoingTraining({ trainingId });
    },
    meta: {
      invalidateQueries: [ongoingTrainingsQueryKeys.active],
      awaitInvalidationQuery: ongoingTrainingsQueryKeys.active,
    },
  });

  return createOngoingTrainingMutation;
};

export default useCreateOngoingTrainingMutation;

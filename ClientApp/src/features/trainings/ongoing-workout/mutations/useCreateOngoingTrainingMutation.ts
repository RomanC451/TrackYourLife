import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { OngoingTrainingsApi } from "@/services/openapi/api";

const ongoingTrainingsApi = new OngoingTrainingsApi();

const useCreateOngoingTrainingMutation = () => {
  const navigate = useNavigate();

  const createOngoingTrainingMutation = useMutation({
    mutationFn: ({ trainingId }: { trainingId: string }) => {
      return ongoingTrainingsApi.createOngoingTraining({ trainingId });
    },

    onSuccess: () => {
      navigate({ to: "/trainings/ongoing-workout" });
    },
  });

  const isPending = useDelayedLoading(createOngoingTrainingMutation.isPending);

  return { createOngoingTrainingMutation, isPending };
};

export default useCreateOngoingTrainingMutation;

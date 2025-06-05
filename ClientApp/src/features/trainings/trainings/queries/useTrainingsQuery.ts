import { TrainingDto, TrainingsApi } from "@/services/openapi";
import { useQuery } from "@tanstack/react-query";
import { QUERY_KEYS } from "../../common/data/queryKeys";
import { queryClient } from "@/queryClient";
import useDelayedLoading from "@/hooks/useDelayedLoading";

const trainingsApi = new TrainingsApi();

const useTrainingsQuery = () => {
  const trainingsQuery = useQuery({
    queryKey: [QUERY_KEYS.trainings],
    queryFn: () => trainingsApi.getTrainings().then((res) => res.data),
  });

  const isPending = useDelayedLoading(trainingsQuery.isPending);

  return { trainingsQuery, isPending };
};

export const invalidateTrainingsQuery = () => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.trainings],
  });
};

export function getTrainingsQueryData() {
  return queryClient.getQueryData<TrainingDto[]>([QUERY_KEYS.trainings]);
}

export function setTrainingsQueryData({
    setter
}: {
  setter: (oldData: TrainingDto[]) => TrainingDto[];
}) {

    queryClient.setQueryData([QUERY_KEYS.trainings], (oldData: TrainingDto[]) => {
        return setter(oldData);
    });
}

export default useTrainingsQuery;

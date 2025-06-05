import { ExerciseDto, ExercisesApi } from "@/services/openapi";
import { QUERY_KEYS } from "../../common/data/queryKeys";
import { useQuery } from "@tanstack/react-query";
import { queryClient } from "@/queryClient";
import useDelayedLoading from "@/hooks/useDelayedLoading";
import { setManuallySet } from "@/lib/dtoUtils";


const exercisesApi = new ExercisesApi();


const useExercisesQuery = () => {
  const exercisesQuery = useQuery({
    queryKey: [QUERY_KEYS.exercises],
    queryFn: () => exercisesApi.getExercises().then((res) => setManuallySet(res.data)),
  });

  const isPending = useDelayedLoading(exercisesQuery.isPending);

  return { exercisesQuery, isPending };
};

export const invalidateExercisesQuery = () => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.exercises],
  });
};

export function getExercisesQueryData() {
  return queryClient.getQueryData<ExerciseDto[]>([QUERY_KEYS.exercises]);
}

export function setExercisesQueryData({
  setter
}: {
  setter: (oldData: ExerciseDto[]) => ExerciseDto[];
}) {
  queryClient.setQueryData([QUERY_KEYS.exercises], (oldData: ExerciseDto[]) => {
    return setter(oldData);
  });
}

export default useExercisesQuery;

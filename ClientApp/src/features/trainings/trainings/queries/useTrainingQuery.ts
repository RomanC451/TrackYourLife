// import { QUERY_KEYS } from "@/features/trainings/common/data/queryKeys"
// import { TrainingsApi } from "@/services/openapi";
// import { useQuery } from "@tanstack/react-query"


// const trainingsApi = new TrainingsApi();

// const useTrainingQuery = (trainingId: string) => {
//     const trainingQuery = useQuery({
//         queryKey: [QUERY_KEYS.trainings, trainingId],
//         queryFn: () => trainingsApi.getTrainingById(trainingId).then((res) => res.data),
//     });

//     const isPending = useDelayedLoading(trainingQuery.isPending);

//     return trainingQuery;
// }

// export default useTrainingQuery;

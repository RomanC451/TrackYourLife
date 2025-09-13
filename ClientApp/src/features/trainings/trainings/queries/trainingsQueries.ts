import { TrainingDto, TrainingsApi } from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const trainingsQueryKeys = {
  all: ["trainings"] as const,
  byId: (id: string) => [...trainingsQueryKeys.all, id] as const,
};

export const trainingsQueryOptions = {
  all: {
    queryKey: trainingsQueryKeys.all,
    queryFn: () => trainingsApi.getTrainings().then((res) => res.data),
  } as const,
  byId: (id: string) =>
    ({
      queryKey: trainingsQueryKeys.byId(id),
      //!! TODO: Implement single training query
      queryFn: () => trainingsApi.getTrainings().then((res) => res.data),
      select: (data: TrainingDto[]) =>
        data.find((training) => training.id === id)!,
    }) as const,
};

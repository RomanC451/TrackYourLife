import { queryClient } from "@/queryClient";
import { TrainingDto, TrainingsApi } from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const trainingsQueryKeys = {
  all: ["trainings"] as const,
  byId: (id: string) => [...trainingsQueryKeys.all, id] as const,
};

export class TrainingNotFoundError extends Error {
  constructor(public readonly trainingId: string) {
    super(`Training not found: ${trainingId}`);
    this.name = "TrainingNotFoundError";
  }
}

export function selectTrainingFromList(
  trainings: TrainingDto[] | undefined,
  trainingId: string,
): TrainingDto | undefined {
  return trainings?.find((training) => training.id === trainingId);
}

export function findTrainingInListCache(
  trainingId: string,
): TrainingDto | undefined {
  const trainings = queryClient.getQueryData<TrainingDto[]>(
    trainingsQueryKeys.all,
  );
  return selectTrainingFromList(trainings, trainingId);
}

async function fetchTrainingsList(): Promise<TrainingDto[]> {
  const trainings = await trainingsApi.getTrainings().then((res) => res.data);
  queryClient.setQueryData(trainingsQueryKeys.all, trainings);
  return trainings;
}

/** Resolves a training by id from the list cache, or by fetching the list once. */
export async function fetchTrainingById(
  trainingId: string,
): Promise<TrainingDto> {
  const cached = findTrainingInListCache(trainingId);
  if (cached) {
    return cached;
  }

  const trainings = await fetchTrainingsList();
  const training = selectTrainingFromList(trainings, trainingId);
  if (!training) {
    throw new TrainingNotFoundError(trainingId);
  }

  return training;
}

export const trainingsQueryOptions = {
  all: {
    queryKey: trainingsQueryKeys.all,
    queryFn: () => trainingsApi.getTrainings().then((res) => res.data),
  } as const,
  byId: (id: string) =>
    ({
      queryKey: trainingsQueryKeys.byId(id),
      queryFn: () => fetchTrainingById(id),
    }) as const,
};

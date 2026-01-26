import { queryOptions } from "@tanstack/react-query";

import { TrainingsApi } from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const trainingTemplatesUsageQueryKeys = {
  all: ["trainingTemplatesUsage"] as const,
};

export const trainingTemplatesUsageQueryOptions = {
  all: queryOptions({
    queryKey: trainingTemplatesUsageQueryKeys.all,
    queryFn: () =>
      trainingsApi.getTrainingTemplatesUsage().then((res) => res.data),
  }),
};

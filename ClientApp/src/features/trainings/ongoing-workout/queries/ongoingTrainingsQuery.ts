import { queryOptions } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";

import { queryClient } from "@/queryClient";
import {
  OngoingTrainingDto,
  OngoingTrainingsApi,
} from "@/services/openapi/api";
import { retryQueryExcept } from "@/services/openapi/retry";

import {
  applyOngoingWorkoutNavigation,
  type OngoingWorkoutNavigationAction,
} from "../ongoingWorkoutNavigation";

const ongoingTrainingsApi = new OngoingTrainingsApi();

export const ongoingTrainingsQueryKeys = {
  all: ["ongoingTrainings"] as const,
  active: ["ongoingTrainings", "active"] as const,
  byId: (id: string) => ["ongoingTrainings", id] as const,
};

export const ongoingTrainingsQueryOptions = {
  active: queryOptions({
    queryKey: ongoingTrainingsQueryKeys.active,
    queryFn: () =>
      ongoingTrainingsApi.getActiveOngoingTraining().then((res) => res.data),
    retry: (failureCount, error) =>
      retryQueryExcept(failureCount, error, {
        max_retries: 3,
        checkedCodes: {
          [StatusCodes.NOT_FOUND]: () => {
            queryClient.setQueryData(ongoingTrainingsQueryKeys.active, null);
            return null;
          },
        },
      }),
  }),
  byId: (id: string) =>
    queryOptions({
      queryKey: ongoingTrainingsQueryKeys.byId(id),
      queryFn: () =>
        ongoingTrainingsApi.getOngoingTrainingById(id).then((res) => res.data),
    }),
};

export function setActiveOngoingTrainingQueryDataByAction({
  action,
}: {
  action: OngoingWorkoutNavigationAction;
}) {
  queryClient.setQueryData(
    ongoingTrainingsQueryKeys.active,
    (oldData: OngoingTrainingDto) => {
      const updatedData = applyOngoingWorkoutNavigation(oldData, action);
      return {
        ...updatedData,
        isLoading: true,
      };
    },
  );
}

/** Route loaders / preload: active ongoing session. */
export async function ensureActiveOngoingTraining() {
  await queryClient.ensureQueryData(ongoingTrainingsQueryOptions.active);
}

import { queryOptions } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";

import { queryClient } from "@/queryClient";
import {
  OngoingTrainingDto,
  OngoingTrainingsApi,
} from "@/services/openapi/api";
import { retryQueryExcept } from "@/services/openapi/retry";

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
  action: "next" | "previous";
}) {
  queryClient.setQueryData(
    ongoingTrainingsQueryKeys.active,
    (oldData: OngoingTrainingDto) => ({
      ...(action === "next" ? next(oldData) : previous(oldData)),
      isLoading: true,
    }),
  );
}

function next(ongoingTraining: OngoingTrainingDto) {
  if (ongoingTraining.finishedOnUtc) {
    return ongoingTraining;
  }

  if (!ongoingTraining.hasNext) {
    return ongoingTraining;
  }

  if (ongoingTraining.isLastSet) {
    return {
      ...ongoingTraining,
      exerciseIndex: ongoingTraining.exerciseIndex + 1,
      setIndex: 0,
    };
  }

  return {
    ...ongoingTraining,
    setIndex: ongoingTraining.setIndex + 1,
  };
}

function previous(ongoingTraining: OngoingTrainingDto) {
  if (ongoingTraining.finishedOnUtc) {
    return ongoingTraining;
  }

  if (!ongoingTraining.hasPrevious) {
    return ongoingTraining;
  }

  if (ongoingTraining.isFirstSet) {
    return {
      ...ongoingTraining,
      exerciseIndex: ongoingTraining.exerciseIndex - 1,
      setIndex:
        ongoingTraining.training.exercises[ongoingTraining.exerciseIndex - 1]
          .exerciseSets.length - 1,
    };
  }

  return {
    ...ongoingTraining,
    setIndex: ongoingTraining.setIndex - 1,
  };
}

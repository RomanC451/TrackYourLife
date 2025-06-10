import { useQuery } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { setManuallySet } from "@/lib/dtoUtils";
import { queryClient } from "@/queryClient";
import {
  OngoingTrainingDto,
  OngoingTrainingsApi,
} from "@/services/openapi/api";
import { ApiError } from "@/services/openapi/apiSettings";
import { retryQueryExcept } from "@/services/openapi/retry";

import { QUERY_KEYS } from "../../common/data/queryKeys";

const ongoingTrainingsApi = new OngoingTrainingsApi();

const useActiveOngoingTrainingQuery = () => {
  const activeOngoingTrainingQuery = useQuery({
    queryKey: [QUERY_KEYS.activeOngoingTraining],
    queryFn: () =>
      ongoingTrainingsApi
        .getActiveOngoingTraining()
        .then((res) => setManuallySet(res.data)),
    retry: (failureCount, error: ApiError) =>
      retryQueryExcept(failureCount, error, {
        max_retries: 3,
        checkedCodes: {
          [StatusCodes.BAD_REQUEST]: () => {
            console.log("BAD_REQUEST");
          },
        },
      }),
  });

  const isPending = useDelayedLoading(activeOngoingTrainingQuery.isPending);

  return { activeOngoingTrainingQuery, isPending };
};

export const prefetchActiveOngoingTrainingQuery = () => {
  queryClient.prefetchQuery({
    queryKey: [QUERY_KEYS.activeOngoingTraining],
    queryFn: () =>
      ongoingTrainingsApi
        .getActiveOngoingTraining()
        .then((res) => setManuallySet(res.data)),
  });
};

export const invalidateActiveOngoingTrainingQuery = () => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.activeOngoingTraining],
  });
};

export const resetActiveOngoingTrainingQuery = () => {
  queryClient.resetQueries({
    queryKey: [QUERY_KEYS.activeOngoingTraining],
  });
};

export function getActiveOngoingTrainingQueryData() {
  return queryClient.getQueryData<OngoingTrainingDto>([
    QUERY_KEYS.activeOngoingTraining,
  ]);
}

export function setActiveOngoingTrainingQueryData({
  setter,
}: {
  setter: (oldData: OngoingTrainingDto) => OngoingTrainingDto;
}) {
  queryClient.setQueryData(
    [QUERY_KEYS.activeOngoingTraining],
    (oldData: OngoingTrainingDto) => {
      return setter(oldData);
    },
  );
}

export function setActiveOngoingTrainingQueryDataByAction({
  action,
}: {
  action: "next" | "previous";
}) {
  queryClient.setQueryData(
    [QUERY_KEYS.activeOngoingTraining],
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

export default useActiveOngoingTrainingQuery;

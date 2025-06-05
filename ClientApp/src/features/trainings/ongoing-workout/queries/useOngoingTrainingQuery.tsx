import { useQuery } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { setManuallySet } from "@/lib/dtoUtils";
import { OngoingTrainingsApi } from "@/services/openapi/api";
import { ApiError } from "@/services/openapi/apiSettings";
import { retryQueryExcept } from "@/services/openapi/retry";

import { QUERY_KEYS } from "../../common/data/queryKeys";

const ongoingTrainingsApi = new OngoingTrainingsApi();

const useOngoingTrainingQuery = (id: string) => {
  const ongoingTrainingQuery = useQuery({
    queryKey: [QUERY_KEYS.activeOngoingTraining],
    queryFn: () =>
      ongoingTrainingsApi
        .getOngoingTrainingById(id)
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

  const isPending = useDelayedLoading(ongoingTrainingQuery.isPending);

  return { ongoingTrainingQuery, isPending };
};

export default useOngoingTrainingQuery;

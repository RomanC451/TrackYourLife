import { useState } from "react";
import { UseQueryResult } from "@tanstack/react-query";

import { ApiError } from "@/services/openapi/apiSettings";

import { Button } from "./ui/button";

type HandleQueryProps<T> = {
  query: UseQueryResult<T, ApiError>;
  success: (data: T) => React.JSX.Element;
  error?: (error: ApiError) => React.JSX.Element;
  pending?: () => React.JSX.Element;
  enableToggleLoading?: boolean;
} & (
  | {
      isDelayedLoading: boolean;
      empty?: () => React.JSX.Element;
    }
  | {
      isDelayedLoading?: undefined;
      empty?: undefined;
    }
);

function HandleQuery<T>({
  query,
  success,
  error,
  pending,
  empty,
  isDelayedLoading,
  enableToggleLoading = false,
}: HandleQueryProps<T>): React.JSX.Element {
  const [isLoading, setIsLoading] = useState(false);

  const toggleLoading = () => {
    setIsLoading((prev) => !prev);
  };

  if (enableToggleLoading) {
    query.isPending = isLoading;
  }

  function renderQuery() {
    if (query.isPending) {
      if (isDelayedLoading !== undefined && isDelayedLoading === false) {
        return empty ? empty() : <div>Empty...</div>;
      }

      return pending ? pending() : <div>Loading...</div>;
    }

    if (query.isError) {
      return error ? (
        error(query.error)
      ) : (
        <div>Error: {query.error.message}</div>
      );
    }

    return success(query.data);
  }

  return (
    <>
      {enableToggleLoading && (
        <Button onClick={toggleLoading}>Toggle loading</Button>
      )}
      {renderQuery()}
    </>
  );
}

export default HandleQuery;

import { useState } from "react";
import { UseQueryResult } from "@tanstack/react-query";

import { ApiError } from "@/services/openapi/apiSettings";

import { Button } from "./ui/button";

type QueryMap = Record<string, UseQueryResult<unknown, ApiError>>;

type HandleMultipleQueriesProps = {
  queries: QueryMap;
  success: (data: Record<string, unknown>) => React.JSX.Element;
  error?: (errors: Record<string, ApiError>) => React.JSX.Element;
  pending?: () => React.JSX.Element;
  enableToggleLoading?: boolean;
} & (
    | {
      isDelayedLoading: boolean;
      empty?: () => React.JSX.Element;
    }
    | {
      isDelayedLoading?: never;
      empty?: never;
    }
  );

function HandleMultipleQueries({
  queries,
  success,
  error,
  pending,
  enableToggleLoading = false,
}: HandleMultipleQueriesProps): React.JSX.Element {
  const [isLoading, setIsLoading] = useState(false);

  const toggleLoading = () => {
    setIsLoading((prev) => !prev);
  };

  if (enableToggleLoading) {
    Object.values(queries).forEach((query) => {
      query.isPending = isLoading;
    });
  }

  function getErrors(): Record<string, ApiError> | null {
    const errors: Record<string, ApiError> = {};
    let hasError = false;

    Object.entries(queries).forEach(([key, query]) => {
      if (query.isError) {
        errors[key] = query.error;
        hasError = true;
      }
    });

    return hasError ? errors : null;
  }

  function getPendingQueries(): string[] {
    return Object.entries(queries)
      .filter(([, query]) => query.isPending)
      .map(([key]) => key);
  }

  function getAllData(): Record<string, unknown> {
    const data: Record<string, unknown> = {};
    Object.entries(queries).forEach(([key, query]) => {
      data[key] = query.data;
    });
    return data;
  }

  function renderError(errors: Record<string, ApiError>) {
    return error ? (
      error(errors)
    ) : (
      <div>
        Error:{" "}
        {Object.entries(errors)
          .map(([key, err]) => `${key}: ${err.message}`)
          .join(", ")}
      </div>
    );
  }



  function renderQueries() {
    const errors = getErrors();
    if (errors) {
      return renderError(errors);
    }

    const pendingQueries = getPendingQueries();




    if (pendingQueries.length > 0) {
      return pending ? (
        pending()
      ) : (
        <div>Loading ...</div>
      );
    }

    return success(getAllData());
  }

  return (
    <>
      {enableToggleLoading && (
        <Button onClick={toggleLoading}>Toggle loading</Button>
      )}
      {renderQueries()}
    </>
  );
}

export default HandleMultipleQueries;

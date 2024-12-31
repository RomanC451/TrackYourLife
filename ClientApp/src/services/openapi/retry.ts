import { StatusCodes } from "http-status-codes";

import { ApiError } from "@/services/openapi/apiSettings";

const MAX_RETRIES = 3 as const;

type RetryExceptConfig = {
  max_retries?: number;
  checkedCodes: Record<number, (() => void) | null>;
};

export function retryQueryExcept(
  failureCount: number,
  error: ApiError,
  config?: RetryExceptConfig,
) {
  const max_retries = config?.max_retries ?? MAX_RETRIES;

  if (failureCount >= max_retries) return false;

  const status = error.status ?? error.response?.data?.status;

  if (status && config?.checkedCodes[status] !== undefined) {
    config.checkedCodes[status]?.();
    return false;
  }

  return true;
}
type RetryExcept404Config = {
  max_retries?: number;
  notFoundCallback?: () => void;
};

export function retryQueryExcept404(
  failureCount: number,
  error: ApiError,
  config?: RetryExcept404Config,
) {
  return retryQueryExcept(failureCount, error, {
    max_retries: config?.max_retries,
    checkedCodes: {
      [StatusCodes.NOT_FOUND]: () => {
        config?.notFoundCallback?.();
      },
    },
  });
}

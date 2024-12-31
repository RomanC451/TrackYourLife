import {
  ApiError,
  ApiErrorData,
  toastDefaultServerError,
} from "~/data/apiSettings";

type ApiErrorHandler = (errorData: ApiErrorData) => void;

type StatusCodeHandlerType = {
  default?: ApiErrorHandler;
  [errorType: string]: ApiErrorHandler | undefined;
};

type ApiErrorHandlers = {
  [statusCode: number]: StatusCodeHandlerType;
};

export function handleApiError({
  error,
  errorHandlers,
  defaultHandler,
}: {
  error: ApiError;
  errorHandlers: ApiErrorHandlers;
  defaultHandler?: () => void;
}) {
  const statusCode = error?.response?.status;

  const errorData = error?.response?.data;
  const errorType = errorData?.type;

  if (!errorData || !statusCode) {
    toastDefaultServerError(error);
    defaultHandler?.();
    return;
  }

  if (errorHandlers[statusCode]) {
    if (!errorType) {
      errorHandlers[statusCode].default?.(errorData);
      return;
    }

    if (errorHandlers[statusCode][errorType]) {
      errorHandlers[statusCode][errorType](errorData);
      return;
    }

    errorHandlers[statusCode].default?.(errorData);
    return;
  }

  toastDefaultServerError(error);
  defaultHandler?.();
}

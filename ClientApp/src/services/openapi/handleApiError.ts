import { ErrorOption } from "react-hook-form";

import { ApiError, ApiErrorData } from "@/services/openapi/apiSettings";

type ApiErrorHandler = (errorData: ApiErrorData) => void;

type StatusCodeHandlerType = {
  default?: ApiErrorHandler;
  [errorType: string]: ApiErrorHandler | undefined;
};

type ApiErrorHandlers = {
  [statusCode: number]: StatusCodeHandlerType;
};

type ValidationErrorsHandler<TSchema> = (
  name: keyof TSchema,
  error: ErrorOption,
  options?: {
    shouldFocus: boolean;
  },
) => void;

export function handleApiError<TSchema>({
  error,
  errorHandlers,
  defaultHandler,
  validationErrorsHandler,
  focusOnError = false,
}: {
  error: ApiError;
  errorHandlers?: ApiErrorHandlers;
  validationErrorsHandler?: ValidationErrorsHandler<TSchema>;
  defaultHandler?: () => void;
  focusOnError?: boolean;
}) {
  const statusCode = error?.response?.status;

  const errorData = error?.response?.data;
  const errorType = errorData?.type;

  if (!errorData || !statusCode) {
    defaultHandler?.();
    return;
  }

  if (errorType == "ValidationError") {
    const validationErrors = errorData?.errors;

    if (
      !validationErrors ||
      validationErrors.length === 0 ||
      !validationErrorsHandler
    ) {
      defaultHandler?.();
      return;
    }

    for (const error of validationErrors) {
      if (validationErrorsHandler) {
        validationErrorsHandler(
          error.name.toLowerCase() as keyof TSchema,
          { type: "custom", message: error.message },
          { shouldFocus: focusOnError },
        );
        return;
      }
    }
    return;
  }

  if (!errorHandlers) {
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

  defaultHandler?.();
}

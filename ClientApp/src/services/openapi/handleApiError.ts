import {
  ApiError,
  ApiErrorData,
  toastDefaultServerError,
} from "@/services/openapi/apiSettings";
import { ErrorOption } from "react-hook-form";

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
    toastDefaultServerError(error);
    defaultHandler?.();
    return;
  }

  if(errorType == "ValidationError"){
    const validationErrors = errorData?.errors;

    if (!validationErrors || validationErrors.length === 0 || !validationErrorsHandler){
      toastDefaultServerError(error);
      defaultHandler?.();
      return;
    }

    for (const error of validationErrors){
      if (validationErrorsHandler) {
        validationErrorsHandler(error.name.toLowerCase() as keyof TSchema, {type: "custom", message: error.message}, {shouldFocus: focusOnError});
        return;
      }
    }
    return;
  }

  if (!errorHandlers) {
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

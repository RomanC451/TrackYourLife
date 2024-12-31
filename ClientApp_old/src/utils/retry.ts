import { StatusCodes } from "http-status-codes";
import { ApiError } from "~/data/apiSettings";

type RetryExcept404Config = {
  max_retries?: number;
  customCheck?: (error: Error) => boolean;
  notFoundCallback?: () => void;
};

const MAX_RETRIES = 3 as const;

function retryExcept404(
  failureCount: number,
  error: ApiError,
  config?: RetryExcept404Config,
) {
  const max_retries = config?.max_retries ?? MAX_RETRIES;

  if (failureCount >= max_retries) return false;

  if (error.response?.data?.status === StatusCodes.NOT_FOUND) {
    config?.notFoundCallback?.();
    return false;
  }

  if (config?.customCheck) 
    return config.customCheck(error);

  return true
}



type RetryExceptConfig = {
  max_retries?: number;
  checkedCodes: Record<number, () => void>,
}


function retryExcept(failureCount: number, error: ApiError, config?: RetryExceptConfig){
  const max_retries = config?.max_retries ?? MAX_RETRIES;

  if (failureCount >= max_retries) return false;

  const status = error.response?.data?.status;

  if (status && config?.checkedCodes[status]) {
    config.checkedCodes[status]();
    return false;
  }

  return true

}

retryExcept(1, {response: {data: {status: 404}}} as ApiError, {checkedCodes: 
  {[StatusCodes.NOT_FOUND]: () => {}
}})

export default retryExcept404;

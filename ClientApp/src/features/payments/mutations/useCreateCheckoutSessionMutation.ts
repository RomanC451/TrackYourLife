import { useCustomMutation } from "@/hooks/useCustomMutation";
import { CreateCheckoutSessionRequest, PaymentsApi } from "@/services/openapi";
import {
  ApiErrorHandlers,
  handleApiError,
} from "@/services/openapi/handleApiError";

const paymentsApi = new PaymentsApi();

export function useCreateCheckoutSessionMutation({
  errorHandlers,
}: { errorHandlers?: ApiErrorHandlers } = {}) {
  return useCustomMutation({
    mutationFn: (request: CreateCheckoutSessionRequest) =>
      paymentsApi.createCheckoutSession(request).then((res) => res.data),
    onError: (error) => {
      handleApiError({
        error,
        errorHandlers: errorHandlers,
      });
    },
  });
}

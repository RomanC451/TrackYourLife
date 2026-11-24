import { useCustomMutation } from "@/hooks/useCustomMutation";
import {
  PersonalAccessTokensApi,
  type CreatePersonalAccessTokenRequest,
  type CreatedPersonalAccessTokenDto,
} from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

import { personalAccessTokensQueryKeys } from "../queries/personalAccessTokensQuery";

const personalAccessTokensApi = new PersonalAccessTokensApi();

export function useCreatePersonalAccessTokenMutation() {
  return useCustomMutation<
    CreatedPersonalAccessTokenDto,
    CreatePersonalAccessTokenRequest,
    unknown
  >({
    mutationFn: (body) =>
      personalAccessTokensApi
        .createPersonalAccessToken(body)
        .then((res) => res.data),
    meta: {
      invalidateQueries: [personalAccessTokensQueryKeys.all],
    },
    onError: (error: ApiError) => {
      handleApiError({ error });
    },
  });
}

export function useRevokePersonalAccessTokenMutation() {
  return useCustomMutation<void, string, unknown>({
    mutationFn: async (id) => {
      await personalAccessTokensApi.revokePersonalAccessToken(id);
    },
    meta: {
      invalidateQueries: [personalAccessTokensQueryKeys.all],
      onSuccessToast: { message: "Token revoked", type: "success" },
    },
    onError: (error: ApiError) => {
      handleApiError({ error });
    },
  });
}

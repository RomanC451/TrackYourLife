import { queryOptions } from "@tanstack/react-query";

import { PersonalAccessTokensApi } from "@/services/openapi";

const personalAccessTokensApi = new PersonalAccessTokensApi();

export const personalAccessTokensQueryKeys = {
  all: ["personalAccessTokens"] as const,
};

export const personalAccessTokensQueryOptions = {
  list: () =>
    queryOptions({
      queryKey: personalAccessTokensQueryKeys.all,
      queryFn: () =>
        personalAccessTokensApi.getPersonalAccessTokens().then((res) => res.data),
    }),
};

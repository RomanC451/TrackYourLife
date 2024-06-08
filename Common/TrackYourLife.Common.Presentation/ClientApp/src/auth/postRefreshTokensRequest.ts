import { userEndpoints } from "~/data/apiSettings";
import { TFetchRequest, requestTypes } from "~/hooks/useApiRequests";

export type TPostRefreshTokensResponse = string;

type PostRefreshTokensRequestProps = {
  fetchRequest: TFetchRequest<undefined, TPostRefreshTokensResponse>;
};

export const postRefreshTokensRequest = ({
  fetchRequest,
}: PostRefreshTokensRequestProps) => {
  return fetchRequest({
    endpoint: `${userEndpoints.refreshToken}`,
    requestType: requestTypes.POST,
    authorized: false,
  });
};

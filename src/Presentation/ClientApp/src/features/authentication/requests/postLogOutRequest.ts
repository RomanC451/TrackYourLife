import { userEndpoints } from "~/data/apiSettings";
import { TFetchRequest, requestTypes } from "~/hooks/useApiRequests";

type PostLogOutRequestProps = {
  fetchRequest: TFetchRequest<undefined, undefined>;
};

export const postLogOutRequest = ({ fetchRequest }: PostLogOutRequestProps) => {
  return fetchRequest({
    endpoint: `${userEndpoints.logout}`,
    requestType: requestTypes.POST,
    jsonResponse: false,
    authorized: true,
  });
};

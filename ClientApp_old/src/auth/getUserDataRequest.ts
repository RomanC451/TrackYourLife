import { userEndpoints } from "~/data/apiSettings";
import { TFetchRequest, requestTypes } from "~/hooks/useApiRequests";

export type TGetUserDataResponse = {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
};

type GetUserDataRequestProps = {
  fetchRequest: TFetchRequest<undefined, TGetUserDataResponse>;
};

export const getUserDataRequest = ({
  fetchRequest,
}: GetUserDataRequestProps) => {
  return fetchRequest({
    endpoint: `${userEndpoints.default}`,
    requestType: requestTypes.GET,
    authorized: true,
  });
};

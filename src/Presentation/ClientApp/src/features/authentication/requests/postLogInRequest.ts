import { userEndpoints } from "~/data/apiSettings";
import { requestTypes, TFetchRequest } from "~/hooks/useApiRequests";

import { TLogInSchema } from "../data/schemas";

export type TPostLogInResponse = {
  jwtToken: string;
  userId: string;
};

type PostLogInRequestProps = {
  fetchRequest: TFetchRequest<TLogInSchema, TPostLogInResponse>;
  data: TLogInSchema;
};

export const postLogInRequest = ({
  fetchRequest,
  data,
}: PostLogInRequestProps) => {
  return fetchRequest({
    endpoint: userEndpoints.login,
    requestType: requestTypes.POST,
    data: data,
  });
};

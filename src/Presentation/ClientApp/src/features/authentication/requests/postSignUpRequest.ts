import { userEndpoints } from "~/data/apiSettings";
import { requestTypes, TFetchRequest } from "~/hooks/useApiRequests";

import { TSignUpSchema } from "../data/schemas";

type PostSignUpRequestProps = {
  fetchRequest: TFetchRequest<TSignUpSchema, undefined>;
  data: TSignUpSchema;
};

export const postSignUpRequest = ({
  fetchRequest,
  data,
}: PostSignUpRequestProps) => {
  return fetchRequest({
    endpoint: userEndpoints.register,
    requestType: requestTypes.POST,
    data: data,
  });
};

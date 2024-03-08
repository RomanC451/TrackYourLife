import { userEndpoints } from "~/data/apiSettings";
import { requestTypes, TFetchRequest } from "~/hooks/useApiRequests";

export type TResendVerificationEmailData = {
  email: string;
};

export type TPostResendVerificationEmailResponse = {
  userId: string;
};

export const postResendVerificationEmail = (
  fetchRequest: TFetchRequest<
    TResendVerificationEmailData,
    TPostResendVerificationEmailResponse
  >,
  data: TResendVerificationEmailData,
) => {
  return fetchRequest({
    endpoint: userEndpoints.resendVerificationEmail,
    requestType: requestTypes.POST,
    jsonResponse: false,
    data,
  });
};

import wretch, { Wretch } from "wretch";
import { apiUrl, userEndpoints } from "~/data/apiSettings";

export function postFetch(
  apiInstance: Wretch,
  data: any,
  endpoint: string,
  storeTokenFnc: (arg0: string) => void
) {
  return apiInstance
    .url(endpoint)
    .post(data)
    .unauthorized(async (_, req) => {
      const token = await wretch(apiUrl + userEndpoints.refreshToken)
        .get()
        .text();
      storeTokenFnc(token);
      // Replay the original request with new credentials
      return req
        .auth(token)
        .get()
        .unauthorized((err) => {
          throw err;
        })
        .json();
    });
}

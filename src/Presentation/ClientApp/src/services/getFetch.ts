import { NavigateFunction } from "react-router-dom";
import wretch, { Wretch } from "wretch";
import { apiUrl, userEndpoints } from "~/data/apiSettings";

export function getFetch(
  apiInstance: Wretch,
  endpoint: string,
  navigate: NavigateFunction,
  storeTokenFnc?: (arg0: string) => void
) {
  return apiInstance
    .url(endpoint)
    .get()
    .unauthorized(async (_, req) => {
      const token = await wretch(apiUrl + userEndpoints.refreshToken)
        .options({ credentials: "include", mode: "cors" })
        .post()
        .badRequest(() => {
          navigate("/auth");
        })
        .text();

      if (token == undefined) return;

      if (storeTokenFnc) storeTokenFnc(token);
      // Replay the original request with new credentials
      return req
        .auth(token)
        .options({ credentials: "include", mode: "cors" })
        .get()
        .unauthorized((err) => {
          navigate("/auth");
          throw err;
        })
        .json();
    });
}

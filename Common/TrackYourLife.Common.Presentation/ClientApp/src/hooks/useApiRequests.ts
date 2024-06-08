import wretch from "wretch";
import AbortAddon from "wretch/addons/abort";
import { useApiContext } from "~/contexts/ApiContextProvider";
import { apiUrl, userEndpoints } from "~/data/apiSettings";
import { ObjectValues, TJsonObject } from "~/types/defaultTypes";

export const requestTypes = {
  GET: "GET",
  POST: "POST",
  PUT: "PUT",
  DELETE: "DELETE",
} as const;

export type RequestType = ObjectValues<typeof requestTypes>;

export type FetchRequestProps<TData extends TFetchRequestData> = {
  endpoint: string;
  requestType: RequestType;
  authorized?: boolean;
  jsonResponse?: boolean;
  data?: TData;
  abortControllerRef?: React.MutableRefObject<AbortController | undefined>;
  abortSignal?: AbortSignal;
};

export type TFetchRequestData = TJsonObject | undefined;
export type TFetchRequestResponse = TJsonObject | string | undefined;

export type TFetchRequest<
  TData extends TFetchRequestData,
  TResponse extends TFetchRequestResponse,
> = (props: FetchRequestProps<TData>) => Promise<Awaited<TResponse>>;

export const useApiRequests = () => {
  const { authorizedApi, defaultApi, setJwtToken } = useApiContext();
  function fetchRequest<
    TData extends TFetchRequestData,
    TResponse extends TFetchRequestResponse,
  >({
    endpoint,
    requestType,
    data,
    authorized = false,
    jsonResponse = true,
    abortControllerRef,
    abortSignal,
  }: FetchRequestProps<TData>): Promise<Awaited<TResponse>> {
    if (abortControllerRef) {
      abortControllerRef.current?.abort();
      abortControllerRef.current = new AbortController();
    }

    const apiInstance = authorized ? authorizedApi : defaultApi;

    const apiReq = apiInstance
      .url(endpoint)
      .addon(AbortAddon())
      .options({
        credentials: "include",
        mode: "cors",
        signal: abortControllerRef?.current?.signal ?? abortSignal,
      });

    let ranApiReq;

    switch (requestType) {
      case requestTypes.GET:
        ranApiReq = apiReq.get();
        break;
      case requestTypes.POST:
        ranApiReq = apiReq.post(data);
        break;
      case requestTypes.PUT:
        ranApiReq = apiReq.put(data);
        break;
      case requestTypes.DELETE:
        ranApiReq = apiReq.delete();
        break;
      default:
        throw new Error("Request type not implemented");
    }

    ranApiReq.unauthorized(async (error, req) => {
      if (!authorized) return error;

      const token = await wretch(apiUrl + userEndpoints.refreshToken)
        .options({ credentials: "include", mode: "cors" })
        .post()
        .text();

      if (token == undefined) {
        return error;
      }

      setJwtToken(token);
      // Replay the original request with new credentials

      const newReq = req.auth(`Bearer ${token}`);

      let ranNewReq;

      switch (requestType) {
        case requestTypes.GET:
          ranNewReq = newReq.get();
          break;
        case requestTypes.POST:
          ranNewReq = newReq.post(data);
          break;
        case requestTypes.PUT:
          ranNewReq = newReq.put(data);
          break;
        case requestTypes.DELETE:
          ranNewReq = newReq.delete();
          break;
        default:
          throw new Error("Request type not implemented");
      }

      if (jsonResponse) return ranNewReq.json<TResponse>();
      else return ranNewReq.res();
    });

    if (jsonResponse) return ranApiReq.json<TResponse>();
    else return ranApiReq.res();
  }

  return { fetchRequest };
};

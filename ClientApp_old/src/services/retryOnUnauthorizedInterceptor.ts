import globalAxios, { AxiosError } from "axios";
import { router } from "~/App";
import { AuthApi } from "./openapi";

const refreshAxios = globalAxios.create();
refreshAxios.defaults.withCredentials = true;

globalAxios.interceptors.response.use(undefined, async (error: AxiosError) => {
  if (error.config && error.response && error.response.status === 401) {
    try {
      const refreshTokenResponse = await new AuthApi(
        undefined,
        undefined,
        refreshAxios,
      ).refreshToken();

      globalAxios.defaults.headers.common["Authorization"] =
        `Bearer ${refreshTokenResponse.data.tokenValue}`;

      // TODO REMOVE: just for debug purpose
      localStorage.setItem("jwtToken", refreshTokenResponse.data.tokenValue);

      const newRequest = {
        ...error.config,
        headers: {
          ...error.config.headers,
          Authorization: `Bearer ${refreshTokenResponse.data.tokenValue}`,
        },
      };

      return globalAxios(newRequest);
    } catch (refreshError) {
      router.navigate({ to: "/auth" });
      return Promise.reject(error);
    }
  }

  // If error was not 401 or refreshing failed, reject the original request
  return Promise.reject(error);
});

globalAxios.defaults.withCredentials = true;

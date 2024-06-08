import globalAxios, { AxiosError } from "axios";
import { AuthenticationApi } from "./openapi";

globalAxios.interceptors.response.use(undefined, async (error: AxiosError) => {
  if (error.config && error.response && error.response.status === 401) {
    try {
      const refreshTokenResponse = await new AuthenticationApi().refreshToken();

      globalAxios.defaults.headers.common["Authorization"] =
        `Bearer ${refreshTokenResponse.data.tokenValue}`;

      //!! TO BE REMOVED: just for debug purpose
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
      console.error("Refresh failed:", refreshError);
      throw refreshError;
    }
  }

  // If error was not 401 or refreshing failed, reject the original request
  return Promise.reject(error);
});

globalAxios.defaults.withCredentials = true;

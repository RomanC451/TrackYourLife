import globalAxios, { AxiosError } from "axios";

import { router } from "@/App";

import { AuthApi } from "../api";

const refreshAxios = globalAxios.create();
refreshAxios.defaults.withCredentials = true;

globalAxios.interceptors.response.use(undefined, async (error: AxiosError) => {
  if (error.config && error.response && error.response.status === 401) {
    const deviceId = localStorage.getItem("deviceId");

    const currentPath = window.location.pathname + window.location.search;
    try {

      if (!deviceId) {
        router.navigate({ to: "/auth", search: { redirect: currentPath } });
        return Promise.reject(error);
      }

      const refreshTokenResponse = await new AuthApi(
        undefined,
        undefined,
        refreshAxios,
      ).refreshToken({ deviceId: deviceId });

      globalAxios.defaults.headers.common["Authorization"] =
        `Bearer ${refreshTokenResponse.data.tokenValue}`;

      const newRequest = {
        ...error.config,
        headers: {
          ...error.config.headers,
          Authorization: `Bearer ${refreshTokenResponse.data.tokenValue}`,
        },
      };

      return globalAxios(newRequest);
    } catch {
      router.navigate({ to: "/auth", search: { redirect: currentPath } });
      return Promise.reject(error);
    }
  }

  // If error was not 401 or refreshing failed, reject the original request
  return Promise.reject(error);
});

globalAxios.defaults.withCredentials = true;

import globalAxios, { AxiosError } from "axios";

import { router } from "@/App";
import { authModes } from "@/features/authentication/data/enums";
import { getSafeRedirectUrl } from "@/lib/urlSanitizer";

import { AuthApi } from "../api";

const refreshAxios = globalAxios.create();
refreshAxios.defaults.withCredentials = true;

// Mutex for refresh token - prevents race conditions
let isRefreshing = false;
let refreshPromise: Promise<string> | null = null;

globalAxios.interceptors.response.use(undefined, async (error: AxiosError) => {
  if (error.config && error.response?.status === 401) {
    const deviceId = localStorage.getItem("deviceId");

    const currentPath =
      globalThis.location.pathname + globalThis.location.search;
    console.log("currentPath", currentPath);
    const safeRedirect = getSafeRedirectUrl(currentPath, "/home");
    console.log("safeRedirect", safeRedirect);

    try {
      if (!deviceId) {
        router.navigate({
          to: "/auth",
          search: { authMode: authModes.logIn, redirect: safeRedirect },
        });
        throw error;
      }

      // If a refresh is already in progress, wait for it instead of making another request
      if (isRefreshing && refreshPromise) {
        const newToken = await refreshPromise;
        const newRequest = {
          ...error.config,
          headers: {
            ...error.config.headers,
            Authorization: `Bearer ${newToken}`,
          },
        };
        return globalAxios(newRequest);
      }

      // Start a new refresh - set mutex
      isRefreshing = true;
      refreshPromise = new AuthApi(undefined, undefined, refreshAxios)
        .refreshToken({ deviceId: deviceId })
        .then((response) => {
          const newToken = response.data.tokenValue;
          globalAxios.defaults.headers.common["Authorization"] =
            `Bearer ${newToken}`;
          return newToken;
        });

      const newToken = await refreshPromise;

      // Clear mutex after successful refresh
      isRefreshing = false;
      refreshPromise = null;

      const newRequest = {
        ...error.config,
        headers: {
          ...error.config.headers,
          Authorization: `Bearer ${newToken}`,
        },
      };

      return globalAxios(newRequest);
    } catch {
      // Clear mutex on error
      isRefreshing = false;
      refreshPromise = null;

      router.navigate({
        to: "/auth",
        search: { authMode: authModes.logIn, redirect: safeRedirect },
      });
      throw error;
    }
  }

  // If error was not 401 or refreshing failed, reject the original request
  throw error;
});

globalAxios.defaults.withCredentials = true;

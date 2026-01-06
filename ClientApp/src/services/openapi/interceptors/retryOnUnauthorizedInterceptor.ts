import globalAxios, { AxiosError, InternalAxiosRequestConfig } from "axios";

import { router } from "@/App";
import { authModes } from "@/features/authentication/data/enums";
import { getSafeRedirectUrl } from "@/lib/urlSanitizer";

import { AuthApi } from "../api";

const refreshAxios = globalAxios.create();
refreshAxios.defaults.withCredentials = true;

// Cross-tab mutex using localStorage
const REFRESH_LOCK_KEY = "auth_refresh_lock";
const REFRESH_LOCK_TIMEOUT = 10000; // 10 seconds max lock time

const acquireLock = (): boolean => {
  const now = Date.now();
  const lockData = localStorage.getItem(REFRESH_LOCK_KEY);

  if (lockData) {
    const lockTime = parseInt(lockData, 10);
    // If lock is still valid (not expired), can't acquire
    if (now - lockTime < REFRESH_LOCK_TIMEOUT) {
      return false;
    }
  }

  // Acquire the lock
  localStorage.setItem(REFRESH_LOCK_KEY, now.toString());
  return true;
};

const releaseLock = () => {
  localStorage.removeItem(REFRESH_LOCK_KEY);
};

const waitForLock = (): Promise<void> => {
  return new Promise((resolve) => {
    const checkLock = () => {
      const lockData = localStorage.getItem(REFRESH_LOCK_KEY);
      if (
        !lockData ||
        Date.now() - parseInt(lockData, 10) >= REFRESH_LOCK_TIMEOUT
      ) {
        resolve();
      } else {
        setTimeout(checkLock, 100);
      }
    };
    checkLock();
  });
};

// In-tab queue for failed requests
let failedQueue: Array<{
  resolve: (token: string) => void;
  reject: (error: unknown) => void;
}> = [];
let isRefreshingInTab = false;

const processQueue = (error: unknown, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token!);
    }
  });
  failedQueue = [];
};

globalAxios.interceptors.response.use(undefined, async (error: AxiosError) => {
  const originalRequest = error.config as InternalAxiosRequestConfig & {
    _retry?: boolean;
  };

  if (
    error.response?.status === 401 &&
    originalRequest &&
    !originalRequest._retry
  ) {
    const deviceId = localStorage.getItem("deviceId");

    const currentPath =
      globalThis.location.pathname + globalThis.location.search;
    const safeRedirect = getSafeRedirectUrl(currentPath, "/home");

    if (!deviceId) {
      router.navigate({
        to: "/auth",
        search: { authMode: authModes.logIn, redirect: safeRedirect },
      });
      throw error;
    }

    // If already refreshing in THIS tab, queue this request
    if (isRefreshingInTab) {
      return new Promise<string>((resolve, reject) => {
        failedQueue.push({ resolve, reject });
      }).then((token) => {
        originalRequest.headers.Authorization = `Bearer ${token}`;
        return globalAxios(originalRequest);
      });
    }

    originalRequest._retry = true;

    // Try to acquire cross-tab lock
    if (!acquireLock()) {
      // Another tab is refreshing, wait for it and retry the original request
      await waitForLock();
      // After lock is released, just retry - the other tab should have refreshed
      return globalAxios(originalRequest);
    }

    isRefreshingInTab = true;

    try {
      const response = await new AuthApi(
        undefined,
        undefined,
        refreshAxios,
      ).refreshToken({ deviceId: deviceId });

      const newToken = response.data.tokenValue;
      globalAxios.defaults.headers.common["Authorization"] =
        `Bearer ${newToken}`;

      processQueue(null, newToken);

      originalRequest.headers.Authorization = `Bearer ${newToken}`;
      return globalAxios(originalRequest);
    } catch (refreshError) {
      processQueue(refreshError, null);

      router.navigate({
        to: "/auth",
        search: { authMode: authModes.logIn, redirect: safeRedirect },
      });
      throw refreshError;
    } finally {
      isRefreshingInTab = false;
      releaseLock();
    }
  }

  throw error;
});

globalAxios.defaults.withCredentials = true;

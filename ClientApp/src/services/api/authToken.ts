import globalAxios from "axios";

export function setAuthToken(token: string) {
  globalAxios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
}

export function clearAuthToken() {
  globalAxios.defaults.headers.common["Authorization"] = undefined;
}

import globalAxios from "axios";

const replaceNullWithUndefined = <T>(obj: T): T => {
  if (obj === null) {
    return undefined as T;
  }

  if (Array.isArray(obj)) {
    return obj.map(replaceNullWithUndefined) as T;
  }

  if (typeof obj === 'object' && obj !== null) {
    const result = {} as Record<string, unknown>;
    for (const key in obj) {
      result[key] = replaceNullWithUndefined((obj as Record<string, unknown>)[key]);
    }
    return result as T;
  }

  return obj;
};

// Add response interceptor
globalAxios.interceptors.response.use((response) => {
  if (response.data) {
    response.data = replaceNullWithUndefined(response.data);
  }
  return response;
});

// Also apply to the default axios instance
globalAxios.defaults.withCredentials = true; 
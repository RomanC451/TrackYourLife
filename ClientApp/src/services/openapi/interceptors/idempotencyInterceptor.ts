import globalAxios from "axios";

// Add the interceptor
globalAxios.interceptors.request.use(
  async (request) => {
    return request;
  },
  (error) => {
    // Optionally delay errors as well
    return Promise.reject(error);
  },
);

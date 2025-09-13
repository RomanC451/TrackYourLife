import globalAxios from "axios";

import { wait } from "@/lib/wait";

// Add the interceptor
globalAxios.interceptors.response.use(
  async (response) => {
    if (process.env.NODE_ENV !== "development") {
      return response;
    }

    // Get the request URL from the response config
    const url = response.config.url;

    // Exclude requests to /api/users (adjust the match as needed)
    if (url && url.includes("/api/users")) {
      return response; // No delay
    }

    const throttling = localStorage.getItem("throttling");

    // Otherwise, delay the response
    await wait(Number(throttling));
    return response;
  },
  (error) => {
    // Optionally delay errors as well
    return Promise.reject(error);
  },
);

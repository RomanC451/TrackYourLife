import { apiUrl } from "../env";

export async function waitForApi(
  url = apiUrl,
  maxAttempts = 120,
  delayMs = 1000,
) {
  let lastError: unknown;

  for (let attempt = 0; attempt < maxAttempts; attempt++) {
    try {
      const response = await fetch(`${url}/swagger/index.html`, {
        signal: AbortSignal.timeout(5_000),
      });
      if (response.ok) {
        return;
      }
    } catch (error) {
      lastError = error;
    }

    await new Promise((resolve) => setTimeout(resolve, delayMs));
  }

  throw new Error(
    `E2E API at ${url} is not reachable after ${maxAttempts}s. ` +
      `Run "npm run test:e2e" from ClientApp so Playwright starts the API and frontend, ` +
      `or start the API manually on port 5244.\n${lastError}`,
  );
}

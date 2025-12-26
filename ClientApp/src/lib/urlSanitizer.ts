/**
 * URL sanitization utilities for preventing malicious redirects
 */

/**
 * Sanitizes a redirect URL to prevent open redirect vulnerabilities
 * @param redirectUrl - The URL to sanitize
 * @returns A safe redirect path or null if invalid
 */
export function sanitizeRedirectUrl(
  redirectUrl: string | undefined,
): string | null {
  if (!redirectUrl) {
    return null;
  }

  // Remove any URL encoding to prevent double-encoding issues
  const decodedUrl = decodeURIComponent(redirectUrl);

  // Check if it's a relative path (starts with /)
  if (decodedUrl.startsWith("/")) {
    // Ensure it doesn't contain any protocol or host
    if (decodedUrl.includes("://") || decodedUrl.includes("//")) {
      return null;
    }

    // Prevent directory traversal attacks
    if (decodedUrl.includes("..") || decodedUrl.includes("~")) {
      return null;
    }

    // Ensure it's a valid path (no query params that could be malicious)
    const pathOnly = decodedUrl.split("?")[0];

    // Allow only alphanumeric, hyphens, underscores, and forward slashes
    // eslint-disable-next-line no-useless-escape
    if (!/^[a-zA-Z0-9\-_\/]*$/.test(pathOnly)) {
      return null;
    }

    if (pathOnly.includes("/auth")) {
      return null;
    }

    return pathOnly;
  }

  // If it's not a relative path, reject it
  return null;
}

/**
 * Validates that a redirect URL is safe and within the application
 * @param redirectUrl - The URL to validate
 * @returns true if the URL is safe, false otherwise
 */
export function isValidRedirectUrl(redirectUrl: string | undefined): boolean {
  return sanitizeRedirectUrl(redirectUrl) !== null;
}

/**
 * Gets a safe redirect URL with fallback
 * @param redirectUrl - The URL to sanitize
 * @param fallback - Fallback URL if sanitization fails
 * @returns A safe redirect URL
 */
export function getSafeRedirectUrl(
  redirectUrl: string | undefined,
  fallback: string = "/home",
): string {
  const sanitized = sanitizeRedirectUrl(redirectUrl);
  return sanitized || fallback;
}

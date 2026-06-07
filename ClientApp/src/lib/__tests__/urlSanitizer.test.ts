import { describe, expect, it } from "vitest";

import {
  getSafeRedirectUrl,
  isValidRedirectUrl,
  sanitizeRedirectUrl,
} from "../urlSanitizer";

describe("sanitizeRedirectUrl", () => {
  it("returns null for undefined or empty input", () => {
    expect(sanitizeRedirectUrl(undefined)).toBeNull();
    expect(sanitizeRedirectUrl("")).toBeNull();
  });

  it("accepts safe relative paths", () => {
    expect(sanitizeRedirectUrl("/home")).toBe("/home");
    expect(sanitizeRedirectUrl("/trainings/overview")).toBe(
      "/trainings/overview",
    );
  });

  it("strips query params and returns path only", () => {
    expect(sanitizeRedirectUrl("/home?redirect=evil")).toBe("/home");
  });

  it("rejects absolute URLs", () => {
    expect(sanitizeRedirectUrl("https://evil.com")).toBeNull();
    expect(sanitizeRedirectUrl("http://evil.com/path")).toBeNull();
  });

  it("rejects protocol-relative URLs", () => {
    expect(sanitizeRedirectUrl("//evil.com")).toBeNull();
    expect(sanitizeRedirectUrl("/path//evil")).toBeNull();
  });

  it("rejects directory traversal patterns", () => {
    expect(sanitizeRedirectUrl("/../etc/passwd")).toBeNull();
    expect(sanitizeRedirectUrl("/path/~user")).toBeNull();
  });

  it("rejects paths with disallowed characters", () => {
    expect(sanitizeRedirectUrl("/path with spaces")).toBeNull();
    expect(sanitizeRedirectUrl("/path@evil")).toBeNull();
  });

  it("rejects /auth paths", () => {
    expect(sanitizeRedirectUrl("/auth/login")).toBeNull();
    expect(sanitizeRedirectUrl("/settings/auth")).toBeNull();
  });

  it("decodes URL-encoded paths before validating", () => {
    expect(sanitizeRedirectUrl("%2Fhome")).toBe("/home");
    expect(sanitizeRedirectUrl("%2Fauth%2Flogin")).toBeNull();
  });
});

describe("isValidRedirectUrl", () => {
  it("returns true for safe paths", () => {
    expect(isValidRedirectUrl("/home")).toBe(true);
  });

  it("returns false for unsafe paths", () => {
    expect(isValidRedirectUrl("https://evil.com")).toBe(false);
    expect(isValidRedirectUrl(undefined)).toBe(false);
  });
});

describe("getSafeRedirectUrl", () => {
  it("returns sanitized path when valid", () => {
    expect(getSafeRedirectUrl("/trainings")).toBe("/trainings");
  });

  it("returns default fallback when invalid", () => {
    expect(getSafeRedirectUrl("https://evil.com")).toBe("/home");
  });

  it("returns custom fallback when invalid", () => {
    expect(getSafeRedirectUrl(undefined, "/dashboard")).toBe("/dashboard");
  });
});

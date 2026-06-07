import { beforeEach, describe, expect, it, vi } from "vitest";

import {
  generateYoutubeSettingsPassword,
  YOUTUBE_SETTINGS_PASSWORD_HINT,
} from "../generateYoutubeSettingsPassword";

describe("generateYoutubeSettingsPassword", () => {
  beforeEach(() => {
    let counter = 0;
    vi.spyOn(crypto, "getRandomValues").mockImplementation((array) => {
      const typed = array as Uint32Array;
      typed[0] = counter++;
      return typed;
    });
  });

  it("generates passwords that satisfy complexity rules", () => {
    const password = generateYoutubeSettingsPassword();

    expect(password.length).toBeGreaterThanOrEqual(10);
    expect(password).toMatch(/[a-z]/);
    expect(password).toMatch(/[A-Z]/);
    expect(password).toMatch(/\d/);
    expect(password).toMatch(/[@#$%^&+=!.]/);
  });

  it("honors a custom minimum length", () => {
    const password = generateYoutubeSettingsPassword(20);

    expect(password).toHaveLength(20);
  });

  it("exposes a password hint", () => {
    expect(YOUTUBE_SETTINGS_PASSWORD_HINT).toContain("At least 10 characters");
  });
});

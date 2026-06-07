import { describe, expect, it } from "vitest";

import { youtubeSettings } from "@/features/youtube/__tests__/fixtures";

import { toYoutubeSettingsView } from "../youtubeSettingsTypes";

describe("toYoutubeSettingsView", () => {
  it("defaults hasSettingsPassword to false", () => {
    expect(toYoutubeSettingsView(youtubeSettings())).toEqual({
      ...youtubeSettings(),
      hasSettingsPassword: false,
    });
  });

  it("preserves hasSettingsPassword when present on the dto", () => {
    const dto = { ...youtubeSettings(), hasSettingsPassword: true };

    expect(toYoutubeSettingsView(dto)).toEqual({
      ...youtubeSettings(),
      hasSettingsPassword: true,
    });
  });
});

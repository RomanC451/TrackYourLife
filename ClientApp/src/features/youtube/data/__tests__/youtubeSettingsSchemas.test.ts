import { describe, expect, it } from "vitest";

import { youtubeCategory, youtubeSettings } from "@/features/youtube/__tests__/fixtures";

import {
  changeYoutubeSettingsPasswordFormSchema,
  removeYoutubeSettingsPasswordFormSchema,
  setYoutubeSettingsPasswordFormSchema,
  youtubeSettingsDtoToForm,
  youtubeSettingsFormSchema,
} from "../youtubeSettingsSchemas";

describe("youtubeSettingsSchemas", () => {
  it("maps settings dto to sorted form values", () => {
    const form = youtubeSettingsDtoToForm(youtubeSettings());

    expect(form.categoryLimits).toEqual([
      {
        categoryId: "22222222-2222-4222-8222-222222222222",
        maxVideosPerDay: 5,
      },
      {
        categoryId: "11111111-1111-4111-8111-111111111111",
        maxVideosPerDay: 5,
      },
    ]);
    expect(youtubeSettingsFormSchema.parse(form)).toEqual(form);
  });

  it("validates password setup forms", () => {
    expect(
      setYoutubeSettingsPasswordFormSchema.safeParse({
        newPassword: "Aa1!aaaaaa",
        confirmPassword: "Aa1!aaaaaa",
      }).success,
    ).toBe(true);

    expect(
      setYoutubeSettingsPasswordFormSchema.safeParse({
        newPassword: "Aa1!aaaaaa",
        confirmPassword: "Mismatch1!",
      }).success,
    ).toBe(false);
  });

  it("validates password change and removal forms", () => {
    expect(
      changeYoutubeSettingsPasswordFormSchema.safeParse({
        currentPassword: "old",
        newPassword: "Bb2!bbbbbb",
        confirmPassword: "Bb2!bbbbbb",
      }).success,
    ).toBe(true);

    expect(
      removeYoutubeSettingsPasswordFormSchema.safeParse({
        currentPassword: "old",
      }).success,
    ).toBe(true);
  });

  it("rejects invalid category limits", () => {
    expect(
      youtubeSettingsFormSchema.safeParse({
        categoryLimits: [
          {
            categoryId: youtubeCategory("cat-1").id,
            maxVideosPerDay: -1,
          },
        ],
      }).success,
    ).toBe(false);
  });
});

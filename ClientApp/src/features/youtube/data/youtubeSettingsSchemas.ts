import { z } from "zod";

import type { UpdateYoutubeSettingsRequest, YoutubeSettingsDto } from "@/services/openapi";
import { DayOfWeek, SettingsChangeFrequency } from "@/services/openapi";

import { sortYoutubeCategoriesByDisplayOrder } from "../youtubeListSearch";

const categoryLimitRowSchema = z.object({
  categoryId: z.string().uuid(),
  maxVideosPerDay: z.coerce
    .number()
    .int()
    .min(0, { message: "Must be 0 or greater" }),
});

export const youtubeSettingsFormSchema = z
  .object({
    categoryLimits: z.array(categoryLimitRowSchema).min(1),
    settingsChangeFrequency: z.nativeEnum(SettingsChangeFrequency, {
      required_error: "Please select a frequency",
    }),
    daysBetweenChanges: z.coerce.number().int().min(1).optional(),
    specificDayOfWeek: z.nativeEnum(DayOfWeek).optional(),
    specificDayOfMonth: z.coerce
      .number()
      .int()
      .min(1)
      .max(31, { message: "Day must be between 1 and 31" })
      .optional(),
  })
  .refine(
    (data) => {
      if (
        data.settingsChangeFrequency ===
        SettingsChangeFrequency.OnceEveryFewDays
      ) {
        return (
          data.daysBetweenChanges !== undefined && data.daysBetweenChanges >= 1
        );
      }
      return true;
    },
    {
      message: "Days between changes is required",
      path: ["daysBetweenChanges"],
    },
  )
  .refine(
    (data) => {
      if (
        data.settingsChangeFrequency ===
        SettingsChangeFrequency.SpecificDayOfWeek
      ) {
        return data.specificDayOfWeek !== undefined;
      }
      return true;
    },
    {
      message: "Day of week is required",
      path: ["specificDayOfWeek"],
    },
  )
  .refine(
    (data) => {
      if (
        data.settingsChangeFrequency ===
        SettingsChangeFrequency.SpecificDayOfMonth
      ) {
        return (
          data.specificDayOfMonth !== undefined &&
          data.specificDayOfMonth >= 1 &&
          data.specificDayOfMonth <= 31
        );
      }
      return true;
    },
    {
      message: "Day of month is required (1-31)",
      path: ["specificDayOfMonth"],
    },
  );

export type YoutubeSettingsFormSchema = z.infer<
  typeof youtubeSettingsFormSchema
>;

export function youtubeSettingsFormDataToUpdateRequest(
  formData: YoutubeSettingsFormSchema,
): UpdateYoutubeSettingsRequest {
  return {
    settingsChangeFrequency: formData.settingsChangeFrequency,
    daysBetweenChanges:
      formData.settingsChangeFrequency ===
      SettingsChangeFrequency.OnceEveryFewDays
        ? formData.daysBetweenChanges
        : undefined,
    specificDayOfWeek:
      formData.settingsChangeFrequency ===
      SettingsChangeFrequency.SpecificDayOfWeek
        ? formData.specificDayOfWeek
        : undefined,
    specificDayOfMonth:
      formData.settingsChangeFrequency ===
      SettingsChangeFrequency.SpecificDayOfMonth
        ? formData.specificDayOfMonth
        : undefined,
    // OpenAPI client still lists categoryLimits until regenerated.
  } as UpdateYoutubeSettingsRequest;
}

export function youtubeSettingsDtoToForm(
  dto: YoutubeSettingsDto,
): YoutubeSettingsFormSchema {
  const sorted = sortYoutubeCategoriesByDisplayOrder(dto.categories);
  return {
    categoryLimits: sorted.map((c) => ({
      categoryId: c.id,
      maxVideosPerDay: c.maxVideosPerDay,
    })),
    settingsChangeFrequency:
      dto.settingsChangeFrequency ??
      SettingsChangeFrequency.OnceEveryFewDays,
    daysBetweenChanges: dto.daysBetweenChanges,
    specificDayOfWeek: dto.specificDayOfWeek,
    specificDayOfMonth: dto.specificDayOfMonth,
  };
}

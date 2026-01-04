import { z } from "zod";

import { DayOfWeek, SettingsChangeFrequency } from "@/services/openapi";

export const youtubeSettingsFormSchema = z
  .object({
    maxEntertainmentVideosPerDay: z
      .number()
      .int()
      .min(0, { message: "Must be 0 or greater" }),
    settingsChangeFrequency: z.nativeEnum(SettingsChangeFrequency, {
      required_error: "Please select a frequency",
    }),
    daysBetweenChanges: z.number().int().min(1).optional(),
    specificDayOfWeek: z.nativeEnum(DayOfWeek).optional(),
    specificDayOfMonth: z
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

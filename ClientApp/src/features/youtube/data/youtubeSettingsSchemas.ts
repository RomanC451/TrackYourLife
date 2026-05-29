import { z } from "zod";

import type { YoutubeSettingsDto } from "@/services/openapi";

import { sortYoutubeCategoriesByDisplayOrder } from "../youtubeListSearch";

const categoryLimitRowSchema = z.object({
  categoryId: z.string().uuid(),
  maxVideosPerDay: z.coerce
    .number()
    .int()
    .min(0, { message: "Must be 0 or greater" }),
});

export const youtubeSettingsFormSchema = z.object({
  categoryLimits: z.array(categoryLimitRowSchema).min(1),
});

export type YoutubeSettingsFormSchema = z.infer<
  typeof youtubeSettingsFormSchema
>;

export function youtubeSettingsDtoToForm(
  dto: YoutubeSettingsDto,
): YoutubeSettingsFormSchema {
  const sorted = sortYoutubeCategoriesByDisplayOrder(dto.categories);
  return {
    categoryLimits: sorted.map((c) => ({
      categoryId: c.id,
      maxVideosPerDay: c.maxVideosPerDay,
    })),
  };
}

const passwordFieldSchema = z
  .string()
  .min(10, { message: "Password must be at least 10 characters" })
  .regex(/[a-z]/, { message: "Must include a lowercase letter" })
  .regex(/[A-Z]/, { message: "Must include an uppercase letter" })
  .regex(/\d/, { message: "Must include a number" })
  .regex(/[@#$%^&+=!.]/, {
    message: "Must include a special character (@#$%^&+=!.)",
  });

export const setYoutubeSettingsPasswordFormSchema = z
  .object({
    newPassword: passwordFieldSchema,
    confirmPassword: z.string(),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

export type SetYoutubeSettingsPasswordFormSchema = z.infer<
  typeof setYoutubeSettingsPasswordFormSchema
>;

export const changeYoutubeSettingsPasswordFormSchema = z
  .object({
    currentPassword: z.string().min(1, { message: "Current password is required" }),
    newPassword: passwordFieldSchema,
    confirmPassword: z.string(),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

export type ChangeYoutubeSettingsPasswordFormSchema = z.infer<
  typeof changeYoutubeSettingsPasswordFormSchema
>;

export const removeYoutubeSettingsPasswordFormSchema = z.object({
  currentPassword: z.string().min(1, { message: "Current password is required" }),
});

export type RemoveYoutubeSettingsPasswordFormSchema = z.infer<
  typeof removeYoutubeSettingsPasswordFormSchema
>;

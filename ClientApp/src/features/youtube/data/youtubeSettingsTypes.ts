import type { YoutubeSettingsDto } from "@/services/openapi";

/** Extends generated DTO until OpenAPI client is regenerated. */
export type YoutubeSettingsView = YoutubeSettingsDto & {
  hasSettingsPassword: boolean;
};

export function toYoutubeSettingsView(dto: YoutubeSettingsDto): YoutubeSettingsView {
  const extended = dto as YoutubeSettingsDto & { hasSettingsPassword?: boolean };
  return {
    ...dto,
    hasSettingsPassword: extended.hasSettingsPassword ?? false,
  };
}

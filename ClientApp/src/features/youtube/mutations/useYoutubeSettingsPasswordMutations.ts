import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { handleApiError } from "@/services/openapi/handleApiError";
import {
  SettingsApi,
  type SetYoutubeSettingsPasswordRequest,
  type VerifyYoutubeSettingsPasswordRequest,
} from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import { youtubeQueryKeys } from "../queries/youtubeQueries";

const settingsApi = new SettingsApi();

const RESET_EMAIL_ERROR_MESSAGES: Record<string, string> = {
  "Youtube.Settings.ResetEmailRateLimited":
    "A reset email was sent recently. Please wait a few minutes and try again.",
  "Youtube.Settings.AccountEmailNotVerified":
    "Verify your account email before resetting the settings password.",
  "Youtube.Settings.FailedToSendResetEmail":
    "Could not send the email. Your settings password was not changed.",
  "Youtube.Settings.PasswordNotSet":
    "No settings password is configured.",
};

export function useVerifyYoutubeSettingsPasswordMutation() {
  return useCustomMutation({
    mutationFn: (body: VerifyYoutubeSettingsPasswordRequest) =>
      settingsApi.verifyYoutubeSettingsPassword(body),
    onError: (error: ApiError) => {
      handleApiError({ error });
    },
  });
}

export function useSetYoutubeSettingsPasswordMutation() {
  return useCustomMutation({
    mutationFn: (body: SetYoutubeSettingsPasswordRequest) =>
      settingsApi.setYoutubeSettingsPassword(body),
    meta: {
      invalidateQueries: [youtubeQueryKeys.settings()],
      onSuccessToast: { message: "Settings lock updated", type: "success" },
    },
    onError: (error: ApiError) => {
      handleApiError({ error });
    },
  });
}

export function useResetYoutubeSettingsPasswordViaEmailMutation() {
  return useCustomMutation({
    mutationFn: () => settingsApi.resetYoutubeSettingsPasswordViaEmail(),
    onSuccess: () => {
      toast.success("Check your email for a new settings password");
    },
    onError: (error: ApiError) => {
      const errorType = error.response?.data?.type;
      const message =
        (errorType && RESET_EMAIL_ERROR_MESSAGES[errorType]) ||
        error.response?.data?.detail;

      handleApiError({
        error,
        defaultHandler: () => {
          toast.error(message ?? "Could not send reset email");
        },
      });
    },
  });
}

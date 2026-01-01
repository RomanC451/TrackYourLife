import { ErrorOption } from "react-hook-form";
import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";

import "@/queryClient";

import { SettingsApi, UpdateYoutubeSettingsRequest } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

import { YoutubeSettingsFormSchema } from "../data/youtubeSettingsSchemas";
import { youtubeQueryKeys } from "../queries/youtubeQueries";

const settingsApi = new SettingsApi();

type Variables = {
  request: UpdateYoutubeSettingsRequest;
  setError: (
    name: keyof YoutubeSettingsFormSchema,
    error: ErrorOption,
    options?: {
      shouldFocus: boolean;
    },
  ) => void;
};

function useUpdateYoutubeSettingsMutation() {
  const updateSettingsMutation = useCustomMutation({
    mutationFn: ({ request }: Variables) => {
      return settingsApi.updateYoutubeSettings(request);
    },

    meta: {
      noDefaultErrorToast: true,
      onSuccessToast: {
        message: "Settings updated successfully",
        type: "success",
      },
      invalidateQueries: [
        youtubeQueryKeys.settings(),
        youtubeQueryKeys.dailyCounter(),
      ],
    },

    onError: (error: ApiError, variables) => {
      handleApiError({
        error,
        errorHandlers: {
          400: {
            default: (errorData) => {
              toast.error("Failed to update settings", {
                description: errorData.detail,
              });
            },
          },
          403: {
            default: (errorData) => {
              toast.error("Settings change not allowed", {
                description:
                  errorData.detail ||
                  "You cannot change settings at this time based on your frequency rules.",
              });
            },
          },
        },
        validationErrorsHandler: variables.setError,
        defaultHandler: () => {
          toast.error("Failed to update settings");
        },
      });
    },
  });

  return updateSettingsMutation;
}

export default useUpdateYoutubeSettingsMutation;

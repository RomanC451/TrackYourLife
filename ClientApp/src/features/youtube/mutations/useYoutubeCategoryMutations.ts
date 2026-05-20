import { useNavigate } from "@tanstack/react-router";
import globalAxios from "axios";
import { StatusCodes } from "http-status-codes";
import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import {
  SettingsApi,
  type CreateYoutubeCategoryRequest,
  type UpdateYoutubeCategoryMetadataRequest,
} from "@/services/openapi";
import { BASE_PATH } from "@/services/openapi/base";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

import { youtubeQueryKeys } from "../queries/youtubeQueries";

const settingsApi = new SettingsApi();

/** Until OpenAPI client is regenerated after adding PUT .../categories/{id}/limit. */
export type UpdateYoutubeCategoryLimitRequest = {
  maxVideosPerDay: number;
};

async function updateYoutubeCategoryLimit(
  id: string,
  body: UpdateYoutubeCategoryLimitRequest,
) {
  await globalAxios.put(`${BASE_PATH}/api/settings/categories/${id}/limit`, body);
}

export function useCreateYoutubeCategoryMutation() {
  const navigate = useNavigate();

  return useCustomMutation({
    mutationFn: (body: CreateYoutubeCategoryRequest) =>
      settingsApi.createYoutubeCategory(body),
    meta: {
      onSuccessToast: { message: "Category created", type: "success" },
      invalidateQueries: [youtubeQueryKeys.all],
    },
    onError: (error: ApiError) => {
      handleApiError({
        error,
        errorHandlers: {
          [StatusCodes.FORBIDDEN]: {
            "Youtube.Category.ForbiddenForPlan": () => {
              toast.error("This action requires Pro", {
                action: {
                  label: "Get Pro",
                  onClick: () => {
                    navigate({ to: "/upgrade" });
                  },
                },
              });
            },
          },
        },
        defaultHandler: () => {
          const detail = error.response?.data?.detail;
          toast.error(
            typeof detail === "string" ? detail : "Could not create category",
          );
        },
      });
    },
  });
}

export function useUpdateYoutubeCategoryMetadataMutation() {
  return useCustomMutation({
    mutationFn: (args: {
      id: string;
      body: UpdateYoutubeCategoryMetadataRequest;
    }) => settingsApi.updateYoutubeCategoryMetadata(args.id, args.body),
    meta: {
      onSuccessToast: { message: "Category updated", type: "success" },
      invalidateQueries: [youtubeQueryKeys.all],
    },
  });
}

export function useUpdateYoutubeCategoryLimitMutation() {
  return useCustomMutation({
    mutationFn: (args: {
      id: string;
      body: UpdateYoutubeCategoryLimitRequest;
    }) => updateYoutubeCategoryLimit(args.id, args.body),
    meta: {
      onSuccessToast: { message: "Category limit updated", type: "success" },
      invalidateQueries: [
        youtubeQueryKeys.settings(),
        youtubeQueryKeys.dailyCategoryWatchCounters(),
      ],
    },
    onError: (error: ApiError) => {
      const detail = error.response?.data?.detail;
      toast.error(
        typeof detail === "string" ? detail : "Could not update category limit",
      );
    },
  });
}

export function useDeleteYoutubeCategoryMutation() {
  return useCustomMutation({
    mutationFn: (args: { id: string; confirmUnsubscribeChannels: boolean }) =>
      settingsApi.deleteYoutubeCategory(args.id, args.confirmUnsubscribeChannels),
    meta: {
      onSuccessToast: { message: "Category removed", type: "success" },
      invalidateQueries: [youtubeQueryKeys.all],
    },
  });
}

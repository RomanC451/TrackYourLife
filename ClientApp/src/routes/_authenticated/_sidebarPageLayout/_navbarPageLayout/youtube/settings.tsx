import { Suspense } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import { AlertCircle, Loader2 } from "lucide-react";
import { ErrorOption } from "react-hook-form";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import useCustomForm from "@/components/forms/useCustomForm";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import YoutubeSettingsForm from "@/features/youtube/components/settings/YoutubeSettingsForm";
import {
  youtubeSettingsFormSchema,
  YoutubeSettingsFormSchema,
} from "@/features/youtube/data/youtubeSettingsSchemas";
import useUpdateYoutubeSettingsMutation from "@/features/youtube/mutations/useUpdateYoutubeSettingsMutation";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";
import { SettingsChangeFrequency } from "@/services/openapi";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/settings",
)({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <PageCard>
      <PageTitle title="YouTube Settings" />
      <Suspense
        fallback={
          <div className="flex items-center justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
          </div>
        }
      >
        <SettingsContent />
      </Suspense>
    </PageCard>
  );
}

function SettingsContent() {
  const updateSettingsMutation = useUpdateYoutubeSettingsMutation();

  const { data: settingsData } = useSuspenseQuery({
    ...youtubeQueryOptions.settings(),
  });

  console.log(settingsData);

  const { form, handleCustomSubmit } = useCustomForm({
    formSchema: youtubeSettingsFormSchema,
    defaultValues: settingsData ?? getDefaultValues(),
    queryData: settingsData,
    onSubmit: async (formData: YoutubeSettingsFormSchema) => {
      updateSettingsMutation.mutate({
        request: {
          maxDivertissmentVideosPerDay: formData.maxDivertissmentVideosPerDay,
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
        },
        setError: (name, error: ErrorOption, options) => {
          form.setError(name, error, options);
        },
      });
    },
  });

  const { data: dailyCounterData } = useSuspenseQuery({
    ...youtubeQueryOptions.dailyCounter(),
  });

  console.log(dailyCounterData);

  return (
    <div className="space-y-6">
      {dailyCounterData && (
        <Alert>
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>Daily Counter</AlertTitle>
          <AlertDescription>
            You have watched {dailyCounterData.videosWatchedCount} divertissment
            video{dailyCounterData.videosWatchedCount !== 1 ? "s" : ""} today.
            {settingsData && (
              <>
                {" "}
                Your limit is {settingsData.maxDivertissmentVideosPerDay} video
                {settingsData.maxDivertissmentVideosPerDay !== 1 ? "s" : ""} per
                day.
              </>
            )}
          </AlertDescription>
        </Alert>
      )}

      <YoutubeSettingsForm
        form={form}
        handleCustomSubmit={handleCustomSubmit}
        pendingState={updateSettingsMutation.pendingState}
      />
    </div>
  );
}

function getDefaultValues(): YoutubeSettingsFormSchema {
  return {
    maxDivertissmentVideosPerDay: 0,
    settingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
    daysBetweenChanges: 7,
    specificDayOfWeek: undefined,
    specificDayOfMonth: undefined,
  };
}

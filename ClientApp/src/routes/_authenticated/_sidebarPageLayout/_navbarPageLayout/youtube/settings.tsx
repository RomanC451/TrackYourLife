import { Suspense, useCallback, useMemo } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import { AlertCircle, Loader2 } from "lucide-react";
import { ErrorOption } from "react-hook-form";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import useCustomForm from "@/components/forms/useCustomForm";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Form } from "@/components/ui/form";
import YoutubeCategoriesSection from "@/features/youtube/components/settings/YoutubeCategoriesSection";
import YoutubeSettingsForm from "@/features/youtube/components/settings/YoutubeSettingsForm";
import {
  youtubeSettingsFormSchema,
  YoutubeSettingsFormSchema,
  youtubeSettingsDtoToForm,
  youtubeSettingsFormDataToUpdateRequest,
} from "@/features/youtube/data/youtubeSettingsSchemas";
import { useUpdateYoutubeCategoryLimitMutation } from "@/features/youtube/mutations/useYoutubeCategoryMutations";
import useUpdateYoutubeSettingsMutation from "@/features/youtube/mutations/useUpdateYoutubeSettingsMutation";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";
import { sortYoutubeCategoriesByDisplayOrder } from "@/features/youtube/youtubeListSearch";

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
  const updateCategoryLimitMutation = useUpdateYoutubeCategoryLimitMutation();

  const { data: settingsData } = useSuspenseQuery({
    ...youtubeQueryOptions.settings(),
  });

  const { data: dailyCounters } = useSuspenseQuery({
    ...youtubeQueryOptions.dailyCategoryWatchCounters(),
  });

  const sortedCategories = useMemo(
    () => sortYoutubeCategoriesByDisplayOrder(settingsData.categories),
    [settingsData.categories],
  );

  /** Content key: query `select` clones DTOs each read, so `settingsData` ref changes often. */
  const settingsFormSyncKey = useMemo(
    () =>
      JSON.stringify({
        categories: settingsData.categories.map((c) => ({
          id: c.id,
          name: c.name,
          maxVideosPerDay: c.maxVideosPerDay,
          displayOrder: c.displayOrder,
        })),
        settingsChangeFrequency: settingsData.settingsChangeFrequency,
        daysBetweenChanges: settingsData.daysBetweenChanges,
        specificDayOfWeek: settingsData.specificDayOfWeek,
        specificDayOfMonth: settingsData.specificDayOfMonth,
      }),
    [settingsData],
  );

  const formDefaults = useMemo(
    () => youtubeSettingsDtoToForm(settingsData),
    // settingsFormSyncKey captures meaningful server changes; settingsData is read when the key updates.
    // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: stabilize object identity for useCustomForm
    [settingsFormSyncKey],
  );

  const { form, handleCustomSubmit } = useCustomForm({
    formSchema: youtubeSettingsFormSchema,
    defaultValues: formDefaults,
    queryData: formDefaults,
    onSubmit: async (formData: YoutubeSettingsFormSchema) => {
      updateSettingsMutation.mutate({
        request: youtubeSettingsFormDataToUpdateRequest(formData),
        setError: (name, error: ErrorOption, options) => {
          form.setError(name, error, options);
        },
      });
    },
  });

  const handleSettingsSubmit = async (
    event: React.FormEvent<HTMLFormElement>,
  ) => {
    event.preventDefault();
    const result = await form.trigger();
    if (!result) {
      return;
    }
    form.clearErrors();
    handleCustomSubmit(event);
  };

  const saveCategoryLimit = useCallback(
    async (categoryId: string, maxVideosPerDay: number) => {
      await updateCategoryLimitMutation.mutateAsync({
        id: categoryId,
        body: { maxVideosPerDay },
      });
    },
    [updateCategoryLimitMutation],
  );

  return (
    <Form {...form}>
      <form onSubmit={handleSettingsSubmit} className="space-y-8">
      {(dailyCounters ?? []).length > 0 && (
        <Alert>
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>Today's watch counts</AlertTitle>
          <AlertDescription>
            <ul className="mt-2 list-inside list-disc space-y-1 text-sm">
              {(dailyCounters ?? []).map((row) => {
                const cat = settingsData.categories.find(
                  (c) => c.id === row.youtubeCategoryId,
                );
                const name = cat?.name ?? "Category";
                const limit = cat?.maxVideosPerDay;
                const limitText =
                  limit === undefined
                    ? ""
                    : limit === 0
                      ? " (unlimited)"
                      : ` / ${limit} allowed today`;
                return (
                  <li key={row.id}>
                    {name}: {row.videosWatchedCount} watched{limitText}
                  </li>
                );
              })}
            </ul>
          </AlertDescription>
        </Alert>
      )}

      <YoutubeCategoriesSection
        form={form}
        categories={sortedCategories}
        dailyCounters={dailyCounters}
        isPersistingCategoryLimits={updateCategoryLimitMutation.isPending}
        onSaveCategoryLimit={saveCategoryLimit}
      />

      {sortedCategories.length > 0 ? (
        <YoutubeSettingsForm
          form={form}
          pendingState={updateSettingsMutation.pendingState}
        />
      ) : (
        <p className="text-sm text-muted-foreground">
          Add a category above to set per-category daily limits and scheduling
          rules.
        </p>
      )}
      </form>
    </Form>
  );
}

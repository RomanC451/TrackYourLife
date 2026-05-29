import { Suspense, useCallback, useMemo } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import { AlertCircle, Loader2 } from "lucide-react";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import useCustomForm from "@/components/forms/useCustomForm";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Form } from "@/components/ui/form";
import { cn } from "@/lib/utils";
import YoutubeCategoriesSection from "@/features/youtube/components/settings/YoutubeCategoriesSection";
import YoutubeSettingsLockSection from "@/features/youtube/components/settings/YoutubeSettingsLockSection";
import YoutubeSettingsUnlockOverlay from "@/features/youtube/components/settings/YoutubeSettingsUnlockOverlay";
import {
  youtubeSettingsFormSchema,
  youtubeSettingsDtoToForm,
} from "@/features/youtube/data/youtubeSettingsSchemas";
import { toYoutubeSettingsView } from "@/features/youtube/data/youtubeSettingsTypes";
import { useYoutubeSettingsUnlock } from "@/features/youtube/hooks/useYoutubeSettingsUnlock";
import { useUpdateYoutubeCategoryLimitMutation } from "@/features/youtube/mutations/useYoutubeCategoryMutations";
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
  const updateCategoryLimitMutation = useUpdateYoutubeCategoryLimitMutation();

  const { data: settingsData } = useSuspenseQuery({
    ...youtubeQueryOptions.settings(),
  });

  const settings = useMemo(
    () => toYoutubeSettingsView(settingsData),
    [settingsData],
  );

  const { isUnlocked, unlock, isVerifying } = useYoutubeSettingsUnlock(
    settings.hasSettingsPassword,
  );

  const showLockOverlay = settings.hasSettingsPassword && !isUnlocked;

  const { data: dailyCounters } = useSuspenseQuery({
    ...youtubeQueryOptions.dailyCategoryWatchCounters(),
  });

  const sortedCategories = useMemo(
    () => sortYoutubeCategoriesByDisplayOrder(settings.categories),
    [settings.categories],
  );

  const settingsFormSyncKey = useMemo(
    () =>
      JSON.stringify({
        categories: settings.categories.map((c) => ({
          id: c.id,
          name: c.name,
          maxVideosPerDay: c.maxVideosPerDay,
          displayOrder: c.displayOrder,
        })),
        hasSettingsPassword: settings.hasSettingsPassword,
      }),
    [settings],
  );

  const formDefaults = useMemo(
    () => youtubeSettingsDtoToForm(settings),
    // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: stabilize object identity for useCustomForm
    [settingsFormSyncKey],
  );

  const { form } = useCustomForm({
    formSchema: youtubeSettingsFormSchema,
    defaultValues: formDefaults,
    queryData: formDefaults,
    onSubmit: async () => {},
  });

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
    <div className="relative">
      <div
        className={cn(
          "space-y-8",
          showLockOverlay && "pointer-events-none select-none blur-sm",
        )}
      >
        <Form {...form}>
          {(dailyCounters ?? []).length > 0 && (
            <Alert>
              <AlertCircle className="h-4 w-4" />
              <AlertTitle>Today's watch counts</AlertTitle>
              <AlertDescription>
                <ul className="mt-2 list-inside list-disc space-y-1 text-sm">
                  {(dailyCounters ?? []).map((row) => {
                    const cat = settings.categories.find(
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

          {sortedCategories.length === 0 ? (
            <p className="text-sm text-muted-foreground">
              Add a category above to set per-category daily limits.
            </p>
          ) : null}
        </Form>

        {isUnlocked ? (
          <YoutubeSettingsLockSection
            hasSettingsPassword={settings.hasSettingsPassword}
          />
        ) : null}
      </div>

      {showLockOverlay ? (
        <YoutubeSettingsUnlockOverlay
          onUnlock={unlock}
          isVerifying={isVerifying}
        />
      ) : null}
    </div>
  );
}

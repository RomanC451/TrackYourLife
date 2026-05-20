import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { UseFormReturn, useWatch } from "react-hook-form";
import {
  closestCenter,
  DndContext,
  type DragEndEvent,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
} from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { SettingsApi } from "@/services/openapi";
import type {
  DailyCategoryWatchCounterDto,
  YoutubeCategorySettingsDto,
} from "@/services/openapi";

import {
  useDeleteYoutubeCategoryMutation,
  useUpdateYoutubeCategoryMetadataMutation,
} from "../../mutations/useYoutubeCategoryMutations";
import { youtubeQueryKeys } from "../../queries/youtubeQueries";
import { sortYoutubeCategoriesByDisplayOrder } from "../../youtubeListSearch";
import type { YoutubeSettingsFormSchema } from "../../data/youtubeSettingsSchemas";

import { SortableYoutubeCategoryRow } from "./SortableYoutubeCategoryRow";
import { AddYoutubeCategoryDialog } from "./AddYoutubeCategoryDialog";

const settingsApi = new SettingsApi();

/** Until OpenAPI client is regenerated, `subscribedChannelCount` may be absent from the TS type. */
function getSubscribedChannelCount(c: YoutubeCategorySettingsDto): number {
  const v = (c as YoutubeCategorySettingsDto & { subscribedChannelCount?: unknown })
    .subscribedChannelCount;
  return typeof v === "number" && Number.isFinite(v) ? v : 0;
}

interface YoutubeCategoriesSectionProps {
  form: UseFormReturn<YoutubeSettingsFormSchema>;
  categories: YoutubeCategorySettingsDto[];
  dailyCounters: DailyCategoryWatchCounterDto[] | undefined;
  isPersistingCategoryLimits: boolean;
  onSaveCategoryLimit: (
    categoryId: string,
    maxVideosPerDay: number,
  ) => Promise<void>;
}

type EditState = Record<string, { name: string; displayOrder: number }>;

function YoutubeCategoriesSection({
  form,
  categories,
  dailyCounters,
  isPersistingCategoryLimits,
  onSaveCategoryLimit,
}: YoutubeCategoriesSectionProps) {
  const categoryLimits = useWatch({
    control: form.control,
    name: "categoryLimits",
  });

  const queryClient = useQueryClient();
  const sorted = useMemo(
    () => sortYoutubeCategoriesByDisplayOrder(categories),
    [categories],
  );

  const [orderedIds, setOrderedIds] = useState<string[]>(() =>
    sortYoutubeCategoriesByDisplayOrder(categories).map((c) => c.id),
  );
  const [edits, setEdits] = useState<EditState>({});
  const [deleteTargetId, setDeleteTargetId] = useState<string | null>(null);
  const [isSavingOrder, setIsSavingOrder] = useState(false);

  const orderedIdsRef = useRef(orderedIds);
  orderedIdsRef.current = orderedIds;
  const editsRef = useRef(edits);
  editsRef.current = edits;

  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    }),
  );

  useEffect(() => {
    setOrderedIds(sorted.map((c) => c.id));
    setEdits(
      Object.fromEntries(
        sorted.map((c) => [
          c.id,
          { name: c.name, displayOrder: c.displayOrder },
        ]),
      ),
    );
  }, [sorted]);

  const categoryById = useMemo(
    () => new Map(sorted.map((c) => [c.id, c])),
    [sorted],
  );

  const updateMutation = useUpdateYoutubeCategoryMetadataMutation();
  const deleteMutation = useDeleteYoutubeCategoryMutation();

  const watchedByCategoryId = useMemo(() => {
    const map = new Map<string, number>();
    for (const row of dailyCounters ?? []) {
      map.set(row.youtubeCategoryId, row.videosWatchedCount);
    }
    return map;
  }, [dailyCounters]);

  const persistDisplayOrder = async (
    newIds: string[],
    nextEdits: EditState,
  ) => {
    const serverIds = sortYoutubeCategoriesByDisplayOrder(categories).map(
      (c) => c.id,
    );
    if (newIds.join(",") === serverIds.join(",")) {
      return;
    }

    setIsSavingOrder(true);
    try {
      const payloads = newIds.map((id, index) => ({
        id,
        body: {
          name: (
            nextEdits[id]?.name.trim() ||
            categories.find((c) => c.id === id)?.name ||
            ""
          ).trim(),
          displayOrder: index,
        },
      }));
      await Promise.all(
        payloads.map((p) =>
          settingsApi.updateYoutubeCategoryMetadata(p.id, p.body),
        ),
      );
      await queryClient.invalidateQueries({ queryKey: youtubeQueryKeys.all });
      toast.success("Category order saved");
    } catch {
      toast.error("Could not save category order");
      setOrderedIds(serverIds);
      setEdits(
        Object.fromEntries(
          sortYoutubeCategoriesByDisplayOrder(categories).map((c) => [
            c.id,
            { name: c.name, displayOrder: c.displayOrder },
          ]),
        ),
      );
    } finally {
      setIsSavingOrder(false);
    }
  };

  const handleNameBlurSave = useCallback(
    async (id: string, serverName: string) => {
      if (updateMutation.isPending || isSavingOrder) {
        return;
      }
      const row = editsRef.current[id];
      if (!row) {
        return;
      }
      const trimmed = row.name.trim();
      if (!trimmed || trimmed === serverName) {
        return;
      }
      try {
        await updateMutation.mutateAsync({
          id,
          body: { name: trimmed, displayOrder: row.displayOrder },
        });
      } catch {
        setEdits((prev) => {
          const cur = prev[id];
          if (!cur) {
            return prev;
          }
          return {
            ...prev,
            [id]: { ...cur, name: serverName },
          };
        });
      }
    },
    [updateMutation, isSavingOrder],
  );

  const handleSaveMaxVideosPerDay = useCallback(
    async (categoryId: string, maxVideosPerDay: number) => {
      if (isSavingOrder || isPersistingCategoryLimits) {
        return false;
      }

      try {
        await onSaveCategoryLimit(categoryId, maxVideosPerDay);
      } catch {
        return false;
      }
      return true;
    },
    [isSavingOrder, isPersistingCategoryLimits, onSaveCategoryLimit],
  );

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;
    if (!over || active.id === over.id) {
      return;
    }

    const prevOrderedIds = orderedIdsRef.current;
    const oldIndex = prevOrderedIds.indexOf(String(active.id));
    const newIndex = prevOrderedIds.indexOf(String(over.id));
    if (oldIndex === -1 || newIndex === -1) {
      return;
    }

    const newIds = arrayMove(prevOrderedIds, oldIndex, newIndex);
    setOrderedIds(newIds);

    const prevEdits = editsRef.current;
    const nextEdits: EditState = { ...prevEdits };
    newIds.forEach((id, i) => {
      const r = nextEdits[id];
      if (r) {
        nextEdits[id] = { ...r, displayOrder: i };
      }
    });
    setEdits(nextEdits);
    void persistDisplayOrder(newIds, nextEdits);
  };

  const deleteTargetName =
    deleteTargetId === null ? "" : (edits[deleteTargetId]?.name ?? "");

  return (
    <div className="space-y-4 rounded-lg border p-4">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <h3 className="text-sm font-medium leading-none">Categories</h3>
        <AddYoutubeCategoryDialog disabled={isSavingOrder} />
      </div>

      <DndContext
        sensors={sensors}
        collisionDetection={closestCenter}
        onDragEnd={handleDragEnd}
      >
        <SortableContext
          items={orderedIds}
          strategy={verticalListSortingStrategy}
        >
          <div className="space-y-3">
            {orderedIds.map((id, index) => {
              const c = categoryById.get(id);
              if (!c) {
                return null;
              }
              const row = edits[c.id];
              const watched = watchedByCategoryId.get(c.id);
              const categoryLimitIndex =
                categoryLimits?.findIndex((r) => r.categoryId === c.id) ?? -1;
              return (
                <SortableYoutubeCategoryRow
                  key={c.id}
                  form={form}
                  categoryLimitIndex={categoryLimitIndex}
                  category={c}
                  row={row}
                  index={index}
                  watched={watched}
                  isSavingOrder={isSavingOrder}
                  isPersistingCategoryLimits={isPersistingCategoryLimits}
                  onNameBlur={() => {
                    void handleNameBlurSave(c.id, c.name);
                  }}
                  onSaveMaxVideosPerDay={async (maxVideosPerDay) => {
                    return await handleSaveMaxVideosPerDay(c.id, maxVideosPerDay);
                  }}
                  onNameChange={(name) =>
                    setEdits((prev) => ({
                      ...prev,
                      [c.id]: {
                        name,
                        displayOrder: prev[c.id]?.displayOrder ?? c.displayOrder,
                      },
                    }))
                  }
                  onRequestDelete={() => {
                    const count = getSubscribedChannelCount(c);
                    if (count > 0) {
                      setDeleteTargetId(c.id);
                    } else {
                      deleteMutation.mutate({
                        id: c.id,
                        confirmUnsubscribeChannels: false,
                      });
                    }
                  }}
                />
              );
            })}
          </div>
        </SortableContext>
      </DndContext>

      <AlertDialog
        open={deleteTargetId !== null}
        onOpenChange={(open) => {
          if (!open) {
            setDeleteTargetId(null);
          }
        }}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete category?</AlertDialogTitle>
            <AlertDialogDescription>
              This will remove &quot;{deleteTargetName}&quot;. If you continue,
              every subscribed channel in this category will be unsubscribed,
              then the category will be deleted.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
              onClick={() => {
                if (deleteTargetId) {
                  deleteMutation.mutate({
                    id: deleteTargetId,
                    confirmUnsubscribeChannels: true,
                  });
                }
                setDeleteTargetId(null);
              }}
            >
              Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}

export default YoutubeCategoriesSection;

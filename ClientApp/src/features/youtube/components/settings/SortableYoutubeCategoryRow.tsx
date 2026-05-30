import { type ChangeEvent, useEffect, useState } from "react";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { GripVertical, MoreVertical, Settings2, Trash2 } from "lucide-react";
import { UseFormReturn } from "react-hook-form";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { cn } from "@/lib/utils";
import type { YoutubeCategorySettingsDto } from "@/services/openapi";

import type { YoutubeSettingsFormSchema } from "../../data/youtubeSettingsSchemas";

type RowEdit = { name: string; displayOrder: number };

interface SortableYoutubeCategoryRowProps {
  form: UseFormReturn<YoutubeSettingsFormSchema>;
  categoryLimitIndex: number;
  category: YoutubeCategorySettingsDto;
  row: RowEdit | undefined;
  index: number;
  watched: number | undefined;
  isSavingOrder: boolean;
  isPersistingCategoryLimits: boolean;
  onNameBlur: () => void;
  onSaveMaxVideosPerDay: (maxVideosPerDay: number) => Promise<boolean>;
  onNameChange: (name: string) => void;
  onRequestDelete: () => void;
}

export function SortableYoutubeCategoryRow({
  form,
  categoryLimitIndex,
  category,
  row,
  index,
  watched,
  isSavingOrder,
  isPersistingCategoryLimits,
  onNameBlur,
  onSaveMaxVideosPerDay,
  onNameChange,
  onRequestDelete,
}: SortableYoutubeCategoryRowProps) {
  const [isSettingsDialogOpen, setIsSettingsDialogOpen] = useState(false);
  const [draftMaxRaw, setDraftMaxRaw] = useState(() =>
    String(category.maxVideosPerDay),
  );
  const [draftError, setDraftError] = useState<string | null>(null);
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({
    id: category.id,
    disabled: isSavingOrder,
    animateLayoutChanges: () => false,
  });

  const style = {
    transform: transform
      ? CSS.Transform.toString({
          ...transform,
          x: 0,
          scaleX: 1,
          scaleY: 1,
        })
      : undefined,
    transition,
  };

  const showLimitField = categoryLimitIndex >= 0;
  const limitFieldDisabled =
    isSavingOrder || isPersistingCategoryLimits || !showLimitField;
  const categoryNameForTitle = row?.name.trim() || category.name;

  useEffect(() => {
    if (isSettingsDialogOpen) {
      setDraftMaxRaw(String(category.maxVideosPerDay));
      setDraftError(null);
    }
  }, [isSettingsDialogOpen, category.maxVideosPerDay, category.id]);

  const displayMax = category.maxVideosPerDay;

  return (
    <div
      ref={setNodeRef}
      style={style}
      className={cn(
        "flex flex-col gap-2 rounded-md border bg-card p-3 transition-colors @sm:flex-row @sm:items-end @sm:justify-between",
        isDragging ? "z-50 opacity-50" : "",
      )}
    >
      <div className="flex min-w-0 flex-1 flex-nowrap items-end gap-3">
        <div
          {...attributes}
          {...listeners}
          className={cn(
            "flex shrink-0 cursor-grab items-center self-center active:cursor-grabbing",
            isSavingOrder && "pointer-events-none opacity-50",
          )}
          style={{ touchAction: "none" }}
        >
          <GripVertical className="size-5 text-muted-foreground" />
          <div className="ml-1 flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10 text-sm font-medium">
            {index + 1}
          </div>
        </div>
        <div className="flex min-w-0 flex-1 flex-nowrap items-end gap-3">
          <div className="min-w-0 max-w-xs flex-1 space-y-1.5">
            <Label htmlFor={`cat-name-${category.id}`}>Name</Label>
            <Input
              id={`cat-name-${category.id}`}
              className="w-full min-w-0"
              value={row?.name ?? ""}
              onChange={(e: ChangeEvent<HTMLInputElement>) => onNameChange(e.target.value)}
              onBlur={onNameBlur}
              disabled={isSavingOrder}
            />
          </div>
        </div>
      </div>
      <div className="flex flex-wrap items-center gap-2 @sm:pl-0">
        <span className="text-xs tabular-nums text-muted-foreground">
          Max / day:{" "}
          {displayMax === 0 ? "Unlimited" : displayMax}
        </span>
        {watched !== undefined && (
          <span className="text-xs text-muted-foreground">
            Watched today: {watched}
          </span>
        )}
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              type="button"
              size="icon"
              variant="outline"
              className="size-8 shrink-0"
              disabled={isSavingOrder}
              aria-label="Category actions"
            >
              <MoreVertical className="size-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="z-100 min-w-40">
            {showLimitField ? (
              <>
                <DropdownMenuItem
                  disabled={isSavingOrder}
                  onSelect={() => {
                    // Let the dropdown fully close (focus/scroll lock) before opening
                    // the dialog — otherwise Radix can leave the page non-interactive.
                    globalThis.setTimeout(() => setIsSettingsDialogOpen(true), 0);
                  }}
                >
                  <Settings2 />
                  Edit settings
                </DropdownMenuItem>
                <DropdownMenuSeparator />
              </>
            ) : null}
            <DropdownMenuItem
              disabled={isSavingOrder}
              className="text-destructive focus:bg-destructive/10 focus:text-destructive"
              onSelect={() => {
                // Let the dropdown fully close before opening another overlay.
                globalThis.setTimeout(() => onRequestDelete(), 0);
              }}
            >
              <Trash2 />
              Delete
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
        {showLimitField ? (
          <Dialog
            open={isSettingsDialogOpen}
            onOpenChange={setIsSettingsDialogOpen}
          >
            <DialogContent
              className="sm:max-w-md"
              onCloseAutoFocus={(e) => e.preventDefault()}
            >
              <DialogHeader>
                <DialogTitle>Edit category settings</DialogTitle>
                <DialogDescription>
                  Configure limits for {categoryNameForTitle}.
                </DialogDescription>
              </DialogHeader>
              <div className="space-y-3">
                <div className="space-y-2">
                  <Label htmlFor={`cat-max-draft-${category.id}`}>Max / day</Label>
                  <Input
                    id={`cat-max-draft-${category.id}`}
                    type="number"
                    min={0}
                    className="w-full tabular-nums"
                    disabled={limitFieldDisabled}
                    value={draftMaxRaw}
                    onChange={(e: ChangeEvent<HTMLInputElement>) => {
                      setDraftMaxRaw(e.target.value);
                      setDraftError(null);
                    }}
                  />
                  {draftError ? (
                    <p className="text-sm text-destructive">{draftError}</p>
                  ) : null}
                </div>
                <div className="flex justify-end">
                  <Button
                    type="button"
                    disabled={limitFieldDisabled}
                    onClick={async () => {
                      const numeric = Number(draftMaxRaw);
                      if (
                        draftMaxRaw.trim() === "" ||
                        !Number.isFinite(numeric) ||
                        numeric < 0
                      ) {
                        setDraftError("Must be 0 or greater");
                        return;
                      }

                      const committed = category.maxVideosPerDay;

                      if (numeric === committed) {
                        setIsSettingsDialogOpen(false);
                        return;
                      }

                      const didSave = await onSaveMaxVideosPerDay(numeric);
                      if (!didSave) {
                        return;
                      }

                      if (categoryLimitIndex >= 0) {
                        form.setValue(
                          `categoryLimits.${categoryLimitIndex}.maxVideosPerDay`,
                          numeric,
                          { shouldDirty: false, shouldValidate: false },
                        );
                      }
                      setIsSettingsDialogOpen(false);
                    }}
                  >
                    {isPersistingCategoryLimits ? "Saving..." : "Save settings"}
                  </Button>
                </div>
              </div>
            </DialogContent>
          </Dialog>
        ) : null}
      </div>
    </div>
  );
}

import { useMemo, useState } from "react";
import {
  closestCenter,
  DndContext,
  DragEndEvent,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
} from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  useSortable,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { GripVertical, Plus, Trash2, X, Check } from "lucide-react";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { DialogFooter } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { MuscleGroupWorkoutIcon } from "@/features/trainings/utils/muscleGroupWorkoutIcon";
import { cn } from "@/lib/utils";
import { TrainingDto } from "@/services/openapi";

type WorkoutPlanFormValues = {
  name: string;
  isActive: boolean;
  trainingIds: string[];
};

type WorkoutPlanFormProps = {
  trainings: TrainingDto[];
  defaultValues: WorkoutPlanFormValues;
  submitButtonText: string;
  isPending: boolean;
  onCancel: () => void;
  onSubmit: (values: WorkoutPlanFormValues) => void;
};

type SortableWorkoutRowProps = {
  training: TrainingDto;
  index: number;
  isPending: boolean;
  onMoveUp: () => void;
  onMoveDown: () => void;
  onRemove: () => void;
  isFirst: boolean;
  isLast: boolean;
};

function SortableWorkoutRow({
  training,
  index,
  isPending,
  onMoveUp,
  onMoveDown,
  onRemove,
  isFirst,
  isLast,
}: SortableWorkoutRowProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({
    id: training.id,
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

  return (
    <div
      ref={setNodeRef}
      style={style}
      className={cn(
        "flex items-center gap-2 rounded-lg border border-border bg-card-secondary p-3 group",
        isDragging && "z-10 opacity-50",
      )}
    >
      <div className="flex flex-col gap-0.5">
        <button
          type="button"
          onClick={onMoveUp}
          disabled={isPending || isFirst}
          className="text-muted-foreground hover:text-foreground disabled:cursor-not-allowed disabled:opacity-30"
          aria-label="Move up"
        >
          <svg className="h-3 w-3" viewBox="0 0 10 6" fill="currentColor">
            <path d="M5 0L10 6H0L5 0Z" />
          </svg>
        </button>
        <button
          type="button"
          onClick={onMoveDown}
          disabled={isPending || isLast}
          className="text-muted-foreground hover:text-foreground disabled:cursor-not-allowed disabled:opacity-30"
          aria-label="Move down"
        >
          <svg className="h-3 w-3" viewBox="0 0 10 6" fill="currentColor">
            <path d="M5 6L0 0H10L5 6Z" />
          </svg>
        </button>
      </div>

      <div
        {...attributes}
        {...listeners}
        className="cursor-grab active:cursor-grabbing"
        style={{ touchAction: "none" }}
      >
        <GripVertical className="h-4 w-4 text-muted-foreground" />
      </div>

      <div className="flex items-center gap-2 flex-1">
        <MuscleGroupWorkoutIcon
          muscleGroups={training.muscleGroups}
          className="h-6 w-6 shrink-0 text-primary"
        />
        <div className="flex h-6 w-6 items-center justify-center rounded-full bg-primary/10 text-xs font-medium text-primary">
          {index + 1}
        </div>
        <span className="font-medium text-foreground">{training.name}</span>
      </div>

      <div className="flex items-center gap-2">
        <Button
          type="button"
          variant="ghost"
          size="icon"
          disabled={isPending}
          onClick={onRemove}
          className="h-8 w-8 text-muted-foreground opacity-0 transition-opacity hover:bg-destructive/10 hover:text-destructive group-hover:opacity-100"
          aria-label="Remove workout"
        >
          <Trash2 className="h-4 w-4" />
        </Button>
      </div>
    </div>
  );
}

function WorkoutPlanForm({
  trainings,
  defaultValues,
  submitButtonText,
  isPending,
  onCancel,
  onSubmit,
}: WorkoutPlanFormProps) {
  const [name, setName] = useState(defaultValues.name);
  const [isActive] = useState(defaultValues.isActive);
  const [trainingIds, setTrainingIds] = useState<string[]>(defaultValues.trainingIds);
  const [isAddWorkoutOpen, setIsAddWorkoutOpen] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    }),
  );

  const selectedTrainings = useMemo(
    () =>
      trainingIds
        .map((id) => trainings.find((training) => training.id === id))
        .filter((training): training is TrainingDto => Boolean(training)),
    [trainings, trainingIds],
  );

  const availableTrainings = useMemo(
    () => trainings.filter((training) => !trainingIds.includes(training.id)),
    [trainings, trainingIds],
  );

  const handleMoveUp = (index: number) => {
    if (index === 0) return;
    setTrainingIds((prev) => {
      const next = [...prev];
      [next[index - 1], next[index]] = [next[index], next[index - 1]];
      return next;
    });
  };

  const handleMoveDown = (index: number) => {
    if (index === trainingIds.length - 1) return;
    setTrainingIds((prev) => {
      const next = [...prev];
      [next[index], next[index + 1]] = [next[index + 1], next[index]];
      return next;
    });
  };

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;
    if (!over || active.id === over.id) return;

    const oldIndex = trainingIds.findIndex((id) => id === active.id);
    const newIndex = trainingIds.findIndex((id) => id === over.id);
    if (oldIndex === -1 || newIndex === -1) return;

    setTrainingIds((prev) => arrayMove(prev, oldIndex, newIndex));
  };

  return (
    <form
      className="space-y-4"
      onSubmit={(event) => {
        event.preventDefault();
        if (trainingIds.length < 2) {
          setSubmitError("Please add at least 2 workouts to the plan.");
          return;
        }

        onSubmit({
          name: name.trim(),
          isActive,
          trainingIds,
        });
      }}
    >
      <div className="space-y-2">
        <Label className="text-muted-foreground" htmlFor="workout-plan-name">
          Plan Name
        </Label>
        <Input
          id="workout-plan-name"
          value={name}
          onChange={(event) => setName(event.target.value)}
          placeholder="Push/Pull/Legs rotation"
          required
          disabled={isPending}
          className="border-input bg-background"
        />
      </div>

      <div className="space-y-2">
        <Label>Workouts in plan</Label>

        <div className="max-h-72 space-y-2 overflow-y-auto rounded-md border border-border p-2">
          {selectedTrainings.length === 0 ? (
            <div className="py-8 text-center text-muted-foreground">
              <MuscleGroupWorkoutIcon
                muscleGroups={[]}
                className="mx-auto mb-2 h-10 w-10 opacity-50"
              />
              <p>No workouts in this plan</p>
            </div>
          ) : (
            <DndContext
              sensors={sensors}
              collisionDetection={closestCenter}
              onDragEnd={handleDragEnd}
            >
              <SortableContext
                items={selectedTrainings.map((training) => training.id)}
                strategy={verticalListSortingStrategy}
              >
                <div className="space-y-2">
                  {selectedTrainings.map((training, index) => (
                    <SortableWorkoutRow
                      key={training.id}
                      training={training}
                      index={index}
                      isPending={isPending}
                      isFirst={index === 0}
                      isLast={index === selectedTrainings.length - 1}
                      onMoveUp={() => handleMoveUp(index)}
                      onMoveDown={() => handleMoveDown(index)}
                      onRemove={() => {
                        setTrainingIds((prev) => {
                          const next = prev.filter((id) => id !== training.id);
                          if (next.length >= 2 && submitError) {
                            setSubmitError(null);
                          }
                          return next;
                        });
                      }}
                    />
                  ))}
                </div>
              </SortableContext>
            </DndContext>
          )}
        </div>

        {availableTrainings.length > 0 && (
          <>
            {!isAddWorkoutOpen ? (
              <Button
                type="button"
                variant="outline"
                className="w-full border-dashed"
                disabled={isPending}
                onClick={() => setIsAddWorkoutOpen(true)}
              >
                <Plus className="h-4 w-4 mr-2" />
                Add workout to plan
              </Button>
            ) : (
              <div className="space-y-2">
                <Label className="text-muted-foreground">Add Workout</Label>

                {availableTrainings.length > 0 ? (
                  <div className="grid grid-cols-2 gap-2">
                    {availableTrainings.map((training) => (
                      <button
                        key={training.id}
                        type="button"
                        disabled={isPending}
                        onClick={() => {
                          setTrainingIds((prev) => {
                            const next = [...prev, training.id];
                            if (next.length >= 2 && submitError) {
                              setSubmitError(null);
                            }
                            return next;
                          });
                        }}
                        className="flex items-center gap-2 rounded-lg border border-border bg-muted/50 p-3 text-left transition-colors hover:border-primary/30 hover:bg-muted disabled:opacity-60"
                      >
                        <MuscleGroupWorkoutIcon
                          muscleGroups={training.muscleGroups}
                          className="h-6 w-6 text-primary"
                        />
                        <span className="text-sm text-foreground">{training.name}</span>
                      </button>
                    ))}
                  </div>
                ) : (
                  <p className="text-sm text-muted-foreground">
                    All available workouts are already in the plan
                  </p>
                )}

                <Button
                  type="button"
                  variant="ghost"
                  className="text-muted-foreground"
                  disabled={isPending}
                  onClick={() => setIsAddWorkoutOpen(false)}
                >
                  <X className="h-4 w-4 mr-2" />
                  Cancel
                </Button>
              </div>
            )}
          </>
        )}

        {submitError && (
          <p className="text-sm text-destructive">{submitError}</p>
        )}
      </div>

      <DialogFooter className="mt-6 gap-2">
        <Button
          type="button"
          onClick={onCancel}
          variant="outline"
          disabled={isPending}
        >
          Cancel
        </Button>
        <ButtonWithLoading
          type="submit"
          isLoading={isPending}
          disabled={isPending}
        >
          <Check className="h-4 w-4 mr-2" />
          {submitButtonText}
        </ButtonWithLoading>
      </DialogFooter>
    </form>
  );
}

export type { WorkoutPlanFormValues };
export default WorkoutPlanForm;

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
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { ScrollArea } from "@/components/ui/scroll-area";
import { LoadingState } from "@/hooks/useDelayedLoading";
import { ExerciseDto } from "@/services/openapi";

import { ExercisesFormStep } from "../ExercisesFormList";
import { SortableExerciseCard } from "./SortableExerciseCard";

const OrderExercisesFormStep = ({
  selectedExercises,
  setStep,
  setFormExercises,
  removeExerciseFromForm,
  onCancel,
  isPending,
}: {
  selectedExercises: ExerciseDto[];
  setStep: (step: ExercisesFormStep) => void;
  setFormExercises: (exercises: ExerciseDto[]) => void;
  removeExerciseFromForm: (exerciseId: string) => void;
  onCancel: () => void;
  isPending: LoadingState;
}) => {
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    }),
  );

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;

    if (active.id !== over?.id) {
      const oldIndex = selectedExercises.findIndex((e) => e.id === active.id);
      const newIndex = selectedExercises.findIndex((e) => e.id === over?.id);

      const newOrder = arrayMove(selectedExercises, oldIndex, newIndex);
      setFormExercises(newOrder);
    }
  };

  return (
    <>
      <div className="space-y-4">
        <OrderExercisesFormStepHeader />
        <ScrollArea className="h-[400px] rounded-md border bg-card">
          <div className="overflow-hidden p-2">
            <DndContext
              sensors={sensors}
              collisionDetection={closestCenter}
              onDragEnd={handleDragEnd}
            >
              <SortableContext
                items={selectedExercises.map((e) => e.id)}
                strategy={verticalListSortingStrategy}
              >
                {selectedExercises.map((exercise, index) => (
                  <SortableExerciseCard
                    key={exercise.id}
                    exercise={exercise}
                    index={index}
                    onRemove={() => removeExerciseFromForm(exercise.id)}
                  />
                ))}
              </SortableContext>
            </DndContext>
          </div>
        </ScrollArea>
      </div>
      <OrderExercisesFormStepFooter
        onCancel={onCancel}
        setStep={setStep}
        isPending={isPending}
      />
    </>
  );
};

function OrderExercisesFormStepHeader() {
  return (
    <>
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold">Step 2: Order Exercises</h2>
      </div>
      <p className="text-sm text-muted-foreground">
        Drag and drop exercises to set the order they'll appear in your workout.
      </p>
    </>
  );
}

function OrderExercisesFormStepFooter({
  onCancel,
  setStep,
  isPending,
}: {
  onCancel: () => void;
  setStep: (step: ExercisesFormStep) => void;
  isPending: LoadingState;
}) {
  return (
    <div className="flex justify-end gap-2 pt-4">
      <Button variant="outline" type="button" onClick={onCancel}>
        Cancel
      </Button>
      <Button
        variant="secondary"
        onClick={() => setStep("select")}
        type="button"
      >
        Back to Selection
      </Button>
      <ButtonWithLoading
        type="submit"
        disabled={!isPending.isLoaded}
        isLoading={isPending.isLoading}
        className="min-w-[100px]"
      >
        Save Changes
      </ButtonWithLoading>
    </div>
  );
}

export default OrderExercisesFormStep;

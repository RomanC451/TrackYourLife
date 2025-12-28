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
import { useFormContext } from "react-hook-form";

import { ScrollArea } from "@/components/ui/scroll-area";
import { MutationPendingState } from "@/hooks/useCustomMutation";

import { TrainingFormSchema } from "../../../data/trainingsSchemas";
import { ExercisesFormStep } from "../ExercisesFormList";
import OrderExercisesFormStepFooter from "./OrderExercisesFormStepFooter";
import OrderExercisesFormStepHeader from "./OrderExercisesFormStepHeader";
import { SortableExerciseCard } from "./SortableExerciseCard";

const OrderExercisesFormStep = ({
  setStep,
  onCancel,
  pendingState,
  submitButtonText,
}: {
  setStep: (step: ExercisesFormStep) => void;
  onCancel: () => void;
  pendingState: MutationPendingState;
  submitButtonText: string;
}) => {
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    }),
  );

  const { getValues, setValue } = useFormContext<TrainingFormSchema>();

  const removeExerciseFromForm = (exerciseId: string) => {
    const currentExercises = getValues("exercises") || [];
    setValue(
      "exercises",
      currentExercises.filter((e) => e.id !== exerciseId),
    );
  };

  const selectedExercises = getValues("exercises").map((exercise) => ({
    ...exercise,
    isLoading: false,
    isDeleting: false,
    createdOnUtc: "",
  }));

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;

    if (active.id !== over?.id) {
      const oldIndex = selectedExercises.findIndex((e) => e.id === active.id);
      const newIndex = selectedExercises.findIndex((e) => e.id === over?.id);

      const newOrder = arrayMove(selectedExercises, oldIndex, newIndex);
      setValue("exercises", newOrder);
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
                items={selectedExercises}
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
        pendingState={pendingState}
        submitButtonText={submitButtonText}
      />
    </>
  );
};

export default OrderExercisesFormStep;

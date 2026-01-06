import { useEffect, useState } from "react";
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
import { ExerciseDto } from "@/services/openapi";

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

  const form = useFormContext<TrainingFormSchema>();
  const { getValues, setValue } = form;

  const [selectedExercises, setSelectedExercises] = useState<ExerciseDto[]>(
    () => {
      const exercises = getValues("exercises") || [];
      return exercises.map((exercise) => ({
        ...exercise,
        isLoading: false,
        isDeleting: false,
        createdOnUtc: "",
      }));
    },
  );

  useEffect(() => {
    const subscription = form.watch((values, { name }) => {
      if (name !== "exercises") {
        return;
      }

      const exercises = (values.exercises as ExerciseDto[]) || [];
      setSelectedExercises(
        exercises.map((exercise) => ({
          ...exercise,
          isLoading: false,
          isDeleting: false,
          createdOnUtc: "",
        })),
      );
    });

    return () => subscription.unsubscribe();
  }, [form]);

  const removeExerciseFromForm = (exerciseId: string) => {
    const currentExercises = getValues("exercises") || [];
    setValue(
      "exercises",
      currentExercises.filter((e) => e.id !== exerciseId),
      { shouldDirty: true },
    );
  };

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;

    if (over && active.id !== over.id) {
      const currentExercises = getValues("exercises") || [];
      const oldIndex = currentExercises.findIndex((e) => e.id === active.id);
      const newIndex = currentExercises.findIndex((e) => e.id === over.id);

      if (oldIndex !== -1 && newIndex !== -1) {
        const newOrder = arrayMove(currentExercises, oldIndex, newIndex);
        setValue("exercises", newOrder, { shouldDirty: true });
      }
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
        pendingState={pendingState}
        submitButtonText={submitButtonText}
      />
    </>
  );
};

export default OrderExercisesFormStep;

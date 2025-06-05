import { useState } from "react";
import { CheckCircle2, Plus, Search } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { ScrollArea } from "@/components/ui/scroll-area";
import { useTrainingsDialogsContext } from "@/features/trainings/common/contexts/TrainingsDialogsContextProvider";
import { ExerciseMenu } from "@/features/trainings/exercises/components/common/ExerciseMenu";
import useExercisesQuery from "@/features/trainings/exercises/queries/useExercisesQuery";
import { cn } from "@/lib/utils";
import { ExerciseDto } from "@/services/openapi";

import { ExercisesFormStep } from "../ExercisesFormList";

function SelectExercisesFormStep({
  selectedExercises,
  removeExerciseFromForm,
  updateExerciseInForm,
  setFormExercises,
  getFormExercises,
  onCancel,
  setStep,
}: {
  selectedExercises: ExerciseDto[];
  removeExerciseFromForm: (exerciseId: string) => void;
  updateExerciseInForm: (exercise: Partial<ExerciseDto>) => void;
  setFormExercises: (exercises: ExerciseDto[]) => void;
  getFormExercises: () => ExerciseDto[];
  onCancel: () => void;
  setStep: (step: ExercisesFormStep) => void;
}) {
  const { exercisesQuery } = useExercisesQuery();
  const [searchQuery, setSearchQuery] = useState("");

  const { setCreateExerciseDialogOpen } = useTrainingsDialogsContext();

  const [openMenuId, setOpenMenuId] = useState<string | null>(null);

  const filteredExercises = exercisesQuery.data?.filter((exercise) =>
    exercise.name.toLowerCase().includes(searchQuery.toLowerCase()),
  );

  const toggleExercise = (exercise: ExerciseDto) => {
    if (exercise.isDeleting || exercise.isLoading) {
      return;
    }

    const currentExercises = getFormExercises() || [];
    const isSelected = currentExercises.some((e) => e.id === exercise.id);

    if (isSelected) {
      setFormExercises(currentExercises.filter((e) => e.id !== exercise.id));
    } else {
      setFormExercises([
        ...currentExercises,
        {
          id: exercise.id,
          name: exercise.name,
          description: exercise.description,
          equipment: exercise.equipment,
          videoUrl: exercise.videoUrl || undefined,
          exerciseSets: exercise.exerciseSets.map((set) => ({
            name: set.name,
            reps: set.reps,
            weight: set.weight,
          })),
          createdOnUtc: exercise.createdOnUtc,
          isLoading: true,
          isDeleting: false,
        },
      ]);
    }
  };

  const handleMenuOpen = (exerciseId: string) => {
    setOpenMenuId(exerciseId);
  };

  const handleMenuClose = () => {
    setOpenMenuId(null);
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold">Step 1: Select Exercises</h2>
      </div>

      <div className="relative">
        <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
        <Input
          placeholder="Search exercises..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="pl-8"
        />
      </div>

      <div className="relative">
        <ScrollArea
          className="h-[400px] rounded-md border bg-card"
          onScroll={handleMenuClose}
        >
          <div className="grid grid-cols-1 gap-2 p-2">
            {filteredExercises?.map((exercise) => {
              const isSelected = selectedExercises.some(
                (e) => e.id === exercise.id,
              );

              return (
                <Card
                  key={exercise.id}
                  className={cn(
                    "cursor-pointer border bg-card transition-all duration-200",
                    exercise.isLoading
                      ? "cursor-not-allowed border-accent bg-accent/40"
                      : exercise.isDeleting
                        ? "cursor-not-allowed border-accent bg-red-900/40"
                        : isSelected
                          ? "border-primary/50 shadow-sm"
                          : "border-accent hover:border-primary/20",
                  )}
                  onClick={() => toggleExercise(exercise)}
                >
                  <CardContent className="p-4">
                    <div className="flex-1">
                      <div className="flex items-center gap-2">
                        <div className="flex-1">
                          <div className="flex items-center justify-between gap-2">
                            <div className="flex w-full justify-between gap-4">
                              <div className="flex w-full items-center justify-between gap-2">
                                <div>
                                  <h3 className="font-semibold">
                                    {exercise.name}
                                  </h3>

                                  <Badge variant="secondary">
                                    {exercise.equipment
                                      ? exercise.equipment
                                      : "No equipment"}
                                  </Badge>
                                </div>
                                {isSelected && (
                                  <CheckCircle2 className="size-4 flex-shrink-0" />
                                )}
                              </div>
                            </div>
                            <ExerciseMenu
                              exercise={exercise}
                              onSuccessDelete={() => {
                                removeExerciseFromForm(exercise.id);
                              }}
                              onSuccessEdit={(exercise) => {
                                updateExerciseInForm(exercise);
                              }}
                              isOpen={openMenuId === exercise.id}
                              onRequestClose={handleMenuClose}
                              onOpenMenu={() => handleMenuOpen(exercise.id)}
                            />
                          </div>
                        </div>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              );
            })}
            <div className="h-[50px] w-[442px]" />
          </div>
        </ScrollArea>

        <Button
          size="icon"
          className="absolute bottom-4 right-4 rounded-full shadow-md"
          onClick={() => {
            setCreateExerciseDialogOpen(true);
          }}
          type="button"
        >
          <Plus className="h-4 w-4" />
        </Button>
      </div>
      <SelectExercisesFormStepFooter
        selectedExercises={selectedExercises}
        onCancel={onCancel}
        setStep={setStep}
      />
    </div>
  );
}

function SelectExercisesFormStepFooter({
  selectedExercises,
  onCancel,
  setStep,
}: {
  selectedExercises: ExerciseDto[];
  onCancel: () => void;
  setStep: (step: ExercisesFormStep) => void;
}) {
  return (
    <div className="flex items-center justify-between">
      <p className="text-sm text-muted-foreground">
        {selectedExercises.length} exercises selected
      </p>
      <div className="flex gap-2">
        <Button variant="outline" type="button" onClick={onCancel}>
          Cancel
        </Button>
        <Button
          onClick={() => setStep("order")}
          disabled={selectedExercises.length === 0}
          type="button"
        >
          Next: Order Exercises
        </Button>
      </div>
    </div>
  );
}

export default SelectExercisesFormStep;

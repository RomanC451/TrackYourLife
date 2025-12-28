import { useState } from "react";
import { Link, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";

import { router } from "@/App";
import { Button } from "@/components/ui/button";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Skeleton } from "@/components/ui/skeleton";
import { ExerciseFormSchema } from "@/features/trainings/exercises/data/exercisesSchemas";
import { cn } from "@/lib/utils";
import { ExerciseDto } from "@/services/openapi";

import SelectExercisesListElement from "./SelectExercisesListElement";

function SelectExercisesList({
  exercises,
  selectedExercises,
  onSelect,
}: {
  exercises: ExerciseDto[];
  selectedExercises: ExerciseFormSchema[];
  onSelect: (exercise: ExerciseDto) => void;
}) {
  const [openMenuId, setOpenMenuId] = useState<string | null>(null);

  const navigate = useNavigate();

  const handleMenuOpen = (exerciseId: string) => setOpenMenuId(exerciseId);

  const handleMenuClose = () => setOpenMenuId(null);

  return (
    <div className="relative">
      <ScrollArea
        className={cn(
          "h-[400px] rounded-md border p-1",
          exercises.length > 3 && "pr-2",
        )}
        type={"always"}
        onScroll={handleMenuClose}
      >
        <div className="grid grid-cols-1 gap-2 p-2">
          {exercises?.map((exercise) => {
            const isSelected = selectedExercises.some(
              (e) => e.id === exercise.id,
            );

            return (
              <SelectExercisesListElement
                key={exercise.id}
                isMenuOpen={openMenuId === exercise.id}
                exercise={exercise}
                isSelected={isSelected}
                onSelect={onSelect}
                onOpenMenu={handleMenuOpen}
                onCloseMenu={handleMenuClose}
              />
            );
          })}
          <div className="h-[50px] w-[442px]" />
        </div>
      </ScrollArea>

      <Button
        size="icon"
        className="absolute bottom-4 right-4 rounded-full shadow-md"
        type="button"
        onClick={() => {
          navigate({ to: "/trainings/workouts/exercises/create" });
        }}
        onMouseEnter={() => {
          router.preloadRoute({
            to: "/trainings/workouts/exercises/create",
          });
        }}
        onTouchStart={() => {
          router.preloadRoute({
            to: "/trainings/workouts/exercises/create",
          });
        }}
      >
        <Plus className="h-4 w-4" />
      </Button>
    </div>
  );
}

SelectExercisesList.Loading = function Loading() {
  return (
    <div className="relative">
      <ScrollArea className="h-[400px] rounded-md border bg-card">
        <div className="space-y-2 p-2">
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
        </div>
      </ScrollArea>

      <Link to="/trainings/workouts/exercises/create">
        <Button
          size="icon"
          className="absolute bottom-4 right-4 rounded-full shadow-md"
          type="button"
        >
          <Plus className="h-4 w-4" />
        </Button>
      </Link>
    </div>
  );
};

SelectExercisesList.Empty = function Empty() {
  return (
    <div className="relative">
      <div className="h-[400px] rounded-md border bg-card" />

      <Link to="/trainings/workouts/exercises/create">
        <Button
          size="icon"
          className="absolute bottom-4 right-4 rounded-full shadow-md"
          type="button"
        >
          <Plus className="h-4 w-4" />
        </Button>
      </Link>
    </div>
  );
};

export default SelectExercisesList;

import { CheckCircle2 } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import ExerciseMenu from "@/features/trainings/exercises/components/common/ExerciseMenu";
import { cn } from "@/lib/utils";
import { ExerciseDto } from "@/services/openapi";

function SelectExercisesListElement({
  exercise,
  isSelected,
  isMenuOpen,
  onOpenMenu,
  onCloseMenu,
  onSelect,
}: {
  exercise: ExerciseDto;
  isSelected: boolean;
  isMenuOpen: boolean;
  onOpenMenu: (exerciseId: string) => void;
  onCloseMenu: () => void;
  onSelect: (exercise: ExerciseDto) => void;
}) {
  return (
    <Card
      key={exercise.id}
      className={cn(
        "bg-card-secondary cursor-pointer border transition-all duration-200",
        exercise.isLoading
          ? "cursor-not-allowed border-accent bg-accent/40"
          : exercise.isDeleting
            ? "cursor-not-allowed border-accent opacity-50"
            : isSelected
              ? "border-primary/50 shadow-sm"
              : "border-accent hover:border-primary/20",
      )}
      onClick={() => onSelect(exercise)}
    >
      <CardContent className="p-4">
        <div className="flex-1">
          <div className="flex items-center gap-2">
            <div className="flex-1">
              <div className="flex items-center justify-between gap-2">
                <div className="flex w-full justify-between gap-4">
                  <div className="flex w-full items-center justify-between gap-2">
                    <div>
                      <h3 className="font-semibold">{exercise.name}</h3>

                      <Badge variant="secondary">
                        {exercise.equipment
                          ? exercise.equipment
                          : "No equipment"}
                      </Badge>
                    </div>
                    {isSelected && (
                      <CheckCircle2 className="size-4 shrink-0" />
                    )}
                  </div>
                </div>
                <ExerciseMenu
                  exercise={exercise}
                  defaultOpen={isMenuOpen}
                  onOpen={() => onOpenMenu(exercise.id)}
                  onClose={onCloseMenu}
                />
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

export default SelectExercisesListElement;

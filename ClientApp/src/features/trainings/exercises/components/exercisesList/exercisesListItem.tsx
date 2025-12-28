import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { Dumbbell, Target, Volleyball } from "lucide-react";

import { router } from "@/App";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import { ExerciseDto } from "@/services/openapi";

import useDeleteExerciseMutation from "../../mutations/useDeleteExerciseMutation";
import { getDifficultyColor } from "../../utils/exercisesUtils";
import ForceDeleteExerciseAlertDialog from "../common/ForceDeleteExerciseAlertDialog";

function ExercisesListItem({ exercise }: { exercise: ExerciseDto }) {
  const navigate = useNavigate();

  const [showForceDeleteAlert, setShowForceDeleteAlert] = useState(false);

  const deleteExerciseMutation = useDeleteExerciseMutation();

  const onDelete = () => {
    deleteExerciseMutation.mutate({
      id: exercise.id,
      forceDelete: false,
      name: exercise.name,
      onShowAlert: () => {
        setShowForceDeleteAlert(true);
      },
    });
  };

  return (
    <Card
      key={exercise.id}
      className={cn("", {
        "opacity-50": exercise.isDeleting,
      })}
    >
      <CardHeader className="pb-2">
        <div className="flex items-start justify-between">
          <div className="flex items-center space-x-3">
            <div className="rounded-lg bg-orange-600/20 p-2">
              {exercise.equipment === "Barbell" ? (
                <Dumbbell className="h-4 w-4 text-orange-400" />
              ) : (
                // TODO: Add other equipment icons
                <Volleyball className="h-4 w-4 text-orange-400" />
              )}
            </div>
            <div className="min-w-0 flex-1">
              <h3 className="truncate text-lg font-semibold">
                {exercise.name}
              </h3>
              <div className="mt-1 flex items-center space-x-2">
                <span className="text-xs">
                  {exercise.exerciseSets.length} sets
                </span>
              </div>
            </div>
          </div>
          <Badge className={cn(getDifficultyColor(exercise.difficulty), "")}>
            {exercise.difficulty}
          </Badge>
        </div>
      </CardHeader>

      <CardContent className="space-y-3 pt-0">
        {/* Exercise Info */}
        <div className="space-y-2">
          <div className="flex items-center text-xs text-primary">
            <Target className="mr-2 h-3 w-3" />
            <span className="truncate">{exercise.muscleGroups.join(", ")}</span>
          </div>
          <div className="line-clamp-2 text-xs text-muted-foreground">
            {exercise.description}
          </div>
        </div>

        {/* Sets Preview */}
        <div className="rounded-lg bg-card-secondary p-3">
          <div className="mb-2 flex items-center justify-between">
            <span className="text-xs font-medium text-muted-foreground">
              Sets Overview
            </span>
            <span className="text-xs text-muted-foreground">
              {exercise.equipment}
            </span>
          </div>
          <div className="grid grid-cols-3 gap-2 text-xs">
            {exercise.exerciseSets.slice(0, 3).map((set) => (
              <div
                key={set.id}
                className="rounded border border-primary p-1 text-center"
              >
                <div className="font-medium text-muted-foreground">
                  {set.count1} {set.unit1}
                </div>
                {set.count2 && (
                  <div className="text-xs text-slate-400">
                    {set.count2} {set.unit2}
                  </div>
                )}
              </div>
            ))}
          </div>
          {exercise.exerciseSets.length > 3 && (
            <div className="mt-2 text-center">
              <span className="text-xs text-slate-500">
                +{exercise.exerciseSets.length - 3} more sets
              </span>
            </div>
          )}
        </div>

        <div className="flex space-x-2">
          <Button
            variant="outline"
            size="sm"
            disabled={exercise.isDeleting}
            onClick={() => {
              navigate({
                to: "/trainings/exercises/edit/$exerciseId",
                params: {
                  exerciseId: exercise.id,
                },
              });
            }}
            onMouseEnter={() => {
              router.preloadRoute({
                to: "/trainings/exercises/edit/$exerciseId",
                params: {
                  exerciseId: exercise.id,
                },
              });
            }}
            onTouchStart={() => {
              router.preloadRoute({
                to: "/trainings/exercises/edit/$exerciseId",
                params: {
                  exerciseId: exercise.id,
                },
              });
            }}
            className="flex-1"
          >
            Edit
          </Button>
          <Button
            variant="destructive"
            size="sm"
            disabled={exercise.isDeleting}
            onClick={onDelete}
          >
            Delete
          </Button>
        </div>
      </CardContent>
      {showForceDeleteAlert && (
        <ForceDeleteExerciseAlertDialog
          id={exercise.id}
          name={exercise.name}
          onSuccess={() => {
            setShowForceDeleteAlert(false);
          }}
          onCancel={() => {
            setShowForceDeleteAlert(false);
          }}
        />
      )}
    </Card>
  );
}

export default ExercisesListItem;

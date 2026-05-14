import { useMemo, useState } from "react";
import { Link } from "@tanstack/react-router";
import {
  BarChart3,
  Cable,
  Dumbbell,
  Flame,
  MoreVertical,
  Pencil,
  Trash2,
} from "lucide-react";

import { router } from "@/App";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Separator } from "@/components/ui/separator";
import Spinner from "@/components/ui/spinner";
import { cn } from "@/lib/utils";
import { type ExerciseDto } from "@/services/openapi";

import useDeleteExerciseMutation from "../../mutations/useDeleteExerciseMutation";
import { getDifficultyColor } from "../../utils/exercisesUtils";
import ForceDeleteExerciseAlertDialog from "../common/ForceDeleteExerciseAlertDialog";

function equipmentIconComponent(equipment?: string) {
  const e = equipment?.toLowerCase() ?? "";
  if (e.includes("cable")) return Cable;
  if (e.includes("barbell")) return Dumbbell;
  return Dumbbell;
}

function formatCount(count: number, unit: string) {
  if (count < 0) return "—";
  return `${count} ${unit}`;
}

function ExercisesListItem({ exercise }: { exercise: ExerciseDto }) {
  const [showForceDeleteAlert, setShowForceDeleteAlert] = useState(false);
  const deleteExerciseMutation = useDeleteExerciseMutation();

  const EquipmentIcon = useMemo(
    () => equipmentIconComponent(exercise.equipment),
    [exercise.equipment],
  );

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

  const menuDisabled =
    exercise.isDeleting || exercise.isLoading;

  const triggerSpinnerColor = exercise.isDeleting
    ? "fill-destructive"
    : "fill-primary";

  const previewSets = exercise.exerciseSets.slice(0, 3);
  const extraSetsCount = exercise.exerciseSets.length - previewSets.length;

  return (
    <Card
      className={cn(
        "relative overflow-hidden border-t-2 border-t-primary shadow-lg",
        {
          "opacity-50": exercise.isDeleting,
        },
      )}
    >
      <CardHeader className="relative space-y-4 pb-2 pr-12 pt-6">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              type="button"
              variant="ghost"
              size="icon"
              disabled={menuDisabled}
              className="absolute right-4 top-4"
              aria-label="Exercise menu"
            >
              {exercise.isDeleting || exercise.isLoading ? (
                <Spinner className="size-5" color={triggerSpinnerColor} />
              ) : (
                <MoreVertical className="h-5 w-5" />
              )}
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-40">
            <DropdownMenuItem asChild disabled={menuDisabled}>
              <Link
                to="/trainings/exercises/edit/$exerciseId"
                params={{ exerciseId: exercise.id }}
                preload="intent"
                onMouseEnter={() => {
                  router.preloadRoute({
                    to: "/trainings/exercises/edit/$exerciseId",
                    params: { exerciseId: exercise.id },
                  });
                }}
                onTouchStart={() => {
                  router.preloadRoute({
                    to: "/trainings/exercises/edit/$exerciseId",
                    params: { exerciseId: exercise.id },
                  });
                }}
              >
                <Pencil className="h-4 w-4" />
                Edit
              </Link>
            </DropdownMenuItem>
            <DropdownMenuItem asChild disabled={menuDisabled}>
              <Link
                to="/trainings/exercises/$exerciseId/stats"
                params={{ exerciseId: exercise.id }}
                search={{ range: "TwelveWeeks" }}
                preload="intent"
                onMouseEnter={() => {
                  router.preloadRoute({
                    to: "/trainings/exercises/$exerciseId/stats",
                    params: { exerciseId: exercise.id },
                    search: { range: "TwelveWeeks" },
                  });
                }}
                onTouchStart={() => {
                  router.preloadRoute({
                    to: "/trainings/exercises/$exerciseId/stats",
                    params: { exerciseId: exercise.id },
                    search: { range: "TwelveWeeks" },
                  });
                }}
              >
                <BarChart3 className="h-4 w-4" />
                Stats
              </Link>
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem
              disabled={menuDisabled}
              className="text-destructive focus:bg-destructive/10 focus:text-destructive"
              onSelect={() => {
                onDelete();
              }}
            >
              <Trash2 className="h-4 w-4" />
              Delete
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>

        <div className="flex items-start gap-3">
          <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl border border-primary/20 bg-primary/10">
            <EquipmentIcon className="h-5 w-5 text-primary" />
          </div>
          <div className="min-w-0 flex-1 space-y-1">
            <CardTitle className="text-lg leading-tight">
              {exercise.name}
            </CardTitle>
            <CardDescription className="flex items-center gap-2">
              <Dumbbell className="h-3.5 w-3.5 shrink-0" />
              <span className="truncate">
                {exercise.muscleGroups.join(" · ")}
              </span>
            </CardDescription>
          </div>
        </div>

        <div className="flex flex-wrap items-center gap-2 text-sm text-muted-foreground sm:gap-3">
          <div className="flex items-center gap-1.5">
            <Flame className="h-4 w-4 text-primary" />
            <span>{exercise.exerciseSets.length} sets</span>
          </div>
          <Separator
            orientation="vertical"
            className="hidden h-4 sm:block"
          />
          <span>
            {exercise.equipment?.trim() ? exercise.equipment : "—"}
          </span>
          <Separator
            orientation="vertical"
            className="hidden h-4 sm:block"
          />
          <Badge
            variant="outline"
            className={cn(
              getDifficultyColor(exercise.difficulty),
              "font-normal",
            )}
          >
            {exercise.difficulty}
          </Badge>
        </div>
      </CardHeader>

      <CardContent className="pb-6">
        <Card className="rounded-lg border border-border bg-card-secondary shadow-none">
          <CardHeader className="space-y-0 p-4 pb-2">
            <span className="text-xs font-medium uppercase tracking-wider text-primary">
              Sets Overview
            </span>
          </CardHeader>
          <CardContent className="p-4 pt-0">
            {previewSets.length === 0 ? (
              <p className="text-center text-sm text-muted-foreground">
                No sets yet
              </p>
            ) : (
              <>
                <div className="grid grid-cols-3 gap-2">
                  {previewSets.map((set, index) => (
                    <Card
                      key={`${exercise.id}-${set.id}-${index}`}
                      className="group relative rounded-lg border border-border bg-muted/40 text-center shadow-none transition-colors hover:border-primary/40"
                    >
                      <span className="absolute left-2 top-1.5 text-[10px] font-semibold text-muted-foreground">
                        {index + 1}
                      </span>
                      <CardContent className="space-y-0.5 p-0 px-2 py-3">
                        <p className="text-sm font-semibold leading-tight text-foreground">
                          {formatCount(set.count1, set.unit1)}
                        </p>
                        {set.count2 !== undefined && (
                          <p className="text-xs text-muted-foreground">
                            {formatCount(set.count2, set.unit2 ?? "")}
                          </p>
                        )}
                      </CardContent>
                    </Card>
                  ))}
                </div>
                {extraSetsCount > 0 && (
                  <p className="mt-2 text-center text-xs text-muted-foreground">
                    +{extraSetsCount} more sets
                  </p>
                )}
              </>
            )}
          </CardContent>
        </Card>
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

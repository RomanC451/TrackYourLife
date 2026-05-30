import { Link } from "@tanstack/react-router";
import { Play } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { MuscleGroupWorkoutIcon } from "@/features/trainings/utils/muscleGroupWorkoutIcon";
import type { OngoingTrainingDto } from "@/services/openapi";

type HomeResumeWorkoutCardProps = {
  ongoing: OngoingTrainingDto;
};

function HomeResumeWorkoutCard({ ongoing }: HomeResumeWorkoutCardProps) {
  const trainingName = ongoing.training?.name ?? "Workout";
  const exerciseCount = ongoing.training?.exercises?.length ?? 0;
  const currentExerciseNumber = exerciseCount > 0 ? ongoing.exerciseIndex + 1 : 0;
  const muscleGroups = ongoing.training?.exercises?.[ongoing.exerciseIndex]?.muscleGroups;

  return (
    <Card className="overflow-hidden p-0">
      <div className="flex items-center justify-between rounded-xl border border-primary/20 bg-primary/10 p-4 m-4">
        <div className="flex items-center gap-3">
          <div className="rounded-lg bg-primary/20 p-2">
            <MuscleGroupWorkoutIcon
              muscleGroups={muscleGroups ?? ongoing.training?.muscleGroups}
              className="h-7 w-7 text-primary"
            />
          </div>
          <div>
            <span className="text-xs font-medium tracking-wide text-primary uppercase">
              In progress
            </span>
            <p className="font-medium text-foreground">{trainingName}</p>
            {exerciseCount > 0 ? (
              <p className="text-sm text-muted-foreground">
                Exercise {currentExerciseNumber} of {exerciseCount}
              </p>
            ) : null}
          </div>
        </div>

        <Button asChild className="gap-2">
          <Link to="/trainings/ongoing-workout">
            <Play className="h-4 w-4" />
            Continue
          </Link>
        </Button>
      </div>
    </Card>
  );
}

export default HomeResumeWorkoutCard;

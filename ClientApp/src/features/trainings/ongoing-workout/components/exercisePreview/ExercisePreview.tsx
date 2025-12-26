import { useSuspenseQuery } from "@tanstack/react-query";

import { ImageWithSpinner } from "@/components/image-with-spinner";
import { Card } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import VideoPlayerWithLoading from "@/components/video-player-with-loading";
import WorkoutTimer from "@/features/trainings/common/components/workoutTimer/WorkoutTimer";
import { apiExerciseSetToExerciseSetSchema } from "@/features/trainings/exercises/utils/exerciseSetsMappings";

import { ongoingTrainingsQueryOptions } from "../../queries/ongoingTrainingsQuery";
import CurrentSet from "./CurrentSet";
import ExerciseDescriptionCollapsible from "./ExerciseDescriptionCollapsible";
import ExercisePreviewFooter from "./ExercisePreviewFooter";

function ExercisePreview() {
  const ongoingTrainingQuery = useSuspenseQuery(
    ongoingTrainingsQueryOptions.active,
  );

  const exercise =
    ongoingTrainingQuery.data.training.exercises[
      ongoingTrainingQuery.data.exerciseIndex
    ];
  const currentSet = apiExerciseSetToExerciseSetSchema(
    exercise.exerciseSets[ongoingTrainingQuery.data.setIndex],
  );

  return (
    <div className="mx-auto flex w-full flex-col gap-4 rounded-xl">
      <WorkoutTimer />

      <Card className="space-y-4 p-4">
        {/* Header */}
        <div className="flex flex-col gap-1">
          <h2 className="text-2xl font-bold">{exercise.name}</h2>
          <span className="text-sm text-muted-foreground">
            Equipment: {exercise.equipment}
          </span>
        </div>

        {/* Image Placeholder */}
        {exercise.pictureUrl && (
          <div className="flex h-48 items-center justify-center rounded-lg bg-gray-800">
            <ImageWithSpinner
              src={exercise.pictureUrl ?? ""}
              alt={exercise.name}
              className="h-full w-full rounded-lg object-cover"
            />
          </div>
        )}

        <Separator className="h-[1px] w-full" />
        <ExerciseDescriptionCollapsible exercise={exercise} />
      </Card>

      <CurrentSet
        currentSet={currentSet}
        index={ongoingTrainingQuery.data.setIndex}
      />

      {/* Video Player */}

      <div className="overflow-hidden rounded-lg bg-black">
        <VideoPlayerWithLoading url={exercise.videoUrl ?? ""} />
      </div>

      {/* Navigation Buttons */}
      <ExercisePreviewFooter ongoingTraining={ongoingTrainingQuery.data} />
    </div>
  );
}

export default ExercisePreview;

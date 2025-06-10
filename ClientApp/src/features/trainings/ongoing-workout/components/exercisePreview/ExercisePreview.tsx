import { useEffect, useState } from "react";
import { useToggle } from "usehooks-ts";

import handleQuery from "@/components/handle-query";
import { ImageWithSpinner } from "@/components/image-with-spinner";
import { Separator } from "@/components/ui/separator";
import VideoPlayerWithLoading from "@/components/video-player-with-loading";
import WorkoutTimerPage from "@/features/trainings/ongoing-workout/components/exercisePreview/WorkoutTimer";

import { useShowWorkoutTimer } from "../../hooks/useShowWorkoutTimer";
import useActiveOngoingTrainingQuery from "../../queries/useActiveOngoingTrainingQuery";
import CurrentSet from "./CurrentSet";
import ExerciseDescriptionCollapsible from "./ExerciseDescriptionCollapsible";
import ExercisePreviewFooter from "./ExercisePreviewFooter";

function ExercisePreview() {
  const { activeOngoingTrainingQuery: ongoingTrainingQuery } =
    useActiveOngoingTrainingQuery();

  const [timerTrigger, startTimer] = useToggle();
  const [isResting, setIsResting] = useState(false);

  const { shouldShowTimer, refreshShowWorkoutTimer } = useShowWorkoutTimer();

  useEffect(() => {
    refreshShowWorkoutTimer();
    startTimer();
  }, [ongoingTrainingQuery.data, startTimer, refreshShowWorkoutTimer]);

  return handleQuery(ongoingTrainingQuery, (data) => {
    const exercise = data.training.exercises[data.exerciseIndex];
    const currentSet = exercise.exerciseSets[data.setIndex];

    return (
      <div className="mx-auto flex w-full flex-col gap-4 rounded-xl">
        <WorkoutTimerPage
          timerTrigger={timerTrigger}
          timerSeconds={data.training.restSeconds}
          hidden={(data.isFirstSet && data.isFirstExercise) || !shouldShowTimer}
          setIsResting={setIsResting}
        />

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

        <Separator className="h-[1px] w-full" />
        <CurrentSet currentSet={currentSet} index={data.setIndex} />

        {/* Video Player */}
        <Separator className="h-[1px] w-full" />
        <div className="overflow-hidden rounded-lg bg-black">
          <VideoPlayerWithLoading url={exercise.videoUrl ?? ""} />
        </div>

        {/* Navigation Buttons */}
        <ExercisePreviewFooter ongoingTraining={data} isResting={isResting} />
      </div>
    );
  });
}

export default ExercisePreview;

import { useState } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";

import { ImageWithSpinner } from "@/components/image-with-spinner";
import { Card } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import VideoPlayerWithLoading from "@/components/video-player-with-loading";
import WorkoutTimer from "@/features/trainings/common/components/workoutTimer/WorkoutTimer";

import { ongoingTrainingsQueryOptions } from "../../queries/ongoingTrainingsQuery";
import CurrentSet from "./CurrentSet";
import ExerciseDescriptionCollapsible from "./ExerciseDescriptionCollapsible";
import ExercisePreviewFooter from "./ExercisePreviewFooter";

function ExercisePreview() {
  const ongoingTrainingQuery = useSuspenseQuery(
    ongoingTrainingsQueryOptions.active,
  );

  const ongoingTraining = ongoingTrainingQuery.data;
  const exercise =
    ongoingTraining.training.exercises[ongoingTraining.exerciseIndex];
  const currentSet = exercise.exerciseSets[ongoingTraining.setIndex];

  const hasImage =
    exercise.pictureUrl !== undefined && exercise.pictureUrl !== "";
  const hasVideo = exercise.videoUrl !== undefined && exercise.videoUrl !== "";
  const hasDescription =
    exercise.description !== undefined && exercise.description !== "";

  // Determine default tab - prefer image if available, otherwise video
  const defaultTab = hasImage ? "image" : "video";
  const [activeTab, setActiveTab] = useState<string>(defaultTab);

  return (
    <div className="mx-auto flex w-full flex-col gap-4 rounded-xl pb-24 lg:pb-0">
      <WorkoutTimer />

      <CurrentSet equipment={exercise.equipment} currentSet={currentSet} />

      {/* Description Card */}
      {hasDescription && (
        <Card className="space-y-4 p-4">
          <ExerciseDescriptionCollapsible exercise={exercise} />
        </Card>
      )}

      {/* Image/Video Card with Tabs */}
      {(hasImage || hasVideo) && (
        <Card className="space-y-4 p-4">
          <Tabs
            value={activeTab}
            onValueChange={setActiveTab}
            className="w-full"
          >
            <TabsList className="grid w-full grid-cols-2">
              <TabsTrigger value="image" disabled={!hasImage}>
                Image
              </TabsTrigger>
              <TabsTrigger value="video" disabled={!hasVideo}>
                Video
              </TabsTrigger>
            </TabsList>
            <TabsContent value="image" className="mt-4">
              {hasImage && (
                <div className="flex h-80 items-center justify-center rounded-lg bg-gray-800">
                  <ImageWithSpinner
                    src={exercise.pictureUrl ?? ""}
                    alt={exercise.name}
                    className="h-full w-full rounded-lg object-contain"
                  />
                </div>
              )}
            </TabsContent>
            <TabsContent value="video" className="mt-4">
              {hasVideo && (
                <div className="overflow-hidden rounded-lg bg-black">
                  <VideoPlayerWithLoading url={exercise.videoUrl ?? ""} />
                </div>
              )}
            </TabsContent>
          </Tabs>
        </Card>
      )}

      {/* Navigation Buttons */}
      <ExercisePreviewFooter ongoingTraining={ongoingTrainingQuery.data} />

      {/* Empty div for additional scroll space on smaller devices */}
      <div className="h-4 lg:h-0" />
    </div>
  );
}

export default ExercisePreview;

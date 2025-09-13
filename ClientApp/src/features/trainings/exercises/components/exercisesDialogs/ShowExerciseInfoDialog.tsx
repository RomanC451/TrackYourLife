import { Calendar, Clock, Dumbbell, Target, Weight } from "lucide-react";

import { ImageWithSpinner } from "@/components/image-with-spinner";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import VideoPlayerWithLoading from "@/components/video-player-with-loading";
import { capitalizeFirstLetter } from "@/lib/stringUtils";
import { formatDate } from "@/lib/utils";
import { ExerciseDto } from "@/services/openapi";

interface ShowExerciseInfoDialogProps {
  exercise: ExerciseDto;
  onClose: () => void;
}

export default function ShowExerciseInfoDialog({
  exercise,
  onClose,
}: ShowExerciseInfoDialogProps) {
  return (
    <Dialog
      defaultOpen={true}
      onOpenChange={(state) => {
        if (!state) {
          onClose();
        }
      }}
    >
      <DialogContent
        className="max-h-[90vh] overflow-y-auto sm:max-w-[600px]"
        withoutOverlay
      >
        <DialogHeader className="mb-4 pl-5">
          <DialogTitle className="flex items-center gap-4 text-2xl font-bold">
            {exercise.name}

            <Badge
              variant="outline"
              className="flex items-center gap-1 rounded-lg"
            >
              <Dumbbell className="h-3 w-3" />
              {exercise.equipment || "No equipment"}
            </Badge>
          </DialogTitle>
          <DialogDescription hidden>Exercise information</DialogDescription>
        </DialogHeader>

        <div className="space-y-6">
          {/* Exercise Image */}

          {exercise.pictureUrl && (
            <div className="relative h-60 overflow-hidden rounded-lg">
              <ImageWithSpinner
                src={exercise.pictureUrl || "/placeholder.svg"}
                alt={exercise.name}
                className="h-full w-full object-cover"
              />
            </div>
          )}

          {/* Basic Information */}
          <Card>
            <CardHeader>
              <CardTitle className="text-lg">Description</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="whitespace-pre-wrap text-muted-foreground">
                {exercise.description || "No description available"}
              </p>
            </CardContent>
          </Card>

          {/* Exercise Sets and Repetitions */}
          {exercise.exerciseSets && exercise.exerciseSets.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle className="text-lg">Sets</CardTitle>
                <CardDescription>
                  Predefined sets for this exercise
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-6">
                  {exercise.exerciseSets.map((set, index) => (
                    <div className="space-y-2" key={`set-${set.name}-${index}`}>
                      <p className="text-md inline-block w-auto rounded-xl border bg-secondary px-2 font-semibold">
                        {index + 1}. {capitalizeFirstLetter(set.name)}
                      </p>

                      <div className="inline-flex w-full gap-4 rounded-md bg-secondary/60 p-2">
                        <div className="inline-flex items-center gap-2">
                          <Target className="size-4" />
                          <p>{set.reps} reps</p>
                        </div>
                        <div className="inline-flex items-center gap-2">
                          <Weight className="size-4" />
                          <p>{set.weight} kg</p>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          )}

          {/* Equipment Details */}
          <Card>
            <CardHeader>
              <CardTitle className="text-lg">Required Equipment</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex items-center gap-4">
                <div>
                  <h4 className="font-medium">
                    {exercise.equipment || "No equipment"}
                  </h4>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Exercise Video */}
          {exercise.videoUrl && (
            <Card>
              <CardHeader>
                <CardTitle className="text-lg">Exercise Video</CardTitle>
              </CardHeader>
              <CardContent>
                <VideoPlayerWithLoading url={exercise.videoUrl} />
              </CardContent>
            </Card>
          )}

          {/* Metadata */}
          <Card>
            <CardHeader>
              <CardTitle className="text-lg">Exercise Information</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div className="flex items-center gap-2">
                  <Calendar className="h-4 w-4 text-muted-foreground" />
                  <span className="text-muted-foreground">Created:</span>
                  <span>{formatDate(exercise.createdOnUtc)}</span>
                </div>
                {exercise.modifiedOnUtc && (
                  <div className="flex items-center gap-2">
                    <Clock className="h-4 w-4 text-muted-foreground" />
                    <span className="text-muted-foreground">Modified:</span>
                    <span>{formatDate(exercise.modifiedOnUtc)}</span>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        </div>
      </DialogContent>
    </Dialog>
  );
}

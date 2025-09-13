import { createFileRoute, Outlet, redirect } from "@tanstack/react-router";
import { AxiosError } from "axios";
import { StatusCodes } from "http-status-codes";

import { WorkoutTimerContextProvider } from "@/features/trainings/common/components/workoutTimer/WorkoutTimerContext";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { queryClient } from "@/queryClient";
import { preloadImage } from "@/services/openapi/preload";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/ongoing-workout",
)({
  loader: async () => {
    try {
      const ongoingTrainingData = await queryClient.ensureQueryData(
        ongoingTrainingsQueryOptions.active,
      );

      // Preload exercise images from the current training
      if (ongoingTrainingData?.training?.exercises) {
        const imageUrl =
          ongoingTrainingData.training.exercises[
            ongoingTrainingData.exerciseIndex
          ].pictureUrl;

        // Preload images in the background (don't await to avoid blocking)
        if (imageUrl) {
          preloadImage(imageUrl);
        }
      }
    } catch (error) {
      if (
        error instanceof AxiosError &&
        error.status === StatusCodes.NOT_FOUND
      ) {
        throw redirect({ to: "/trainings/workouts" });
      }
      throw error;
    }
  },
  component: OngoingWorkout,
});

function OngoingWorkout() {
  return (
    <WorkoutTimerContextProvider>
      <Outlet />
    </WorkoutTimerContextProvider>
  );
}

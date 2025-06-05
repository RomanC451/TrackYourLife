import { useNavigate, useParams } from "@tanstack/react-router";
import { CheckCircle2 } from "lucide-react";

import PageCard from "@/components/common/PageCard";
import handleQuery from "@/components/handle-query";
import { Button } from "@/components/ui/button";
import { QUERY_KEYS } from "@/features/trainings/common/data/queryKeys";
import useOngoingTrainingQuery from "@/features/trainings/ongoing-workout/queries/useOngoingTrainingQuery";
import { formatDurationMs } from "@/lib/time";
import { queryClient } from "@/queryClient";

function WorkoutFinished() {
  const navigate = useNavigate();
  const params = useParams({
    from: "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workout-finished/$ongoingTrainingId",
  });
  const { ongoingTrainingQuery } = useOngoingTrainingQuery(
    params.ongoingTrainingId,
  );

  return handleQuery(ongoingTrainingQuery, (data) => {
    const trainingName = data.training?.name || "";
    const exercisesCompleted = data.training?.exercises?.length || 0;
    return (
      <PageCard className="flex min-h-[70vh] flex-col items-center justify-center">
        <h1 className="mb-6 mt-2 text-center text-4xl font-bold text-white">
          Workout Complete!
        </h1>
        <div className="flex w-full max-w-md flex-col items-center rounded-xl bg-secondary p-8 shadow-lg">
          <h2 className="mb-1 text-2xl font-bold text-white">Great job!</h2>
          <p className="mb-6 text-gray-400">You've completed your workout</p>
          <CheckCircle2 className="my-4 h-24 w-24 text-green-500" />
          <div className="mb-6 mt-2 w-full text-left">
            <div className="mb-2 font-semibold text-white">Workout Summary</div>
            <div className="text-gray-300">
              Training:{" "}
              <span className="font-medium text-white">{trainingName}</span>
            </div>
            <div className="text-gray-300">
              Exercises completed:{" "}
              <span className="font-medium text-white">
                {exercisesCompleted}
              </span>
            </div>
            <div className="text-gray-300">
              Time taken:{" "}
              <span className="font-medium text-white">
                {formatDurationMs(
                  new Date(data.finishedOnUtc!).getTime() -
                    new Date(data.startedOnUtc).getTime(),
                )}
              </span>
            </div>
          </div>
          <div className="mt-4 flex w-full gap-4">
            <Button
              variant="default"
              className="flex-1"
              onClick={() => {
                queryClient.removeQueries({
                  queryKey: [QUERY_KEYS.activeOngoingTraining],
                });
                navigate({ to: "/trainings/workouts" });
              }}
            >
              &larr; Back to Home
            </Button>
          </div>
        </div>
      </PageCard>
    );
  });
}

export default WorkoutFinished;

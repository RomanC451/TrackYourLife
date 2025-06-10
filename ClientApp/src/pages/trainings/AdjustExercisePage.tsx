import { useNavigate, useParams } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import handleQuery from "@/components/handle-query";
import AdjustExercise from "@/features/trainings/ongoing-workout/components/adjustExerciseForm/AdjustExercise";
import useNextOngoingTrainingMutation from "@/features/trainings/ongoing-workout/mutations/useNextOngoingTrainingMutation";
import useActiveOngoingTrainingQuery from "@/features/trainings/ongoing-workout/queries/useActiveOngoingTrainingQuery";

function AdjustExercisePage() {
  const params = useParams({
    from: "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/adjust-exercise/$exerciseId",
  });

  const { activeOngoingTrainingQuery } = useActiveOngoingTrainingQuery();

  const { nextOngoingTrainingMutation } = useNextOngoingTrainingMutation();

  const navigate = useNavigate();

  return handleQuery(activeOngoingTrainingQuery, (data) => {
    const exercise = data.training.exercises.find(
      (e) => e.id === params.exerciseId,
    );

    if (!exercise) {
      return <div>Exercise not found</div>;
    }

    return (
      <PageCard className="h-auto max-w-2xl self-start">
        <PageTitle className="w-full text-center" title={data.training.name} />
        <AdjustExercise
          ongoingTrainingId={data.id}
          exercise={exercise}
          onSuccess={() => {
            nextOngoingTrainingMutation.mutate(
              {
                ongoingTraining: data,
              },
              {
                onSuccess: () => {
                  navigate({
                    to: "/trainings/ongoing-workout",
                  });
                },
              },
            );
          }}
        />
      </PageCard>
    );
  });
}

export default AdjustExercisePage;

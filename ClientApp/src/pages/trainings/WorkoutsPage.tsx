import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { TrainingsDialogsContextProvider } from "@/features/trainings/common/contexts/TrainingsDialogsContextProvider";
import CreateTrainingDialog from "@/features/trainings/trainings/components/trainingsDialogs/CreateTrainingDialog";
import WorkoutsList from "@/features/trainings/trainings/components/trainingsList/TrainingsList";

function WorkoutsPage() {
  return (
    <PageCard>
      <TrainingsDialogsContextProvider>
        <PageTitle title="Workouts">
          <CreateTrainingDialog />
        </PageTitle>
        <WorkoutsList />
      </TrainingsDialogsContextProvider>
    </PageCard>
  );
}

export default WorkoutsPage;

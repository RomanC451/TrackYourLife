import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";

import CaloriesChart from "@/features/trainings/overview/components/CaloriesChart";
import DifficultyChart from "@/features/trainings/overview/components/DifficultyChart";
import DurationChart from "@/features/trainings/overview/components/DurationChart";
import ExercisePerformanceChart from "@/features/trainings/overview/components/ExercisePerformanceChart";
import MuscleGroupsChart from "@/features/trainings/overview/components/MuscleGroupsChart";
import TopExercisesChart from "@/features/trainings/overview/components/TopExercisesChart";
import TrainingTemplatesChart from "@/features/trainings/overview/components/TrainingTemplatesChart";
import WorkoutFrequencyChart from "@/features/trainings/overview/components/WorkoutFrequencyChart";
import WorkoutSummaryCards from "@/features/trainings/overview/components/WorkoutSummaryCards";

function OverviewPage() {
  return (
    <PageCard>
      <PageTitle title="Trainings Overview" />
      <div className="container mx-auto @container space-y-6">
        {/* Summary Cards */}
        <WorkoutSummaryCards />

        {/* Time-based Analytics */}
        <div className="space-y-6">
          <WorkoutFrequencyChart />
          <DurationChart />
          <CaloriesChart />
        </div>

        {/* Distribution Analytics */}
        <div className="grid gap-6 @2xl:grid-cols-2">
          <MuscleGroupsChart />
          <DifficultyChart />
        </div>

        {/* Performance Analytics */}
        <div className="space-y-6">
          <ExercisePerformanceChart />
          <TopExercisesChart />
          <TrainingTemplatesChart />
        </div>
      </div>
    </PageCard>
  );
}

export default OverviewPage;

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { DateRangeSelector } from "@/components/common/DateRangeSelector";

import { OverviewDateRangeProvider, useOverviewDateRange } from "@/features/trainings/overview/contexts/OverviewDateRangeContext";
import CaloriesChart from "@/features/trainings/overview/components/CaloriesChart";
import DifficultyChart from "@/features/trainings/overview/components/DifficultyChart";
import DurationChart from "@/features/trainings/overview/components/DurationChart";
import ExercisePerformanceChart from "@/features/trainings/overview/components/ExercisePerformanceChart";
import MuscleGroupsChart from "@/features/trainings/overview/components/MuscleGroupsChart";
import TopExercisesChart from "@/features/trainings/overview/components/TopExercisesChart";
import TrainingTemplatesChart from "@/features/trainings/overview/components/TrainingTemplatesChart";
import WorkoutFrequencyChart from "@/features/trainings/overview/components/WorkoutFrequencyChart";
import WorkoutSummaryCards from "@/features/trainings/overview/components/WorkoutSummaryCards";

function OverviewPageContent() {
  const { selectedRange, setSelectedRange } = useOverviewDateRange();

  return (
    <>
      <div className="sticky top-0 pt-2 z-10 flex flex-col items-center gap-4 border-b border-border bg-background pb-2 sm:flex-row sm:items-center sm:justify-between ">
        <PageTitle title="Trainings Overview" />
        <DateRangeSelector
          selectedRange={selectedRange}
          handleRangeSelect={setSelectedRange}
        />
      </div>
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
    </>
  );
}

function OverviewPage() {
  return (
    <PageCard>
      <OverviewDateRangeProvider>
        <OverviewPageContent />
      </OverviewDateRangeProvider>
    </PageCard>
  );
}

export default OverviewPage;

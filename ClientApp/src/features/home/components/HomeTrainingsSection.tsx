import { Suspense } from "react";
import { Link } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";

import { Button } from "@/components/ui/button";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import WorkoutPlansTopSection from "@/features/trainings/workoutPlans/components/WorkoutPlansTopSection";

import HomeResumeWorkoutCard from "./HomeResumeWorkoutCard";
import HomeSection from "./HomeSection";
import HomeTrainingsSectionSkeleton from "./HomeTrainingsSectionSkeleton";

function HomeTrainingsSection() {
  const ongoingQuery = useQuery(ongoingTrainingsQueryOptions.active);

  return (
    <HomeSection
      title="Trainings"
      description="Your workout plan and progress"
      action={
        <Button variant="outline" size="sm" asChild>
          <Link to="/trainings/workouts">View workouts</Link>
        </Button>
      }
    >
      {ongoingQuery.isPending ? (
        <HomeTrainingsSectionSkeleton />
      ) : ongoingQuery.data ? (
        <HomeResumeWorkoutCard ongoing={ongoingQuery.data} />
      ) : (
        <Suspense fallback={<HomeTrainingsSectionSkeleton />}>
          <WorkoutPlansTopSection />
        </Suspense>
      )}
    </HomeSection>
  );
}

export default HomeTrainingsSection;

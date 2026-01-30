import { Clock, Flame, Play, Trophy } from "lucide-react";

import HandleQuery from "@/components/handle-query";
import { Card, CardContent } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { formatDurationMs } from "@/lib/time";
import { TrainingsOverviewDto } from "@/services/openapi";

import { useOverviewDateRange } from "../contexts/OverviewDateRangeContext";
import { trainingsOverviewQueryOptions } from "../queries/useTrainingsOverviewQuery";

function WorkoutSummaryCards() {
  const { startDate, endDate } = useOverviewDateRange();
  const { query: overviewQuery } = useCustomQuery(
    trainingsOverviewQueryOptions.byDateRange(startDate, endDate),
  );


  const formatTime = (seconds: number) => {
    return formatDurationMs(seconds * 1000);
  };

  const renderSuccess = (data: TrainingsOverviewDto) => {
    return (
      <div className="grid gap-4 @2xl:grid-cols-2 @5xl:grid-cols-4">
        <Card className="h-[100px]">
          <CardContent className="flex items-center justify-between p-6">
            <div className="flex items-center gap-4">
              <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10">
                <Trophy className="h-6 w-6 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total Workouts</p>
                <p className="text-2xl font-bold">{data.totalWorkoutsCompleted}</p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="h-[100px]">
          <CardContent className="flex items-center justify-between p-6">
            <div className="flex items-center gap-4">
              <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10">
                <Clock className="h-6 w-6 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total Time</p>
                <p className="text-2xl font-bold">
                  {formatTime(data.totalTrainingTimeSeconds)}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="h-[100px]">
          <CardContent className="flex items-center justify-between p-6">
            <div className="flex items-center gap-4">
              <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10">
                <Flame className="h-6 w-6 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Calories Burned</p>
                <p className="text-2xl font-bold">
                  {data.totalCaloriesBurned?.toLocaleString() ?? "—"}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="h-[100px]">
          <CardContent className="flex items-center justify-between p-6">
            <div className="flex items-center gap-4">
              <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10">
                <Play className="h-6 w-6 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Active Training</p>
                <p className="text-2xl font-bold">
                  {data.hasActiveTraining ? "Yes" : "No"}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  };

  return (
    <HandleQuery
      query={overviewQuery}
      pending={WorkoutSummaryCardsPending}
      success={renderSuccess}
    />
  );
}

function WorkoutSummaryCardsPending() {
  return (
    <div className="grid gap-4 @2xl:grid-cols-2 @5xl:grid-cols-4">
      {["workouts", "time", "calories", "active"].map((cardType) => (
        <Card key={cardType} className="h-[100px]">
          <CardContent className="flex items-center justify-between p-6">
            <div className="flex items-center gap-4">
              <Skeleton className="h-12 w-12 rounded-lg" />
              <div className="space-y-2">
                <Skeleton className="h-4 w-24" />
                <Skeleton className="h-8 w-16" />
              </div>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}

export default WorkoutSummaryCards;

import { Link } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";

import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";

import HomeSection from "@/features/home/components/HomeSection";
import { readingDashboardQueryOptions } from "@/features/reading/queries/readingQueries";
import { readingSessionsQueryOptions } from "@/features/reading/queries/readingQueries";

function HomeReadingSection() {
  const activeQuery = useQuery(readingSessionsQueryOptions.active);
  const dashboardQuery = useQuery(readingDashboardQueryOptions.dashboard);

  const progress = dashboardQuery.data?.dailyProgress;
  const streak = dashboardQuery.data?.streak;
  const progressPercent =
    progress?.hasTarget && progress.targetPages
      ? Math.min(
          100,
          Math.round((progress.pagesReadToday / progress.targetPages) * 100),
        )
      : 0;

  return (
    <HomeSection
      title="Reading"
      description="Daily reading progress and sessions"
      action={
        <Button variant="outline" size="sm" asChild>
          <Link to="/reading/dashboard">Open dashboard</Link>
        </Button>
      }
    >
      {activeQuery.data ? (
        <Card>
          <CardContent className="flex items-center justify-between gap-4 pt-6">
            <div>
              <p className="font-medium">{activeQuery.data.bookTitle}</p>
              <p className="text-muted-foreground text-sm">
                Session in progress from page {activeQuery.data.startPage}
              </p>
            </div>
            <Button asChild>
              <Link to="/reading/ongoing-session">Resume</Link>
            </Button>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardContent className="space-y-3 pt-6">
            {progress?.hasTarget ? (
              <>
                <div className="flex justify-between text-sm">
                  <span>
                    Today: {progress.pagesReadToday}/{progress.targetPages}{" "}
                    pages
                  </span>
                  <span>Streak: {streak?.currentStreak ?? 0}</span>
                </div>
                <Progress value={progressPercent} />
              </>
            ) : (
              <p className="text-muted-foreground text-sm">
                Set a daily reading goal on the dashboard.
              </p>
            )}
          </CardContent>
        </Card>
      )}
    </HomeSection>
  );
}

export default HomeReadingSection;

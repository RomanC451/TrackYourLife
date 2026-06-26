import { useSuspenseQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";

import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";

import ReadingNoteCard from "@/features/reading/notes/components/ReadingNoteCard";

import ReadingPagesChart from "./ReadingPagesChart";
import { readingDashboardQueryOptions } from "../../queries/readingQueries";

function ReadingDashboardPage() {
  const { data } = useSuspenseQuery(readingDashboardQueryOptions.dashboard);

  const progressPercent = data.dailyProgress.hasTarget
    ? Math.min(
        100,
        Math.round(
          (data.dailyProgress.pagesReadToday /
            (data.dailyProgress.targetPages ?? 1)) *
            100,
        ),
      )
    : 0;

  return (
    <>
      <PageTitle title="Reading dashboard">
        <Button variant="outline" asChild>
          <Link to="/reading/daily-goal">Daily goal</Link>
        </Button>
      </PageTitle>

      <div className="grid gap-4 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Today&apos;s progress</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {data.dailyProgress.hasTarget ? (
              <>
                <p>
                  {data.dailyProgress.pagesReadToday} /{" "}
                  {data.dailyProgress.targetPages} pages
                </p>
                <Progress value={progressPercent} />
                {!data.dailyProgress.targetMet && (
                  <p className="text-muted-foreground text-sm">
                    {data.dailyProgress.remaining} pages remaining
                  </p>
                )}
              </>
            ) : (
              <p className="text-muted-foreground text-sm">
                Set a daily reading goal to track progress.
              </p>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Streak</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-3xl font-bold">{data.streak.currentStreak}</p>
            <p className="text-muted-foreground text-sm">
              Longest: {data.streak.longestStreak} days
            </p>
          </CardContent>
        </Card>
      </div>

      <div className="mt-4">
        <ReadingPagesChart />
      </div>

      {data.activeSession && (
        <Card className="mt-4">
          <CardHeader>
            <CardTitle>Active session</CardTitle>
          </CardHeader>
          <CardContent>
            <p>{data.activeSession.bookTitle}</p>
            <Button className="mt-2" asChild>
              <Link to="/reading/ongoing-session">Resume</Link>
            </Button>
          </CardContent>
        </Card>
      )}

      <div className="mt-6 grid gap-4 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Recent books</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {data.recentBooks.length === 0 ? (
              <p className="text-muted-foreground text-sm">No ongoing books.</p>
            ) : (
              data.recentBooks.map((book) => (
                <Link
                  key={book.id}
                  to="/reading/books/$bookId"
                  params={{ bookId: book.id }}
                  className="block hover:underline"
                >
                  {book.title} ({book.currentPage}/{book.totalPages})
                </Link>
              ))
            )}
          </CardContent>
        </Card>

        <ReadingNoteCard scope="dashboard" />
      </div>
    </>
  );
}

export default ReadingDashboardPage;

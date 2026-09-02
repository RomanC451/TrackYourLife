import { useSuspenseQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { BookOpen, MoreHorizontal, Target, TrendingUp } from "lucide-react";

import PageTitle from "@/components/common/PageTitle";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import ReadingNoteCard from "@/features/reading/notes/components/ReadingNoteCard";
import { cn } from "@/lib/utils";
import type { BookDto } from "@/services/openapi";

import DashboardEyebrow from "./DashboardEyebrow";
import ReadingPagesChart from "./ReadingPagesChart";
import { readingDashboardQueryOptions } from "../../queries/readingQueries";

const bookDotClasses = [
  "bg-chart-1",
  "bg-chart-2",
  "bg-primary",
  "bg-chart-3",
] as const;

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
      <div className="space-y-1">
        <DashboardEyebrow>Your reading space</DashboardEyebrow>
        <PageTitle title="Reading dashboard">
          <Button variant="outline" asChild>
            <Link to="/reading/daily-goal">
              <Target />
              Daily goal
              {data.dailyProgress.hasTarget && (
                <span className="text-muted-foreground">
                  {data.dailyProgress.targetPages} pages
                </span>
              )}
            </Link>
          </Button>
        </PageTitle>
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-3">
            <CardTitle className="text-sm font-medium">
              Today&apos;s progress
            </CardTitle>
            <TrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent className="space-y-3">
            {data.dailyProgress.hasTarget ? (
              <>
                <p className="text-3xl font-semibold tracking-tight">
                  {data.dailyProgress.pagesReadToday}{" "}
                  <span className="text-base font-normal text-muted-foreground">
                    / {data.dailyProgress.targetPages} pages
                  </span>
                </p>
                <Progress value={progressPercent} />
                <div className="flex items-center justify-between text-sm text-muted-foreground">
                  <span>
                    {data.dailyProgress.targetMet
                      ? "Goal met"
                      : `${data.dailyProgress.remaining} pages remaining`}
                  </span>
                  <span>{progressPercent}%</span>
                </div>
              </>
            ) : (
              <p className="text-muted-foreground text-sm">
                Set a daily reading goal to track progress.
              </p>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-3">
            <CardTitle className="text-sm font-medium">Streak</CardTitle>
            <BookOpen className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent className="space-y-3">
            <p className="text-3xl font-semibold tracking-tight">
              {data.streak.currentStreak}{" "}
              <span className="text-base font-normal text-muted-foreground">
                {data.streak.currentStreak === 1 ? "day" : "days"}
              </span>
            </p>
            <div className="flex items-center justify-between gap-2 text-sm text-muted-foreground">
              <span>Longest: {data.streak.longestStreak} days</span>
              <Badge variant="secondary" className="rounded-full font-normal">
                {data.streak.todayTargetMet ? "On a roll" : "Keep going"}
              </Badge>
            </div>
          </CardContent>
        </Card>
      </div>

      {data.activeSession && (
        <Card>
          <CardHeader className="flex flex-row flex-wrap items-center justify-between gap-3 space-y-0">
            <div className="space-y-1">
              <DashboardEyebrow>Active session</DashboardEyebrow>
              <CardTitle>{data.activeSession.bookTitle}</CardTitle>
            </div>
            <Button asChild>
              <Link to="/reading/ongoing-session">Resume</Link>
            </Button>
          </CardHeader>
        </Card>
      )}

      <ReadingPagesChart />

      <div className="grid gap-4 md:grid-cols-2">
        <Card className="flex flex-col">
          <CardHeader className="flex flex-row items-center justify-between space-y-0">
            <CardTitle>Recent books</CardTitle>
            <Button variant="ghost" size="icon" asChild>
              <Link to="/reading/books" aria-label="View all books">
                <MoreHorizontal />
              </Link>
            </Button>
          </CardHeader>
          <CardContent className="space-y-3">
            {data.recentBooks.length === 0 ? (
              <p className="text-muted-foreground text-sm">No ongoing books.</p>
            ) : (
              data.recentBooks.map((book, index) => (
                <RecentBookRow key={book.id} book={book} index={index} />
              ))
            )}
          </CardContent>
        </Card>

        <ReadingNoteCard scope="dashboard" layout="chapter" />
      </div>
    </>
  );
}

function RecentBookRow({ book, index }: { book: BookDto; index: number }) {
  return (
    <Link
      to="/reading/books/$bookId"
      params={{ bookId: book.id }}
      className="flex items-center gap-3 rounded-lg px-1 py-1.5 transition-colors hover:bg-muted/60"
    >
      <span
        className={cn(
          "size-2.5 shrink-0 rounded-full",
          bookDotClasses[index % bookDotClasses.length],
        )}
        aria-hidden
      />
      <span className="min-w-0 truncate font-medium">{book.title}</span>
      <em className="ml-auto shrink-0 text-sm font-normal not-italic text-muted-foreground">
        ({book.currentPage}/{book.totalPages})
      </em>
    </Link>
  );
}

export default ReadingDashboardPage;

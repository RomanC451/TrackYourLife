import { useSuspenseQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";

import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

import { readingSessionsQueryOptions } from "../../queries/readingQueries";

function ReadingHistoryPage() {
  const { data } = useSuspenseQuery(readingSessionsQueryOptions.history);

  return (
    <>
      <PageTitle title="Reading history" />
      {data.length === 0 ? (
        <p className="text-muted-foreground">No completed sessions yet.</p>
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Date</TableHead>
              <TableHead>Book</TableHead>
              <TableHead>Pages</TableHead>
              <TableHead>Duration</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {data.map((session) => (
              <TableRow key={session.id}>
                <TableCell>{session.sessionDate}</TableCell>
                <TableCell>{session.bookTitle}</TableCell>
                <TableCell>{session.pagesRead}</TableCell>
                <TableCell>
                  {session.durationSeconds
                    ? `${Math.round(session.durationSeconds / 60)} min`
                    : "—"}
                </TableCell>
                <TableCell className="text-right">
                  <Button size="sm" variant="outline" asChild>
                    <Link
                      to="/reading/edit/$sessionId"
                      params={{ sessionId: session.id }}
                    >
                      Edit
                    </Link>
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
    </>
  );
}

export default ReadingHistoryPage;

import { Link } from "@tanstack/react-router";
import { Star } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { cn } from "@/lib/utils";

import type { GuitarTabSearchResponse } from "../api/guitarTabsApi";

function isPublicTabUrl(url: string) {
  try {
    const parsed = new URL(url);
    return (
      parsed.protocol === "https:" &&
      parsed.hostname === "tabs.ultimate-guitar.com" &&
      parsed.pathname.startsWith("/tab/")
    );
  } catch {
    return false;
  }
}

function songLabel(songName: string, part: string, version: number) {
  const details = [
    part.trim(),
    version > 1 ? `ver ${version}` : "",
  ].filter(Boolean);

  if (details.length === 0) {
    return songName;
  }

  return `${songName} (${details.join(", ")})`;
}

function RatingCell({
  rating,
  votes,
}: {
  rating: number | null;
  votes: number;
}) {
  if (rating == null && votes === 0) {
    return <span className="text-muted-foreground">—</span>;
  }

  const filledStars = rating == null ? 0 : Math.round(rating);

  return (
    <span className="inline-flex items-center justify-end gap-1">
      <span className="inline-flex" aria-hidden>
        {Array.from({ length: 5 }, (_, index) => (
          <Star
            key={index}
            className={cn(
              "size-3.5",
              index < filledStars
                ? "fill-amber-400 text-amber-400"
                : "text-muted-foreground/40",
            )}
          />
        ))}
      </span>
      {votes > 0 && (
        <span className="text-xs text-muted-foreground">{votes}</span>
      )}
    </span>
  );
}

function GuitarTabSearchResults({
  response,
  isError,
  isLoading,
  onPageChange,
}: {
  response: GuitarTabSearchResponse | undefined;
  isError: boolean;
  isLoading: boolean;
  onPageChange: (page: number) => void;
}) {
  if (isLoading && response == null) {
    return (
      <p className="py-12 text-center text-muted-foreground">Searching…</p>
    );
  }

  if (isError && response == null) {
    return (
      <p className="py-12 text-center text-muted-foreground">
        Ultimate Guitar search is unavailable.
      </p>
    );
  }

  if (response == null) {
    return null;
  }

  if (response.results.length === 0) {
    return (
      <p className="py-12 text-center text-muted-foreground">
        No chords or tabs found.
      </p>
    );
  }

  return (
    <div className="flex flex-col gap-4">
      <p className="text-sm text-muted-foreground">
        {response.resultsCount} chords & tabs found
      </p>
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Artist</TableHead>
            <TableHead>Song</TableHead>
            <TableHead className="text-right">Rating</TableHead>
            <TableHead>Type</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {response.results.map((result) => (
            <TableRow key={result.tabUrl}>
              <TableCell>{result.artistName}</TableCell>
              <TableCell>
                {isPublicTabUrl(result.tabUrl) ? (
                  <Link
                    to="/guitar-tabs/tab"
                    search={{ url: result.tabUrl }}
                    className="font-medium hover:underline"
                  >
                    {songLabel(result.songName, result.part, result.version)}
                  </Link>
                ) : (
                  <a
                    href={result.tabUrl}
                    target="_blank"
                    rel="noreferrer"
                    className="font-medium hover:underline"
                  >
                    {songLabel(result.songName, result.part, result.version)}
                  </a>
                )}
              </TableCell>
              <TableCell className="text-right">
                <RatingCell rating={result.rating} votes={result.votes} />
              </TableCell>
              <TableCell>{result.type}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
      {response.totalPages > 1 && (
        <div className="flex items-center justify-end gap-3">
          <Button
            type="button"
            variant="outline"
            size="sm"
            disabled={response.page <= 1}
            onClick={() => onPageChange(response.page - 1)}
          >
            Previous
          </Button>
          <span className="text-sm text-muted-foreground">
            Page {response.page} of {response.totalPages}
          </span>
          <Button
            type="button"
            variant="outline"
            size="sm"
            disabled={response.page >= response.totalPages}
            onClick={() => onPageChange(response.page + 1)}
          >
            Next
          </Button>
        </div>
      )}
    </div>
  );
}

export default GuitarTabSearchResults;

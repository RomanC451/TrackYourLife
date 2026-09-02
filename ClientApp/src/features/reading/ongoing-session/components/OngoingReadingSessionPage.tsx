import { useQuery } from "@tanstack/react-query";
import { Link, useNavigate } from "@tanstack/react-router";
import {
  BookOpen,
  Check,
  FileText,
  Pause,
  Play,
  Plus,
  RotateCcw,
} from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import DashboardEyebrow from "@/features/reading/dashboard/components/DashboardEyebrow";

import BookPreviousNotesPanel from "./BookPreviousNotesPanel";
import FinishReadingSessionDialog from "./FinishReadingSessionDialog";
import { useReadingTimer } from "../context/ReadingTimerContext";
import {
  useAddReadingSessionNoteMutation,
  useCancelReadingSessionMutation,
} from "../mutations/readingSessionMutations";
import {
  getNewestNoteChapterTitle,
  parseChapterTitle,
  resolveChapterTitle,
} from "../../notes/bookNotesUtils";
import { readingSessionsQueryOptions } from "../../queries/readingQueries";

function formatTime(totalSeconds: number) {
  const minutes = Math.floor(totalSeconds / 60);
  const seconds = totalSeconds % 60;
  return `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;
}

function OngoingSessionContent() {
  const navigate = useNavigate();
  const { data: session } = useQuery(readingSessionsQueryOptions.active);
  const cancelMutation = useCancelReadingSessionMutation();
  const addNoteMutation = useAddReadingSessionNoteMutation();
  const { elapsedSeconds, isRunning, start, pause } = useReadingTimer();

  const [chapterTitle, setChapterTitle] = useState("");
  const [notes, setNotes] = useState("");
  const [finishDialogOpen, setFinishDialogOpen] = useState(false);
  const hasInitializedChapter = useRef(false);

  const { data: chapterGroups } = useQuery({
    ...readingSessionsQueryOptions.bookChapterNotes(session?.bookId ?? ""),
    enabled: !!session?.bookId,
  });

  useEffect(() => {
    hasInitializedChapter.current = false;
  }, [session?.id]);

  useEffect(() => {
    const newestChapter = getNewestNoteChapterTitle(chapterGroups);
    if (hasInitializedChapter.current || !newestChapter) {
      return;
    }

    const parsed = parseChapterTitle(newestChapter);
    setChapterTitle(parsed.title || newestChapter);
    hasInitializedChapter.current = true;
  }, [chapterGroups, session?.id]);

  if (!session) {
    return (
      <p className="text-muted-foreground">
        No active session.{" "}
        <Link to="/reading/books" className="underline">
          Pick a book
        </Link>{" "}
        to start reading.
      </p>
    );
  }

  const canSave =
    chapterTitle.trim().length > 0 && notes.trim().length > 0;

  const handleSaveNote = async () => {
    if (!canSave) {
      return;
    }

    const savedChapterTitle = chapterTitle.trim();

    await addNoteMutation.mutateAsync({
      sessionId: session.id,
      bookId: session.bookId,
      chapterTitle: resolveChapterTitle(savedChapterTitle, chapterGroups),
      content: notes.trim(),
    });

    setNotes("");
    setChapterTitle(savedChapterTitle);
    toast.success("Note saved.");
  };

  const handleFinished = (suggestMarkAsFinished: boolean) => {
    if (suggestMarkAsFinished) {
      toast.message(
        "You reached the last page — consider marking the book as Finished.",
      );
    }

    navigate({ to: "/reading/dashboard" });
  };

  return (
    <div className="mx-auto flex w-full max-w-2xl flex-col gap-6">
      <header className="space-y-3">
        <div className="flex items-center gap-1.5">
          <BookOpen className="size-3.5 text-muted-foreground" aria-hidden />
          <DashboardEyebrow className="w-auto">Reading session</DashboardEyebrow>
        </div>
        <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between sm:gap-4">
          <div className="min-w-0 space-y-1">
            <h1 className="text-3xl font-bold tracking-tight">
              {session.bookTitle}
            </h1>
            <p className="text-muted-foreground text-sm">
              {session.bookAuthor} · Started at page {session.startPage}
            </p>
          </div>
          <div className="text-muted-foreground flex shrink-0 items-center gap-2 text-sm">
            <span className="bg-primary size-2 rounded-full" aria-hidden />
            <span>In progress</span>
          </div>
        </div>
      </header>

      <Card aria-label="Session timer">
        <CardContent className="flex flex-col gap-4 p-6 sm:flex-row sm:items-center sm:justify-between">
          <div className="space-y-1">
            <DashboardEyebrow>Session time</DashboardEyebrow>
            <p className="font-mono text-4xl font-semibold tabular-nums sm:text-5xl">
              {formatTime(elapsedSeconds)}
            </p>
          </div>
          <Button
            variant={isRunning ? "secondary" : "default"}
            onClick={isRunning ? pause : start}
          >
            {isRunning ? (
              <>
                <Pause />
                Pause
              </>
            ) : (
              <>
                <Play />
                Resume
              </>
            )}
          </Button>
        </CardContent>
      </Card>

      <BookPreviousNotesPanel bookId={session.bookId} />

      <Card>
        <CardContent className="space-y-5 p-6">
          <div className="flex items-start gap-3">
            <FileText className="mt-0.5 size-5 text-muted-foreground" />
            <div className="space-y-1">
              <h2 className="text-lg font-semibold leading-none">Add a note</h2>
              <p className="text-muted-foreground text-sm">
                Capture what stands out while it is fresh.
              </p>
            </div>
          </div>

          <div className="flex min-w-0 flex-col gap-1.5">
            <Label htmlFor="chapter-title">Chapter title</Label>
            <Input
              id="chapter-title"
              value={chapterTitle}
              onChange={(e) => setChapterTitle(e.target.value)}
              placeholder="e.g. Flux și concentrare"
            />
          </div>

          <div className="flex flex-col gap-1.5">
            <Label htmlFor="session-notes">Note</Label>
            <Textarea
              id="session-notes"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              placeholder="Write your thoughts about this chapter..."
              className="min-h-32 resize-y"
            />
          </div>

          <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <p className="text-muted-foreground text-xs">
              Use the same chapter title to keep related notes together.
            </p>
            <ButtonWithLoading
              onClick={handleSaveNote}
              disabled={!canSave}
              isLoading={addNoteMutation.isPending}
              className="sm:self-auto"
            >
              <Plus />
              Save note
            </ButtonWithLoading>
          </div>
        </CardContent>
      </Card>

      <div className="flex flex-col-reverse gap-3 sm:flex-row sm:items-center">
        <ButtonWithLoading
          variant="outline"
          className="sm:flex-1"
          isLoading={cancelMutation.isPending}
          onClick={async () => {
            await cancelMutation.mutateAsync(session.id);
            navigate({ to: "/reading/books" });
          }}
        >
          <RotateCcw />
          Cancel
        </ButtonWithLoading>
        <Button
          className="sm:flex-1"
          onClick={() => setFinishDialogOpen(true)}
        >
          <Check />
          Finish session
        </Button>
      </div>

      <FinishReadingSessionDialog
        session={session}
        open={finishDialogOpen}
        onOpenChange={setFinishDialogOpen}
        elapsedSeconds={elapsedSeconds}
        onFinished={handleFinished}
      />
    </div>
  );
}

function OngoingReadingSessionPage() {
  return <OngoingSessionContent />;
}

export default OngoingReadingSessionPage;

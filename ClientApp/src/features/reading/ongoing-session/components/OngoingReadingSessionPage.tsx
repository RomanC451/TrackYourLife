import { useQuery } from "@tanstack/react-query";
import { Link, useNavigate } from "@tanstack/react-router";
import {
  BookOpen,
  Pause,
  Play,
  Plus,
  StickyNote,
} from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";

import BookPreviousNotesPanel from "./BookPreviousNotesPanel";
import FinishReadingSessionDialog from "./FinishReadingSessionDialog";
import { useReadingTimer } from "../context/ReadingTimerContext";
import { useAddReadingSessionNoteMutation, useCancelReadingSessionMutation } from "../mutations/readingSessionMutations";
import { getNewestNoteChapterTitle } from "../../notes/bookNotesUtils";
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

    setChapterTitle(newestChapter);
    hasInitializedChapter.current = true;
  }, [chapterGroups, session?.id]);

  if (!session) {
    return (
      <p className="text-muted-foreground">
        No active session.{" "}
        <Link to="/books" className="underline">
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
      chapterTitle: savedChapterTitle,
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
      <header className="flex flex-col gap-1">
        <div className="flex items-center gap-2 text-sm font-medium text-primary">
          <BookOpen className="size-4" aria-hidden="true" />
          Reading session
        </div>
        <h1 className="text-pretty text-2xl font-semibold leading-tight sm:text-3xl">
          {session.bookTitle}
        </h1>
        <p className="text-sm text-muted-foreground">
          {session.bookAuthor} · Started at page {session.startPage}
        </p>
      </header>

      <section className="flex flex-col gap-4 rounded-xl border border-border bg-card p-5">
        <span className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
          Session time
        </span>
        <div className="flex items-end justify-between gap-3">
          <span className="font88-mono text-4xl font-semibold tabular-nums sm:text-5xl">
            {formatTime(elapsedSeconds)}
          </span>
          <Button
            variant={isRunning ? "secondary" : "default"}
            size="sm"
            onClick={isRunning ? pause : start}
            className="gap-1.5"
          >
            {isRunning ? (
              <>
                <Pause className="size-4" aria-hidden="true" />
                Pause
              </>
            ) : (
              <>
                <Play className="size-4" aria-hidden="true" />
                Resume
              </>
            )}
          </Button>
        </div>
      </section>

      <BookPreviousNotesPanel bookId={session.bookId} />

      <section className="flex flex-col gap-3 rounded-xl border border-border bg-card p-5">
        <div className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
          <StickyNote className="size-4" aria-hidden="true" />
          Add a note
        </div>
        <div className="flex flex-col gap-1.5">
          <label
            htmlFor="chapter-title"
            className="text-xs font-medium text-muted-foreground"
          >
            Chapter
          </label>
          <Input
            id="chapter-title"
            value={chapterTitle}
            onChange={(e) => setChapterTitle(e.target.value)}
            placeholder="e.g. Cap. 5 — Flux și concentrare"
          />
        </div>
        <div className="flex flex-col gap-1.5">
          <label
            htmlFor="session-notes"
            className="text-xs font-medium text-muted-foreground"
          >
            Note
          </label>
          <Textarea
            id="session-notes"
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            placeholder="Write your thoughts about this chapter..."
            className="min-h-32 resize-y"
          />
        </div>
        <p className="text-xs text-muted-foreground">
          Notes are grouped by chapter. Type an existing chapter to add to it,
          or a new one to start a new group.
        </p>
        <ButtonWithLoading
          onClick={handleSaveNote}
          disabled={!canSave}
          isLoading={addNoteMutation.isPending}
          className="gap-1.5 self-start"
        >
          <Plus className="size-4" aria-hidden="true" />
          Save note
        </ButtonWithLoading>
      </section>

      <div className="flex flex-col-reverse gap-3 sm:flex-row sm:items-center">
        <ButtonWithLoading
          variant="outline"
          className="sm:flex-1"
          isLoading={cancelMutation.isPending}
          onClick={async () => {
            await cancelMutation.mutateAsync(session.id);
            navigate({ to: "/books" });
          }}
        >
          Cancel
        </ButtonWithLoading>
        <Button
          className="gap-1.5 sm:flex-1"
          onClick={() => setFinishDialogOpen(true)}
        >
          <BookOpen className="size-4" aria-hidden="true" />
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

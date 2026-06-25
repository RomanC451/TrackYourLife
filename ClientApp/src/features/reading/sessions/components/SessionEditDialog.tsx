import { useQuery } from "@tanstack/react-query";
import { Pencil, Plus, StickyNote, Trash2 } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";
import { v4 as uuidv4 } from "uuid";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import type { ReadingSessionDto, ReadingSessionNoteDto } from "@/services/openapi";

import {
  useAddReadingSessionNoteMutation,
  useDeleteReadingSessionNoteMutation,
  useUpdateReadingSessionMutation,
  useUpdateReadingSessionNoteMutation,
} from "../../ongoing-session/mutations/readingSessionMutations";
import {
  formatChapterTitle,
  parseChapterTitle,
} from "../../notes/bookNotesUtils";
import { readingSessionsQueryOptions } from "../../queries/readingQueries";

type SessionEditDialogProps = {
  session: ReadingSessionDto;
  onClose: () => void;
};

type EditableSessionNote = {
  key: string;
  noteId: string | null;
  chapterNumber: string;
  chapterTitle: string;
  content: string;
  deleted: boolean;
};

function mapNoteToEditable(note: ReadingSessionNoteDto): EditableSessionNote {
  const parsed = parseChapterTitle(note.chapterTitle);

  return {
    key: note.id,
    noteId: note.id,
    chapterNumber: parsed.chapterNumber,
    chapterTitle: parsed.title,
    content: note.content,
    deleted: false,
  };
}

function isNoteComplete(note: EditableSessionNote) {
  return (
    note.content.trim().length > 0 &&
    (note.chapterNumber.trim().length > 0 || note.chapterTitle.trim().length > 0)
  );
}

function isNewNoteComplete(note: EditableSessionNote) {
  return (
    note.chapterNumber.trim().length > 0 &&
    note.chapterTitle.trim().length > 0 &&
    note.content.trim().length > 0
  );
}

function getChapterTitleForSave(note: EditableSessionNote) {
  const chapterNumber = note.chapterNumber.trim();
  const chapterTitle = note.chapterTitle.trim();

  if (chapterNumber && chapterTitle) {
    return formatChapterTitle(chapterNumber, chapterTitle);
  }

  if (chapterNumber) {
    return chapterTitle
      ? formatChapterTitle(chapterNumber, chapterTitle)
      : `Chapter ${chapterNumber}`;
  }

  return chapterTitle;
}

function isEmptyDraft(note: EditableSessionNote) {
  return (
    note.noteId === null &&
    note.chapterNumber.trim().length === 0 &&
    note.chapterTitle.trim().length === 0 &&
    note.content.trim().length === 0
  );
}

type SessionNoteFormProps = {
  note: EditableSessionNote;
  onChange: (
    patch: Partial<
      Pick<EditableSessionNote, "chapterNumber" | "chapterTitle" | "content">
    >,
  ) => void;
  onCancel: () => void;
  onDelete?: () => void;
  title: string;
};

function SessionNoteForm({
  note,
  onChange,
  onCancel,
  onDelete,
  title,
}: SessionNoteFormProps) {
  return (
    <div className="flex flex-col gap-3 rounded-lg border border-border bg-muted/30 p-3">
      <div className="flex items-start justify-between gap-2">
        <p className="text-sm font-medium">{title}</p>
        <div className="flex items-center gap-1">
          {onDelete && (
            <Button
              type="button"
              variant="ghost"
              size="icon"
              className="size-8 shrink-0"
              onClick={onDelete}
              aria-label="Remove note"
            >
              <Trash2 className="size-4" aria-hidden="true" />
            </Button>
          )}
          <Button type="button" variant="ghost" size="sm" onClick={onCancel}>
            Cancel
          </Button>
        </div>
      </div>
      <div className="flex flex-col gap-3 sm:flex-row">
        <div className="flex flex-col gap-1.5 sm:w-28">
          <Label htmlFor={`chapter-number-${note.key}`}>Chapter number</Label>
          <Input
            id={`chapter-number-${note.key}`}
            inputMode="numeric"
            value={note.chapterNumber}
            onChange={(e) => onChange({ chapterNumber: e.target.value })}
            placeholder="5"
          />
        </div>
        <div className="flex min-w-0 flex-1 flex-col gap-1.5">
          <Label htmlFor={`chapter-title-${note.key}`}>Chapter title</Label>
          <Input
            id={`chapter-title-${note.key}`}
            value={note.chapterTitle}
            onChange={(e) => onChange({ chapterTitle: e.target.value })}
            placeholder="e.g. Flux și concentrare"
          />
        </div>
      </div>
      <div className="flex flex-col gap-1.5">
        <Label htmlFor={`note-content-${note.key}`}>Note</Label>
        <Textarea
          id={`note-content-${note.key}`}
          value={note.content}
          onChange={(e) => onChange({ content: e.target.value })}
          rows={4}
        />
      </div>
    </div>
  );
}

function SessionEditDialog({ session, onClose }: SessionEditDialogProps) {
  const updateMutation = useUpdateReadingSessionMutation();
  const addNoteMutation = useAddReadingSessionNoteMutation();
  const updateNoteMutation = useUpdateReadingSessionNoteMutation();
  const deleteNoteMutation = useDeleteReadingSessionNoteMutation();

  const { data, isLoading: notesLoading } = useQuery(
    readingSessionsQueryOptions.sessionNotes(session.id),
  );
  const sessionNotes = Array.isArray(data) ? data : [];

  const [endPage, setEndPage] = useState(String(session.endPage ?? ""));
  const [sessionDate, setSessionDate] = useState(session.sessionDate ?? "");
  const [durationMinutes, setDurationMinutes] = useState(
    session.durationSeconds
      ? String(Math.round(session.durationSeconds / 60))
      : "",
  );
  const [editableNotes, setEditableNotes] = useState<EditableSessionNote[]>([]);
  const [editingNoteKey, setEditingNoteKey] = useState<string | null>(null);
  const [isAddingNote, setIsAddingNote] = useState(false);

  const originalNotes = useMemo(
    () => sessionNotes.map(mapNoteToEditable),
    [sessionNotes],
  );

  useEffect(() => {
    setEndPage(String(session.endPage ?? ""));
    setSessionDate(session.sessionDate ?? "");
    setDurationMinutes(
      session.durationSeconds
        ? String(Math.round(session.durationSeconds / 60))
        : "",
    );
  }, [session]);

  useEffect(() => {
    setEditableNotes(sessionNotes.map(mapNoteToEditable));
    setEditingNoteKey(null);
    setIsAddingNote(false);
  }, [sessionNotes]);

  const visibleNotes = editableNotes.filter((note) => !note.deleted);
  const editingNote = visibleNotes.find((note) => note.key === editingNoteKey);
  const addingNote = visibleNotes.find((note) => note.noteId === null);
  const listNotes = visibleNotes.filter(
    (note) => note.key !== editingNoteKey && note.noteId !== null,
  );

  const isSaving =
    updateMutation.isPending ||
    addNoteMutation.isPending ||
    updateNoteMutation.isPending ||
    deleteNoteMutation.isPending;

  const updateNote = (
    key: string,
    patch: Partial<
      Pick<EditableSessionNote, "chapterNumber" | "chapterTitle" | "content">
    >,
  ) => {
    setEditableNotes((notes) =>
      notes.map((note) => (note.key === key ? { ...note, ...patch } : note)),
    );
  };

  const startAddNote = () => {
    setEditingNoteKey(null);
    setIsAddingNote(true);

    setEditableNotes((notes) => {
      const draft = notes.find((note) => note.noteId === null && !note.deleted);
      if (draft) {
        return notes;
      }

      return [
        ...notes,
        {
          key: uuidv4(),
          noteId: null,
          chapterNumber: "",
          chapterTitle: "",
          content: "",
          deleted: false,
        },
      ];
    });
  };

  const cancelAddNote = () => {
    setIsAddingNote(false);
    setEditableNotes((notes) =>
      notes.filter((note) => note.noteId !== null || note.deleted),
    );
  };

  const removeNote = (key: string) => {
    setEditableNotes((notes) =>
      notes
        .map((note) =>
          note.key === key
            ? note.noteId
              ? { ...note, deleted: true }
              : null
            : note,
        )
        .filter((note): note is EditableSessionNote => note !== null),
    );

    if (editingNoteKey === key) {
      setEditingNoteKey(null);
    }

    if (addingNote?.key === key) {
      setIsAddingNote(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const parsedEnd = Number.parseInt(endPage, 10);
    if (!Number.isFinite(parsedEnd)) {
      toast.error("Enter a valid end page.");
      return;
    }

    const notesToSave = editableNotes.filter(
      (note) => !note.deleted && !isEmptyDraft(note),
    );

    if (
      notesToSave.some((note) =>
        note.noteId === null ? !isNewNoteComplete(note) : !isNoteComplete(note),
      )
    ) {
      toast.error("Complete all note fields or remove unfinished notes.");
      return;
    }

    const durationSeconds = durationMinutes
      ? Number.parseInt(durationMinutes, 10) * 60
      : undefined;

    await updateMutation.mutateAsync({
      id: session.id,
      body: {
        endPage: parsedEnd,
        sessionDate,
        durationSeconds:
          durationSeconds && Number.isFinite(durationSeconds)
            ? durationSeconds
            : undefined,
      },
    });

    const originalById = new Map(
      originalNotes
        .filter((note) => note.noteId)
        .map((note) => [note.noteId!, note]),
    );

    for (const note of editableNotes) {
      if (note.deleted && note.noteId) {
        await deleteNoteMutation.mutateAsync({
          sessionId: session.id,
          noteId: note.noteId,
        });
        continue;
      }

      if (note.deleted || isEmptyDraft(note)) {
        continue;
      }

      const chapterTitle = getChapterTitleForSave(note);
      const content = note.content.trim();

      if (!note.noteId) {
        await addNoteMutation.mutateAsync({
          sessionId: session.id,
          bookId: session.bookId,
          chapterTitle,
          content,
        });
        continue;
      }

      const original = originalById.get(note.noteId);
      const hasChanged =
        !original ||
        original.chapterNumber !== note.chapterNumber ||
        original.chapterTitle !== note.chapterTitle ||
        original.content !== note.content;

      if (hasChanged) {
        await updateNoteMutation.mutateAsync({
          sessionId: session.id,
          noteId: note.noteId,
          chapterTitle,
          content,
        });
      }
    }

    onClose();
  };

  return (
    <Dialog open onOpenChange={(open) => !open && onClose()}>
      <DialogContent className="max-h-[90vh] max-w-md overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Edit session — {session.bookTitle}</DialogTitle>
          <DialogDescription className="sr-only">
            Edit session details and chapter notes.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <section className="flex flex-col gap-3 rounded-xl border border-border bg-card p-4">
            <div className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
              <StickyNote className="size-4" aria-hidden="true" />
              Session notes
            </div>

            {notesLoading ? (
              <p className="text-muted-foreground text-sm">Loading notes...</p>
            ) : listNotes.length === 0 && !editingNote && !isAddingNote ? (
              <p className="text-muted-foreground text-sm">
                No notes for this session yet.
              </p>
            ) : (
              <ul className="flex flex-col gap-2">
                {listNotes.map((note) => (
                  <li
                    key={note.key}
                    className="flex items-start justify-between gap-3 rounded-lg border border-border p-3"
                  >
                    <div className="min-w-0 flex-1">
                      <p className="text-sm font-medium">
                        {getChapterTitleForSave(note)}
                      </p>
                      <p className="text-muted-foreground mt-1 line-clamp-3 text-sm">
                        {note.content}
                      </p>
                    </div>
                    <div className="flex shrink-0 items-center gap-1">
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        className="gap-1.5"
                        onClick={() => {
                          setIsAddingNote(false);
                          setEditingNoteKey(note.key);
                        }}
                      >
                        <Pencil className="size-3.5" aria-hidden="true" />
                        Edit
                      </Button>
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        className="size-8"
                        onClick={() => removeNote(note.key)}
                        aria-label="Remove note"
                      >
                        <Trash2 className="size-4" aria-hidden="true" />
                      </Button>
                    </div>
                  </li>
                ))}
              </ul>
            )}

            {editingNote && (
              <SessionNoteForm
                note={editingNote}
                title="Edit note"
                onChange={(patch) => updateNote(editingNote.key, patch)}
                onCancel={() => setEditingNoteKey(null)}
                onDelete={() => removeNote(editingNote.key)}
              />
            )}

            {isAddingNote && addingNote && (
              <SessionNoteForm
                note={addingNote}
                title="New note"
                onChange={(patch) => updateNote(addingNote.key, patch)}
                onCancel={cancelAddNote}
                onDelete={() => removeNote(addingNote.key)}
              />
            )}

            {!isAddingNote && (
              <Button
                type="button"
                variant="outline"
                className="gap-1.5 self-start"
                onClick={startAddNote}
              >
                <Plus className="size-4" aria-hidden="true" />
                Add note
              </Button>
            )}
          </section>

          <div>
            <Label htmlFor="sessionDate">Session date</Label>
            <Input
              id="sessionDate"
              type="date"
              value={sessionDate}
              onChange={(e) => setSessionDate(e.target.value)}
              required
            />
          </div>
          <div>
            <Label htmlFor="endPage">End page</Label>
            <Input
              id="endPage"
              type="number"
              min={session.startPage}
              value={endPage}
              onChange={(e) => setEndPage(e.target.value)}
              required
            />
            <p className="text-muted-foreground mt-1 text-xs">
              Started at page {session.startPage}
            </p>
          </div>
          <div>
            <Label htmlFor="duration">Duration (minutes)</Label>
            <Input
              id="duration"
              type="number"
              min={0}
              value={durationMinutes}
              onChange={(e) => setDurationMinutes(e.target.value)}
            />
          </div>

          <div className="flex justify-end">
            <ButtonWithLoading type="submit" isLoading={isSaving}>
              Save changes
            </ButtonWithLoading>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export default SessionEditDialog;

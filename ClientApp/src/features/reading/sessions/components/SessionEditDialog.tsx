import { useEffect, useState } from "react";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import type { ReadingSessionDto } from "@/services/openapi";

import { useUpdateReadingSessionMutation } from "../../ongoing-session/mutations/readingSessionMutations";

type SessionEditDialogProps = {
  session: ReadingSessionDto;
  onClose: () => void;
};

function SessionEditDialog({ session, onClose }: SessionEditDialogProps) {
  const updateMutation = useUpdateReadingSessionMutation();

  const [endPage, setEndPage] = useState(String(session.endPage ?? ""));
  const [sessionDate, setSessionDate] = useState(session.sessionDate ?? "");
  const [notes, setNotes] = useState(session.notes ?? "");
  const [durationMinutes, setDurationMinutes] = useState(
    session.durationSeconds
      ? String(Math.round(session.durationSeconds / 60))
      : "",
  );

  useEffect(() => {
    setEndPage(String(session.endPage ?? ""));
    setSessionDate(session.sessionDate ?? "");
    setNotes(session.notes ?? "");
    setDurationMinutes(
      session.durationSeconds
        ? String(Math.round(session.durationSeconds / 60))
        : "",
    );
  }, [session]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const parsedEnd = Number.parseInt(endPage, 10);
    if (!Number.isFinite(parsedEnd)) return;

    const durationSeconds = durationMinutes
      ? Number.parseInt(durationMinutes, 10) * 60
      : undefined;

    await updateMutation.mutateAsync({
      id: session.id,
      body: {
        endPage: parsedEnd,
        sessionDate,
        notes: notes || undefined,
        durationSeconds:
          durationSeconds && Number.isFinite(durationSeconds)
            ? durationSeconds
            : undefined,
      },
    });

    onClose();
  };

  return (
    <Dialog open onOpenChange={(open) => !open && onClose()}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Edit session — {session.bookTitle}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
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
          <div>
            <Label htmlFor="notes">Notes</Label>
            <Textarea
              id="notes"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              rows={4}
            />
          </div>
          <div className="flex justify-end">
            <ButtonWithLoading
              type="submit"
              isLoading={updateMutation.isPending}
            >
              Save changes
            </ButtonWithLoading>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export default SessionEditDialog;

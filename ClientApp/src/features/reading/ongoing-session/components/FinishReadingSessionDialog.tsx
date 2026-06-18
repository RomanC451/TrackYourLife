import { useState } from "react";
import { toast } from "sonner";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import type { ReadingSessionDto } from "@/services/openapi";

import { useFinishReadingSessionMutation } from "../mutations/readingSessionMutations";

type FinishReadingSessionDialogProps = {
  session: ReadingSessionDto;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  elapsedSeconds: number;
  onFinished: (suggestMarkAsFinished: boolean) => void;
};

function FinishReadingSessionDialog({
  session,
  open,
  onOpenChange,
  elapsedSeconds,
  onFinished,
}: FinishReadingSessionDialogProps) {
  const finishMutation = useFinishReadingSessionMutation();
  const [endPage, setEndPage] = useState("");

  const handleOpenChange = (nextOpen: boolean) => {
    if (!nextOpen) {
      setEndPage("");
    }
    onOpenChange(nextOpen);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const parsedEnd = Number.parseInt(endPage, 10);
    if (!Number.isFinite(parsedEnd)) {
      toast.error("Enter a valid end page.");
      return;
    }

    const result = await finishMutation.mutateAsync({
      id: session.id,
      body: {
        endPage: parsedEnd,
        durationSeconds: elapsedSeconds || undefined,
      },
    });

    setEndPage("");
    onOpenChange(false);
    onFinished(result.suggestMarkAsFinished);
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Finish session</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <div>
            <Label htmlFor="finishEndPage">End page</Label>
            <Input
              id="finishEndPage"
              type="number"
              min={session.startPage}
              value={endPage}
              onChange={(e) => setEndPage(e.target.value)}
              required
              autoFocus
            />
            <p className="text-muted-foreground mt-1 text-xs">
              Started at page {session.startPage}
            </p>
          </div>
          <DialogFooter>
            <ButtonWithLoading
              type="submit"
              isLoading={finishMutation.isPending}
            >
              Finish session
            </ButtonWithLoading>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export default FinishReadingSessionDialog;

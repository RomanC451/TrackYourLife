import { useState } from "react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import {
  saveCustomTuning,
  updateCustomTuning,
  type CustomTuning,
} from "../lib/customTunings";

function CreateTuningDialog({
  open,
  tuning,
  onOpenChange,
}: {
  open: boolean;
  tuning?: CustomTuning;
  onOpenChange: (open: boolean) => void;
}) {
  const [name, setName] = useState(tuning?.name ?? "");
  const [notes, setNotes] = useState(tuning?.notes.join(" ") ?? "");
  const [error, setError] = useState<string | null>(null);

  const handleOpenChange = (nextOpen: boolean) => {
    if (!nextOpen) {
      setName("");
      setNotes("");
      setError(null);
    }
    onOpenChange(nextOpen);
  };

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    try {
      const result = tuning
        ? updateCustomTuning(tuning.id, name, notes)
        : saveCustomTuning(name, notes);
      if ("error" in result) {
        setError(result.error);
        return;
      }
      handleOpenChange(false);
    } catch {
      setError("Couldn't save this tuning.");
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent>
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>{tuning ? "Edit tuning" : "New tuning"}</DialogTitle>
            <DialogDescription>
              Name it and list the six open strings from low to high. It stays
              in this browser.
            </DialogDescription>
          </DialogHeader>
          <div className="grid gap-4 py-4">
            <div className="grid gap-2">
              <Label htmlFor="custom-tuning-name">Name</Label>
              <Input
                id="custom-tuning-name"
                value={name}
                onChange={(event) => setName(event.target.value)}
                placeholder="C# Tuning"
                maxLength={80}
                autoFocus
              />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="custom-tuning-notes">Notes</Label>
              <Input
                id="custom-tuning-notes"
                value={notes}
                onChange={(event) => setNotes(event.target.value)}
                placeholder="C# F# B E G# C#"
                autoCapitalize="off"
                autoCorrect="off"
                spellCheck={false}
              />
            </div>
            {error ? <p className="text-sm text-destructive">{error}</p> : null}
          </div>
          <DialogFooter>
            <Button type="submit">Save tuning</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export default CreateTuningDialog;

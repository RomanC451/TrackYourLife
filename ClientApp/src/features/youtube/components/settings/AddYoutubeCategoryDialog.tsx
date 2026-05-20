import { useState } from "react";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import { useCreateYoutubeCategoryMutation } from "../../mutations/useYoutubeCategoryMutations";

interface AddYoutubeCategoryDialogProps {
  disabled?: boolean;
}

export function AddYoutubeCategoryDialog({
  disabled = false,
}: AddYoutubeCategoryDialogProps) {
  const [open, setOpen] = useState(false);
  const [name, setName] = useState("");
  const [maxVideosPerDayRaw, setMaxVideosPerDayRaw] = useState("0");

  const createMutation = useCreateYoutubeCategoryMutation();

  const resetForm = () => {
    setName("");
    setMaxVideosPerDayRaw("0");
  };

  const handleOpenChange = (next: boolean) => {
    setOpen(next);
    if (!next) {
      resetForm();
    }
  };

  const maxVideosPerDay = Number(maxVideosPerDayRaw);
  const maxOk =
    maxVideosPerDayRaw.trim() !== "" &&
    Number.isFinite(maxVideosPerDay) &&
    maxVideosPerDay >= 0;

  const handleSubmit = () => {
    const trimmed = name.trim();
    if (!trimmed || !maxOk) {
      return;
    }
    createMutation.mutate(
      { name: trimmed, maxVideosPerDay },
      {
        onSuccess: () => handleOpenChange(false),
      },
    );
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button type="button" disabled={disabled}>
          Add category
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Add category</DialogTitle>
          <DialogDescription>
            Choose a name and a daily watch limit. Use 0 for unlimited.
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-4 py-1">
          <div className="space-y-2">
            <Label htmlFor="add-youtube-category-name">Name</Label>
            <Input
              id="add-youtube-category-name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g. Learning"
              autoComplete="off"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="add-youtube-category-max">Max / day</Label>
            <Input
              id="add-youtube-category-max"
              type="number"
              min={0}
              className="tabular-nums"
              value={maxVideosPerDayRaw}
              onChange={(e) => setMaxVideosPerDayRaw(e.target.value)}
            />
          </div>
        </div>
        <DialogFooter className="gap-2 sm:gap-0">
          <Button
            type="button"
            variant="outline"
            onClick={() => handleOpenChange(false)}
          >
            Cancel
          </Button>
          <ButtonWithLoading
            type="button"
            onClick={handleSubmit}
            disabled={!name.trim() || !maxOk}
            isLoading={createMutation.isPending}
          >
            Create
          </ButtonWithLoading>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

import { useEffect, useState } from "react";

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

import { useUpdateGuitarPlaylistMutation } from "../mutations/useGuitarPlaylistMutations";

const favoritesName = "favorites";

interface RenamePlaylistDialogProps {
  open: boolean;
  playlistId: string;
  currentName: string;
  onOpenChange: (open: boolean) => void;
}

function RenamePlaylistDialog({
  open,
  playlistId,
  currentName,
  onOpenChange,
}: RenamePlaylistDialogProps) {
  const [name, setName] = useState(currentName);
  const updateMutation = useUpdateGuitarPlaylistMutation();
  const trimmedName = name.trim();
  const isReserved = trimmedName.toLowerCase() === favoritesName;

  useEffect(() => {
    if (open) {
      setName(currentName);
    }
  }, [open, currentName]);

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    if (!trimmedName || isReserved) {
      return;
    }

    updateMutation.mutate(
      { playlistId, name: trimmedName },
      {
        onSuccess: () => {
          onOpenChange(false);
        },
      },
    );
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>Rename playlist</DialogTitle>
          </DialogHeader>
          <div className="grid gap-2 py-4">
            <Label htmlFor="guitar-playlist-rename">Name</Label>
            <Input
              id="guitar-playlist-rename"
              value={name}
              onChange={(event) => setName(event.target.value)}
              maxLength={200}
              autoFocus
            />
            {isReserved ? (
              <p className="text-sm text-destructive">
                Favorites is already a page in this section.
              </p>
            ) : null}
          </div>
          <DialogFooter>
            <ButtonWithLoading
              type="submit"
              disabled={!trimmedName || isReserved || trimmedName === currentName}
              isLoading={updateMutation.isPending}
            >
              Save
            </ButtonWithLoading>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export default RenamePlaylistDialog;

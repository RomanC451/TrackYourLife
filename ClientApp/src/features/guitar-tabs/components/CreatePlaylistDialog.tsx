import { useState } from "react";

import ButtonWithLoading from "@/components/ui/button-with-loading";
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

import { useCreateGuitarPlaylistMutation } from "../mutations/useGuitarPlaylistMutations";

const favoritesName = "favorites";

interface CreatePlaylistDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCreated?: (playlistId: string) => void;
}

function CreatePlaylistDialog({
  open,
  onOpenChange,
  onCreated,
}: CreatePlaylistDialogProps) {
  const [name, setName] = useState("");
  const createMutation = useCreateGuitarPlaylistMutation();
  const trimmedName = name.trim();
  const isReserved = trimmedName.toLowerCase() === favoritesName;

  const handleOpenChange = (nextOpen: boolean) => {
    if (!nextOpen) {
      setName("");
    }
    onOpenChange(nextOpen);
  };

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    if (!trimmedName || isReserved) {
      return;
    }

    createMutation.mutate(trimmedName, {
      onSuccess: (response) => {
        setName("");
        handleOpenChange(false);
        if (response?.id) {
          onCreated?.(response.id);
        }
      },
    });
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent>
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>New playlist</DialogTitle>
            <DialogDescription>
              Name the playlist. It will show up in Guitar Tabs, under Favorites.
            </DialogDescription>
          </DialogHeader>
          <div className="grid gap-2 py-4">
            <Label htmlFor="guitar-playlist-name">Name</Label>
            <Input
              id="guitar-playlist-name"
              value={name}
              onChange={(event) => setName(event.target.value)}
              placeholder="e.g. Fingerstyle"
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
              disabled={!trimmedName || isReserved}
              isLoading={createMutation.isPending}
            >
              Create
            </ButtonWithLoading>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export default CreatePlaylistDialog;

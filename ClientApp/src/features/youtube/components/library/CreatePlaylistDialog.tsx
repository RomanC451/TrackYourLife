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

import { useCreatePlaylistMutation } from "../../mutations/useLibraryPlaylistMutations";
import { suppressYoutubeCardClick } from "../../youtubeClickGuard";

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
  const createMutation = useCreatePlaylistMutation();

  const handleOpenChange = (nextOpen: boolean) => {
    if (!nextOpen) {
      suppressYoutubeCardClick();
    }
    onOpenChange(nextOpen);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;
    createMutation.mutate(name.trim(), {
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
      <DialogContent onCloseAutoFocus={(e) => e.preventDefault()}>
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>New playlist</DialogTitle>
            <DialogDescription>
              Give your playlist a name. You can add videos from the Videos or Search
              pages.
            </DialogDescription>
          </DialogHeader>
          <div className="grid gap-2 py-4">
            <Label htmlFor="playlist-name">Name</Label>
            <Input
              id="playlist-name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g. Watch later"
              maxLength={200}
              autoFocus
            />
          </div>
          <DialogFooter>
            <ButtonWithLoading
              type="submit"
              disabled={!name.trim()}
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

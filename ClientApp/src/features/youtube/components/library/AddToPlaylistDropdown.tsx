import { useState, type ReactNode } from "react";
import { ListPlus, Loader2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { useQuery } from "@tanstack/react-query";

import { cn } from "@/lib/utils";

import { useAddVideoToPlaylistMutation } from "../../mutations/useLibraryPlaylistMutations";
import { youtubeQueryOptions } from "../../queries/youtubeQueries";
import CreatePlaylistDialog from "./CreatePlaylistDialog";

interface AddToPlaylistDropdownProps {
  videoId: string;
  triggerClassName?: string;
  menuContentClassName?: string;
  showLabel?: boolean;
}

function AddToPlaylistDropdown({
  videoId,
  triggerClassName,
  menuContentClassName,
  showLabel = false,
}: AddToPlaylistDropdownProps) {
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const [createOpen, setCreateOpen] = useState(false);
  const { data: playlists, isPending } = useQuery(youtubeQueryOptions.libraryPlaylists());
  const addMutation = useAddVideoToPlaylistMutation();

  const openCreatePlaylistDialog = () => {
    setCreateOpen(true);
  };

  const handleDropdownOpenChange = (open: boolean) => {
    if (!open && createOpen) {
      return;
    }
    setDropdownOpen(open);
  };

  let menuBody: ReactNode;
  if (isPending) {
    menuBody = (
      <div className="flex justify-center py-2">
        <Loader2 className="h-5 w-5 animate-spin text-muted-foreground" />
      </div>
    );
  } else if (playlists && playlists.length > 0) {
    menuBody = playlists.map((p) => (
      <DropdownMenuItem
        key={p.id}
        disabled={addMutation.isPending}
        onSelect={() => {
          addMutation.mutate(
            { playlistId: p.id, videoId },
            { onSuccess: () => setDropdownOpen(false) },
          );
        }}
      >
        {p.name}
      </DropdownMenuItem>
    ));
  } else {
    menuBody = (
      <div className="px-2 py-1.5 text-sm text-muted-foreground">
        No playlists yet.{" "}
        <button
          type="button"
          className="text-primary underline hover:no-underline"
          onClick={openCreatePlaylistDialog}
          onPointerDown={(e) => e.stopPropagation()}
        >
          Create one
        </button>
      </div>
    );
  }

  return (
    <>
    <DropdownMenu open={dropdownOpen} onOpenChange={handleDropdownOpenChange}>
      <DropdownMenuTrigger asChild>
        <Button
          type="button"
          variant="secondary"
          size="sm"
          className={cn("shrink-0 gap-1", triggerClassName)}
          onClick={(e) => e.stopPropagation()}
          onPointerDown={(e) => e.stopPropagation()}
        >
          {addMutation.isPending ? (
            <Loader2 className="h-4 w-4 animate-spin" />
          ) : (
            <ListPlus className="h-4 w-4" />
          )}
          {showLabel ? <span>Playlist</span> : null}
          <span className="sr-only">Add to playlist</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent
        align="end"
        className={cn("w-56", menuContentClassName)}
        onClick={(e) => e.stopPropagation()}
      >
        <DropdownMenuLabel>Add to playlist</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {menuBody}
      </DropdownMenuContent>
    </DropdownMenu>
    <CreatePlaylistDialog
      open={createOpen}
      onOpenChange={setCreateOpen}
      onCreated={() => setDropdownOpen(true)}
    />
    </>
  );
}

export default AddToPlaylistDropdown;

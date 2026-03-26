import type { ReactNode } from "react";
import { Link } from "@tanstack/react-router";
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

import { useAddVideoToPlaylistMutation } from "../../mutations/useLibraryPlaylistMutations";
import { youtubeQueryOptions } from "../../queries/youtubeQueries";

interface AddToPlaylistDropdownProps {
  videoId: string;
}

function AddToPlaylistDropdown({ videoId }: AddToPlaylistDropdownProps) {
  const { data: playlists, isPending } = useQuery(youtubeQueryOptions.libraryPlaylists());
  const addMutation = useAddVideoToPlaylistMutation();

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
        onSelect={() => addMutation.mutate({ playlistId: p.id, videoId })}
      >
        {p.name}
      </DropdownMenuItem>
    ));
  } else {
    menuBody = (
      <div className="px-2 py-1.5 text-sm text-muted-foreground">
        No playlists yet.{" "}
        <Link to="/youtube/library" className="text-primary underline">
          Create one
        </Link>
      </div>
    );
  }

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button
          type="button"
          variant="secondary"
          size="sm"
          className="shrink-0"
          onClick={(e) => e.stopPropagation()}
          onPointerDown={(e) => e.stopPropagation()}
        >
          {addMutation.isPending ? (
            <Loader2 className="h-4 w-4 animate-spin" />
          ) : (
            <ListPlus className="h-4 w-4" />
          )}
          <span className="sr-only">Add to playlist</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-56" onClick={(e) => e.stopPropagation()}>
        <DropdownMenuLabel>Add to playlist</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {menuBody}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}

export default AddToPlaylistDropdown;

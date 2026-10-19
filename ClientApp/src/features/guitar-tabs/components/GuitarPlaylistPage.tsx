import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { useCustomQuery } from "@/hooks/useCustomQuery";

import { useDeleteGuitarPlaylistMutation } from "../mutations/useGuitarPlaylistMutations";
import { useRemoveGuitarTabFromPlaylistMutation } from "../mutations/useSavedGuitarTabMutations";
import { guitarTabsQueryOptions } from "../queries/guitarTabsQueries";
import GuitarTabSongList from "./GuitarTabSongList";
import RenamePlaylistDialog from "./RenamePlaylistDialog";

function GuitarPlaylistPage({ playlistId }: { playlistId: string }) {
  const navigate = useNavigate();
  const [renameOpen, setRenameOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const { query } = useCustomQuery(guitarTabsQueryOptions.playlist(playlistId));
  const { query: songsQuery } = useCustomQuery({
    ...guitarTabsQueryOptions.playlistSongs(playlistId),
    enabled: query.isSuccess,
  });
  const deleteMutation = useDeleteGuitarPlaylistMutation();
  const removeMutation = useRemoveGuitarTabFromPlaylistMutation();

  if (query.isPending) {
    return (
      <PageCard>
        <p className="py-8 text-center text-muted-foreground">Loading playlist...</p>
      </PageCard>
    );
  }

  if (query.isError || !query.data) {
    return (
      <PageCard>
        <PageTitle title="Playlist" />
        <p className="py-8 text-center text-muted-foreground">
          This playlist could not be found.
        </p>
      </PageCard>
    );
  }

  const playlist = query.data;

  return (
    <PageCard>
      <PageTitle title={playlist.name}>
        <Button type="button" variant="outline" onClick={() => setRenameOpen(true)}>
          Rename
        </Button>
        <Button type="button" variant="outline" onClick={() => setDeleteOpen(true)}>
          Delete
        </Button>
      </PageTitle>
      {songsQuery.isPending && (
        <p className="py-8 text-center text-muted-foreground">Loading songs...</p>
      )}
      {songsQuery.isError && (
        <p className="py-8 text-center text-muted-foreground">
          Songs in this playlist could not be loaded.
        </p>
      )}
      {songsQuery.data && (
        <GuitarTabSongList
          songs={songsQuery.data}
          emptyMessage="No tabs in this playlist yet."
          removingId={
            removeMutation.isPending ? removeMutation.variables?.songId : null
          }
          onRemove={(song) => {
            removeMutation.mutate({ playlistId: playlist.id, songId: song.id });
          }}
        />
      )}
      <RenamePlaylistDialog
        open={renameOpen}
        playlistId={playlist.id}
        currentName={playlist.name}
        onOpenChange={setRenameOpen}
      />
      <AlertDialog open={deleteOpen} onOpenChange={setDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete {playlist.name}?</AlertDialogTitle>
            <AlertDialogDescription>
              This removes the playlist from Guitar Tabs.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={() => {
                deleteMutation.mutate(playlist.id, {
                  onSuccess: () => {
                    void navigate({ to: "/guitar-tabs/favorites" });
                  },
                });
              }}
            >
              Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </PageCard>
  );
}

export default GuitarPlaylistPage;

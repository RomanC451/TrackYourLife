import { Link } from "@tanstack/react-router";

import { Button } from "@/components/ui/button";

import type { SavedGuitarTab } from "../api/guitarTabsApi";

function GuitarTabSongList({
  songs,
  emptyMessage,
  onRemove,
  removingId,
}: {
  songs: SavedGuitarTab[];
  emptyMessage: string;
  onRemove?: (song: SavedGuitarTab) => void;
  removingId?: string | null;
}) {
  if (songs.length === 0) {
    return (
      <p className="py-8 text-center text-muted-foreground">{emptyMessage}</p>
    );
  }

  return (
    <ul className="divide-y">
      {songs.map((song) => (
        <li key={song.id} className="flex items-center justify-between gap-4 py-3">
          <div>
            <Link
              to="/guitar-tabs/tab"
              search={{ url: song.tabUrl }}
              className="font-medium hover:underline"
            >
              {song.songName}
            </Link>
            <p className="text-sm text-muted-foreground">
              {song.artistName}
              {" · "}
              {song.type}
            </p>
          </div>
          {onRemove && (
            <Button
              type="button"
              variant="outline"
              size="sm"
              disabled={removingId === song.id}
              onClick={() => onRemove(song)}
            >
              Remove
            </Button>
          )}
        </li>
      ))}
    </ul>
  );
}

export default GuitarTabSongList;

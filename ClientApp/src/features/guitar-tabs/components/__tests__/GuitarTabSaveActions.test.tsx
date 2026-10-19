import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { GuitarTabSaveControls } from "../GuitarTabSaveActions";

const playlists = [
  {
    id: "playlist-1",
    name: "Acoustic",
    createdOnUtc: "2026-10-19T12:00:00Z",
    modifiedOnUtc: null,
  },
];

describe("GuitarTabSaveControls", () => {
  it("hearts a song that is not saved yet", () => {
    const onFavoriteChange = vi.fn();

    render(
      <GuitarTabSaveControls
        isFavorite={false}
        playlists={playlists}
        selectedPlaylistIds={[]}
        pending={false}
        playlistsReady
        onFavoriteChange={onFavoriteChange}
        onPlaylistsChange={vi.fn()}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Add to favorites" }));

    expect(onFavoriteChange).toHaveBeenCalledWith(true);
  });

  it("keeps a playlist song in favorites when the heart is clicked", () => {
    const onFavoriteChange = vi.fn();

    render(
      <GuitarTabSaveControls
        isFavorite
        playlists={playlists}
        selectedPlaylistIds={["playlist-1"]}
        pending={false}
        playlistsReady
        onFavoriteChange={onFavoriteChange}
        onPlaylistsChange={vi.fn()}
      />,
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Remove from favorites" }),
    );

    expect(onFavoriteChange).not.toHaveBeenCalled();
  });

  it("adds the song to the checked playlist", () => {
    const onPlaylistsChange = vi.fn();

    render(
      <GuitarTabSaveControls
        isFavorite={false}
        playlists={playlists}
        selectedPlaylistIds={[]}
        pending={false}
        playlistsReady
        onFavoriteChange={vi.fn()}
        onPlaylistsChange={onPlaylistsChange}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Playlists" }));
    fireEvent.click(screen.getByRole("checkbox", { name: "Acoustic" }));

    expect(onPlaylistsChange).toHaveBeenCalledWith(["playlist-1"]);
  });
});

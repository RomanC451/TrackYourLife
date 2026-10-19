import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import type { SavedGuitarTab } from "../../api/guitarTabsApi";

vi.mock("@tanstack/react-router", () => ({
  Link: ({
    children,
    to,
    search,
  }: {
    children: React.ReactNode;
    to: string;
    search?: { url?: string };
  }) => (
    <a href={`${to}?url=${encodeURIComponent(search?.url ?? "")}`}>{children}</a>
  ),
}));

import GuitarTabSongList from "../GuitarTabSongList";

const song: SavedGuitarTab = {
  id: "song-1",
  songName: "Viu",
  artistName: "Bosquito",
  type: "Chords",
  tabUrl: "https://tabs.ultimate-guitar.com/tab/bosquito/viu-chords-5059138",
};

describe("GuitarTabSongList", () => {
  it("links a saved song to its tab page", () => {
    render(
      <GuitarTabSongList songs={[song]} emptyMessage="No favorite tabs yet." />,
    );

    expect(screen.getByRole("link", { name: "Viu" })).toHaveAttribute(
      "href",
      "/guitar-tabs/tab?url=https%3A%2F%2Ftabs.ultimate-guitar.com%2Ftab%2Fbosquito%2Fviu-chords-5059138",
    );
  });

  it("removes a song from the playlist", () => {
    const onRemove = vi.fn();

    render(
      <GuitarTabSongList
        songs={[song]}
        emptyMessage="No tabs in this playlist yet."
        onRemove={onRemove}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Remove" }));

    expect(onRemove).toHaveBeenCalledWith(song);
  });
});

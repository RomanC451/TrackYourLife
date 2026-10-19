import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import type { GuitarTabContent } from "../../api/guitarTabsApi";
import GuitarTabContentView from "../GuitarTabContent";

const tab: GuitarTabContent = {
  id: 5059138,
  songName: "Viu",
  artistName: "Bosquito",
  type: "Chords",
  tonality: "F#m",
  tuningName: "Standard",
  tuning: "E A D G B E",
  capo: 2,
  rating: 4.55,
  votes: 5,
  content: "[Intro]\n[ch]Am[/ch] [ch]G[/ch]\n[Verse 1]\n[tab]line[/tab]",
  tabUrl: "https://tabs.ultimate-guitar.com/tab/bosquito/viu-chords-5059138",
  chords: [
    {
      name: "Am",
      frets: [0, 1, 2, 2, 0, -1],
      position: 0,
      barreFret: null,
      barreFromString: null,
      barreToString: null,
    },
  ],
  youtubeUrl: null,
};

describe("GuitarTabContentView", () => {
  it("shows the song, chord diagram, section, and chord names", () => {
    render(<GuitarTabContentView tab={tab} />);

    expect(screen.getByRole("heading", { name: "Viu" })).toBeInTheDocument();
    expect(screen.getByText(/Bosquito/)).toBeInTheDocument();
    expect(
      screen.getByText("Key F#m · Capo 2 · Tuning E A D G B E"),
    ).toBeInTheDocument();
    expect(screen.getByText("Intro")).toBeInTheDocument();
    expect(screen.getByText("Verse 1")).toBeInTheDocument();
    expect(screen.getAllByText("Am").length).toBeGreaterThan(0);
    expect(screen.getByText("G")).toBeInTheDocument();
    expect(screen.getByText("line")).toBeInTheDocument();
  });
});

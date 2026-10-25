import { fireEvent, render, screen } from "@testing-library/react";
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
      shapes: [
        {
          frets: [0, 1, 2, 2, 0, -1],
          fingers: [0, 1, 3, 2, 0, 0],
          position: 0,
          barreFret: null,
          barreFromString: null,
          barreToString: null,
          barreFinger: null,
        },
        {
          frets: [5, 5, 6, 7, 7, 5],
          fingers: [1, 1, 2, 3, 4, 1],
          position: 5,
          barreFret: 5,
          barreFromString: 0,
          barreToString: 5,
          barreFinger: 1,
        },
      ],
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
    expect(screen.getByText("1 of 2")).toBeInTheDocument();
    expect(screen.getByText("3", { hidden: true })).toBeInTheDocument();

    expect(document.querySelectorAll("circle.fill-amber-400")).toHaveLength(3);

    fireEvent.click(screen.getByRole("button", { name: "Next Am shape" }));
    expect(screen.getByText("2 of 2")).toBeInTheDocument();
    expect(document.querySelectorAll("circle.fill-amber-400")).toHaveLength(3);
  });
});

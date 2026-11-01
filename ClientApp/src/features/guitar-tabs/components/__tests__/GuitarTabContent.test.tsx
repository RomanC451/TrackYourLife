import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

import { getGuitarTabDevices, type GuitarTabContent } from "../../api/guitarTabsApi";
import GuitarTabContentView from "../GuitarTabContent";

vi.mock("../../api/guitarTabsApi", async () => {
  const actual = await vi.importActual<typeof import("../../api/guitarTabsApi")>(
    "../../api/guitarTabsApi",
  );
  return {
    ...actual,
    getGuitarTabDevices: vi.fn(async () => []),
  };
});

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
  beforeEach(() => {
    localStorage.clear();
    vi.mocked(getGuitarTabDevices).mockResolvedValue([]);
  });

  it("shows the song, chord diagram, section, and chord names", () => {
    render(<GuitarTabContentView tab={tab} />, {
      wrapper: createQueryClientWrapper(),
    });

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
    expect(document.querySelector("figure svg")?.textContent).toContain("3");

    expect(document.querySelectorAll("circle.fill-amber-400")).toHaveLength(3);

    fireEvent.click(screen.getByRole("button", { name: "Next Am shape" }));
    expect(screen.getByText("2 of 2")).toBeInTheDocument();
    expect(document.querySelectorAll("circle.fill-amber-400")).toHaveLength(3);
  });

  it("saves nothing until speed changes, then reset restores the device preset", async () => {
    const deviceId = "11111111-1111-1111-1111-111111111111";
    localStorage.setItem("deviceId", deviceId);
    vi.mocked(getGuitarTabDevices).mockResolvedValue([
      {
        deviceId,
        name: "Laptop",
        speed: 0.2,
        semitones: 0,
        useFlats: false,
      },
    ]);

    render(<GuitarTabContentView tab={tab} />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(await screen.findByRole("combobox", { name: "Device" })).toHaveTextContent(
      "Laptop",
    );
    expect(screen.queryByRole("button", { name: "Save settings" })).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Increase speed" }));

    expect(screen.getByRole("button", { name: "Save settings" })).toBeInTheDocument();
    expect(screen.getByRole("spinbutton", { name: "Speed" })).toHaveAttribute(
      "aria-valuenow",
      "0.3",
    );

    fireEvent.click(screen.getByRole("button", { name: "Reset settings" }));

    expect(screen.getByRole("spinbutton", { name: "Speed" })).toHaveAttribute(
      "aria-valuenow",
      "0.2",
    );
    expect(screen.queryByRole("button", { name: "Save settings" })).not.toBeInTheDocument();
  });
});

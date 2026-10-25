import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

vi.mock("../../lib/videoDuration", () => ({
  rememberVideoDuration: vi.fn(),
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({
    screenSize: { width: 1400 },
    queryToolsRef: { current: null },
    routerToolsRef: { current: null },
  }),
}));

import {
  getGuitarTabPlaybackSettings,
  resetGuitarTabPlaybackSettings,
} from "../../lib/playbackSettings";
import { GuitarTabAudioButton, GuitarTabYoutubePlayer } from "../GuitarTabYoutube";

function openMenu(name: string) {
  const button = screen.getByRole("button", { name });
  fireEvent.keyDown(button, { key: "Enter" });
}

describe("GuitarTabYoutubePlayer", () => {
  beforeEach(() => {
    resetGuitarTabPlaybackSettings();
  });

  it("plays the video linked on the tab", () => {
    render(
      <GuitarTabYoutubePlayer
        songName="Wonderwall"
        artistName="Oasis"
        suggestedUrl="https://www.youtube.com/watch?v=RBumgq5yVrA"
        savedUrl={null}
        canSaveVideo={false}
      />,
    );

    expect(screen.queryByRole("button", { name: "Add audio" })).not.toBeInTheDocument();

    expect(screen.getByTitle("Wonderwall by Oasis").getAttribute("src")).toContain(
      "https://www.youtube.com/embed/RBumgq5yVrA",
    );
    expect(screen.getByTitle("Wonderwall by Oasis").getAttribute("src")).toContain(
      "enablejsapi=1",
    );
    expect(screen.getByTitle("Wonderwall by Oasis")).toHaveAttribute(
      "referrerpolicy",
      "strict-origin-when-cross-origin",
    );
    expect(screen.getByText("Playing the video linked on the tab.")).toBeInTheDocument();
  });

  it("saves a pasted YouTube link and ignores other links", () => {
    const onSave = vi.fn();
    render(
      <GuitarTabAudioButton
        suggestedUrl={null}
        savedUrl={null}
        pending={false}
        onSave={onSave}
      />,
    );

    expect(screen.queryByLabelText("YouTube URL")).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Add audio" }));
    fireEvent.change(screen.getByLabelText("YouTube URL"), {
      target: { value: "https://example.com/song" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));
    expect(onSave).not.toHaveBeenCalled();
    expect(screen.getByText("Enter a YouTube video link.")).toBeInTheDocument();

    fireEvent.change(screen.getByLabelText("YouTube URL"), {
      target: { value: "https://youtu.be/RBumgq5yVrA" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));
    expect(onSave).toHaveBeenCalledWith("https://youtu.be/RBumgq5yVrA");
  });

  it("keeps the player free of the audio button", () => {
    render(
      <GuitarTabYoutubePlayer
        songName="Last Call"
        artistName="Red Leather"
        suggestedUrl={null}
        savedUrl={null}
        canSaveVideo
      />,
    );

    expect(
      screen.getByText(/This tab has no YouTube video/),
    ).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Add audio" })).not.toBeInTheDocument();
  });

  it("edits or removes a saved video from the audio menu", async () => {
    const onSave = vi.fn();
    render(
      <GuitarTabAudioButton
        suggestedUrl="https://www.youtube.com/watch?v=RBumgq5yVrA"
        savedUrl="https://www.youtube.com/watch?v=bx1Bh8ZvH84"
        pending={false}
        onSave={onSave}
      />,
    );

    expect(screen.queryByRole("button", { name: "Add audio" })).not.toBeInTheDocument();

    openMenu("Audio");
    fireEvent.click(screen.getByRole("menuitem", { name: "Edit audio" }));
    expect(
      await screen.findByRole("dialog", { name: "Edit audio" }),
    ).toBeInTheDocument();
    expect(screen.getByLabelText("YouTube URL")).toHaveValue(
      "https://www.youtube.com/watch?v=bx1Bh8ZvH84",
    );

    fireEvent.click(screen.getByRole("button", { name: "Close" }));
    openMenu("Audio");
    const sync = screen.getByRole("menuitemcheckbox", { name: "Sync audio" });
    expect(sync).toHaveAttribute("aria-checked", "true");
    fireEvent.click(sync);
    expect(getGuitarTabPlaybackSettings().syncVideo).toBe(false);
    expect(sync).toHaveAttribute("aria-checked", "false");
    expect(screen.getByRole("button", { name: "Audio" })).toHaveAttribute(
      "aria-expanded",
      "true",
    );

    fireEvent.click(screen.getByRole("menuitem", { name: "Remove audio" }));
    expect(onSave).toHaveBeenCalledWith(null);
  });

  it("uses the saved video instead of the tab video", () => {
    render(
      <GuitarTabYoutubePlayer
        songName="Wonderwall"
        artistName="Oasis"
        suggestedUrl="https://www.youtube.com/watch?v=RBumgq5yVrA"
        savedUrl="https://www.youtube.com/watch?v=bx1Bh8ZvH84"
        canSaveVideo
      />,
    );

    expect(screen.getByTitle("Wonderwall by Oasis").getAttribute("src")).toContain(
      "https://www.youtube.com/embed/bx1Bh8ZvH84",
    );
    expect(screen.queryByRole("button", { name: "Audio" })).not.toBeInTheDocument();
  });
});

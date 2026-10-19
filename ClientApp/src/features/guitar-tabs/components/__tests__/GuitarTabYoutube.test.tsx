import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { GuitarTabYoutubePlayer } from "../GuitarTabYoutube";

describe("GuitarTabYoutubePlayer", () => {
  it("plays the video linked on the tab", () => {
    render(
      <GuitarTabYoutubePlayer
        songName="Wonderwall"
        artistName="Oasis"
        suggestedUrl="https://www.youtube.com/watch?v=RBumgq5yVrA"
        savedUrl={null}
        pending={false}
        canSaveVideo={false}
        onSave={vi.fn()}
      />,
    );

    expect(screen.queryByRole("button", { name: "Save video" })).not.toBeInTheDocument();

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
      <GuitarTabYoutubePlayer
        songName="Last Call"
        artistName="Red Leather"
        suggestedUrl={null}
        savedUrl={null}
        pending={false}
        canSaveVideo
        onSave={onSave}
      />,
    );

    expect(
      screen.getByText(/This tab has no YouTube video/),
    ).toBeInTheDocument();

    fireEvent.change(screen.getByLabelText("YouTube URL"), {
      target: { value: "https://example.com/song" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Save video" }));
    expect(onSave).not.toHaveBeenCalled();
    expect(screen.getByText("Enter a YouTube video link.")).toBeInTheDocument();

    fireEvent.change(screen.getByLabelText("YouTube URL"), {
      target: { value: "https://youtu.be/RBumgq5yVrA" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Save video" }));
    expect(onSave).toHaveBeenCalledWith("https://youtu.be/RBumgq5yVrA");
  });

  it("uses the saved video instead of the tab video", () => {
    render(
      <GuitarTabYoutubePlayer
        songName="Wonderwall"
        artistName="Oasis"
        suggestedUrl="https://www.youtube.com/watch?v=RBumgq5yVrA"
        savedUrl="https://www.youtube.com/watch?v=bx1Bh8ZvH84"
        pending={false}
        canSaveVideo
        onSave={vi.fn()}
      />,
    );

    expect(screen.getByTitle("Wonderwall by Oasis").getAttribute("src")).toContain(
      "https://www.youtube.com/embed/bx1Bh8ZvH84",
    );
    expect(screen.getByRole("button", { name: "Remove video" })).toBeInTheDocument();
  });
});

import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("../../components/dialogs/VideoPlayerDialog", () => ({
  default: ({ videoId }: { videoId: string }) => (
    <div data-testid="video-player-dialog">{videoId}</div>
  ),
}));

import {
  useYoutubePlayerHost,
  YoutubePlayerHostProvider,
} from "../YoutubePlayerHostContext";

function TestConsumer() {
  const { openYoutubePlayer, closeYoutubePlayer, minimizeYoutubePlayer, playerState } =
    useYoutubePlayerHost();

  return (
    <div>
      <span data-testid="video-id">{playerState?.videoId ?? "none"}</span>
      <button type="button" onClick={() => openYoutubePlayer({ videoId: "video-1" })}>
        Open
      </button>
      <button type="button" onClick={minimizeYoutubePlayer}>
        Minimize
      </button>
      <button type="button" onClick={closeYoutubePlayer}>
        Close
      </button>
    </div>
  );
}

describe("YoutubePlayerHostContext", () => {
  it("opens, minimizes, and closes the player", () => {
    render(
      <YoutubePlayerHostProvider>
        <TestConsumer />
      </YoutubePlayerHostProvider>,
    );

    fireEvent.click(screen.getByRole("button", { name: "Open" }));
    expect(screen.getByTestId("video-id")).toHaveTextContent("video-1");
    expect(screen.getByTestId("video-player-dialog")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Minimize" }));
    expect(screen.getByText(/Player minimized/i)).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Close" }));
    expect(screen.getByTestId("video-id")).toHaveTextContent("none");
  });

  it("throws outside the provider", () => {
    expect(() => render(<TestConsumer />)).toThrow(
      /must be used within YoutubePlayerHostProvider/i,
    );
  });
});

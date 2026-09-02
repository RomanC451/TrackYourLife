import { fireEvent, render, screen, within } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

const { mockNavigate, mockPathname } = vi.hoisted(() => ({
  mockNavigate: vi.fn(),
  mockPathname: { current: "/youtube/videos/watch/video-1" },
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
  useLocation: () => ({ pathname: mockPathname.current }),
}));

vi.mock("../../components/dialogs/VideoPlayerDialog", () => ({
  default: ({
    videoId,
    isVisible,
  }: {
    videoId: string;
    isVisible?: boolean;
  }) => (
    <div data-testid="video-player-dialog" data-visible={isVisible !== false}>
      {videoId}
    </div>
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
  beforeEach(() => {
    vi.clearAllMocks();
    mockPathname.current = "/youtube/videos/watch/video-1";
  });

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
    expect(screen.getByTestId("video-player-dialog")).toHaveAttribute(
      "data-visible",
      "false",
    );

    fireEvent.click(screen.getByRole("button", { name: "Close" }));
    expect(screen.getByTestId("video-id")).toHaveTextContent("none");
  });

  it("leaves the watch URL when minimized so navigation cannot reopen it", () => {
    render(
      <YoutubePlayerHostProvider>
        <TestConsumer />
      </YoutubePlayerHostProvider>,
    );

    fireEvent.click(screen.getByRole("button", { name: "Open" }));
    fireEvent.click(screen.getByRole("button", { name: "Minimize" }));

    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/youtube/videos",
      search: expect.any(Function),
      replace: true,
    });
  });

  it("restores the player from the minimized bar", () => {
    render(
      <YoutubePlayerHostProvider>
        <TestConsumer />
      </YoutubePlayerHostProvider>,
    );

    fireEvent.click(screen.getByRole("button", { name: "Open" }));
    fireEvent.click(screen.getByRole("button", { name: "Minimize" }));

    const minimizedBar = screen.getByText(/Player minimized/i).closest("div");
    expect(minimizedBar).not.toBeNull();
    fireEvent.click(within(minimizedBar!).getByRole("button", { name: "Open" }));

    expect(screen.queryByText(/Player minimized/i)).not.toBeInTheDocument();
    expect(screen.getByTestId("video-player-dialog")).toHaveAttribute(
      "data-visible",
      "true",
    );
  });

  it("throws outside the provider", () => {
    expect(() => render(<TestConsumer />)).toThrow(
      /must be used within YoutubePlayerHostProvider/i,
    );
  });
});

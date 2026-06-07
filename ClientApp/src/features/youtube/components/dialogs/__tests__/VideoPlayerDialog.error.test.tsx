import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => vi.fn(),
}));

vi.mock("../../../playback/usePlayVideoMutation", () => ({
  default: () => ({
    mutate: vi.fn(),
    isPending: false,
    isError: true,
    error: { response: { data: { detail: "Limit reached" } } },
  }),
}));

import VideoPlayerDialog from "../VideoPlayerDialog";

describe("VideoPlayerDialog errors", () => {
  it("shows an error state when playback fails", () => {
    render(<VideoPlayerDialog videoId="video-1" />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByText("Error Loading Video")).toBeInTheDocument();
    expect(screen.getByText("Limit reached")).toBeInTheDocument();
  });

  it("closes from the error footer and backdrop", () => {
    const onClose = vi.fn();

    render(<VideoPlayerDialog videoId="video-1" onClose={onClose} />, {
      wrapper: createQueryClientWrapper(),
    });

    fireEvent.click(screen.getAllByRole("button", { name: /^Close$/i })[1]);
    expect(onClose).toHaveBeenCalledTimes(1);
  });
});

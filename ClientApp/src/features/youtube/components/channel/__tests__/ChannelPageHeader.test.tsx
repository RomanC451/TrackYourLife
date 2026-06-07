import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { youtubeChannel } from "@/features/youtube/__tests__/fixtures";

vi.mock("../ChannelHeaderActions", () => ({
  default: () => <div data-testid="channel-header-actions" />,
}));

import ChannelPageHeader from "../ChannelPageHeader";

describe("ChannelPageHeader", () => {
  it("renders channel details with thumbnail", () => {
    render(
      <ChannelPageHeader
        youtubeChannelId="ch-1"
        channelName="Fitness Channel"
        thumbnailUrl="https://example.com/avatar.jpg"
        subscribedChannel={youtubeChannel("ch-1")}
      />,
    );

    expect(screen.getByText("Fitness Channel")).toBeInTheDocument();
    expect(screen.getByRole("img")).toHaveAttribute("src", "https://example.com/avatar.jpg");
    expect(screen.getByText("Fitness")).toBeInTheDocument();
    expect(screen.getByTestId("channel-header-actions")).toBeInTheDocument();
  });

  it("falls back to an initial when no thumbnail exists", () => {
    render(
      <ChannelPageHeader
        youtubeChannelId="ch-1"
        channelName="alpha"
        subscribedChannel={undefined}
      />,
    );

    expect(screen.getByText("A")).toBeInTheDocument();
  });
});

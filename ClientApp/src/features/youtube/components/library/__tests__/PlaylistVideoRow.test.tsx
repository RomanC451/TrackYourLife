import { fireEvent, render, screen } from "@testing-library/react";

import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";



import { youtubeVideo } from "@/features/youtube/__tests__/fixtures";

import type { YoutubePlaylistVideoItemDto } from "@/services/openapi";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

function playlistVideoItem(youtubeId: string): YoutubePlaylistVideoItemDto {
  return {
    id: `item-${youtubeId}`,
    youtubeId,
    addedOnUtc: "2026-06-01T00:00:00Z",
    isLoading: false,
    isDeleting: false,
  };
}



const { mockUseQuery } = vi.hoisted(() => ({

  mockUseQuery: vi.fn(),

}));



vi.mock("@tanstack/react-query", async (importOriginal) => {

  const actual = await importOriginal<typeof import("@tanstack/react-query")>();

  return {

    ...actual,

    useQuery: (...args: unknown[]) => mockUseQuery(...args),

  };

});



import PlaylistVideoRow from "../PlaylistVideoRow";



describe("PlaylistVideoRow", () => {

  beforeEach(() => {
    vi.clearAllMocks();
    mockUseQuery.mockReturnValue({
      data: undefined,
      isPending: false,
      isError: false,
    });
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });



  afterEach(() => {

    vi.useRealTimers();

  });



  it("renders preview data and handles actions", () => {

    const onWatch = vi.fn();

    const onRemove = vi.fn();



    render(

      <PlaylistVideoRow

        item={playlistVideoItem("video-1")}

        preview={youtubeVideo("video-1")}

        onWatch={onWatch}

        onRemove={onRemove}

        playVideoPending={false}

        removeVideoPending={false}

      />,

      { wrapper: createQueryClientWrapper() },

    );



    fireEvent.click(screen.getByRole("button", { name: /^Watch$/i }));

    fireEvent.click(screen.getByRole("button", { name: /Remove/i }));



    expect(screen.getByText("Video video-1")).toBeInTheDocument();

    expect(onWatch).toHaveBeenCalledWith("video-1");

    expect(onRemove).toHaveBeenCalledWith("video-1");

  });



  it("formats view counts and published dates from preview data", () => {

    render(

      <PlaylistVideoRow

        item={playlistVideoItem("video-1")}

        preview={youtubeVideo("video-1", {

          viewCount: 1_500_000,

          publishedAt: "2026-06-05T10:00:00Z",

        })}

        onWatch={vi.fn()}

        onRemove={vi.fn()}

        playVideoPending={false}

        removeVideoPending={false}

      />,

      { wrapper: createQueryClientWrapper() },

    );



    expect(screen.getByText("1.5M")).toBeInTheDocument();

    expect(screen.getByText("Today")).toBeInTheDocument();

  });



  it("shows a loading skeleton while details are fetched", () => {

    mockUseQuery.mockReturnValue({

      data: undefined,

      isPending: true,

      isError: false,

    });



    const { container } = render(

      <PlaylistVideoRow

        item={playlistVideoItem("video-1")}

        onWatch={vi.fn()}

        onRemove={vi.fn()}

        playVideoPending={false}

        removeVideoPending={false}

      />,

      { wrapper: createQueryClientWrapper() },

    );



    expect(container.querySelector(".animate-pulse")).toBeInTheDocument();

  });



  it("shows fallback UI when details fail to load", () => {

    mockUseQuery.mockReturnValue({

      data: undefined,

      isPending: false,

      isError: true,

    });



    const onWatch = vi.fn();

    const onRemove = vi.fn();



    render(

      <PlaylistVideoRow

        item={playlistVideoItem("video-1")}

        onWatch={onWatch}

        onRemove={onRemove}

        playVideoPending={false}

        removeVideoPending={false}

      />,

      { wrapper: createQueryClientWrapper() },

    );



    expect(screen.getByText("video-1")).toBeInTheDocument();

    expect(screen.getByText("Could not load video details")).toBeInTheDocument();



    fireEvent.click(screen.getByRole("button", { name: /^Watch$/i }));

    fireEvent.click(screen.getByRole("button", { name: /^Remove$/i }));



    expect(onWatch).toHaveBeenCalledWith("video-1");

    expect(onRemove).toHaveBeenCalledWith("video-1");

  });



  it("shows a pending spinner on the watch button", () => {

    render(

      <PlaylistVideoRow

        item={playlistVideoItem("video-1")}

        preview={youtubeVideo("video-1")}

        onWatch={vi.fn()}

        onRemove={vi.fn()}

        playVideoPending={true}

        removeVideoPending={false}

      />,

      { wrapper: createQueryClientWrapper() },

    );



    expect(document.querySelectorAll(".animate-spin").length).toBeGreaterThan(0);

  });

});



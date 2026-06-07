import { fireEvent, render, screen } from "@testing-library/react";

import { beforeEach, describe, expect, it, vi } from "vitest";



import { watchHistoryEntry } from "@/features/youtube/__tests__/fixtures";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";



const { mockUseInfiniteQuery } = vi.hoisted(() => ({

  mockUseInfiniteQuery: vi.fn(),

}));



let intersectionCallback: IntersectionObserverCallback | undefined;

let disconnectSpy = vi.fn();



vi.mock("@tanstack/react-query", async (importOriginal) => {

  const actual = await importOriginal<typeof import("@tanstack/react-query")>();

  return {

    ...actual,

    useInfiniteQuery: (...args: unknown[]) => mockUseInfiniteQuery(...args),

  };

});



vi.mock("../WatchHistoryEntryCard", () => ({

  default: ({ entry }: { entry: { videoId: string } }) => (

    <div data-testid="history-entry">{entry.videoId}</div>

  ),

}));



import WatchHistoryList from "../WatchHistoryList";



describe("WatchHistoryList", () => {

  beforeEach(() => {

    vi.clearAllMocks();

    intersectionCallback = undefined;

    disconnectSpy = vi.fn();



    class IntersectionObserverMock {

      constructor(callback: IntersectionObserverCallback) {

        intersectionCallback = callback;

      }

      observe() {}

      unobserve() {}

      disconnect = disconnectSpy;

    }

    vi.stubGlobal("IntersectionObserver", IntersectionObserverMock);

  });



  it("shows loading state", () => {

    mockUseInfiniteQuery.mockReturnValue({ isPending: true });



    const { container } = render(<WatchHistoryList />, {

      wrapper: createQueryClientWrapper(),

    });



    expect(container.querySelector(".animate-spin")).toBeInTheDocument();

  });



  it("shows empty state", () => {

    mockUseInfiniteQuery.mockReturnValue({

      isPending: false,

      isError: false,

      data: { pages: [{ items: [] }] },

      hasNextPage: false,

      isFetchingNextPage: false,

    });



    render(<WatchHistoryList />, { wrapper: createQueryClientWrapper() });



    expect(screen.getByText(/No watch history yet/i)).toBeInTheDocument();

  });



  it("renders history entries", () => {

    mockUseInfiniteQuery.mockReturnValue({

      isPending: false,

      isError: false,

      data: { pages: [{ items: [watchHistoryEntry()] }] },

      hasNextPage: false,

      isFetchingNextPage: false,

    });



    render(<WatchHistoryList />, { wrapper: createQueryClientWrapper() });



    expect(screen.getByTestId("history-entry")).toHaveTextContent("video-1");

    expect(screen.getByText(/You've reached the end/i)).toBeInTheDocument();

  });



  it("shows an error state and retries", () => {

    const refetch = vi.fn();

    mockUseInfiniteQuery.mockReturnValue({

      isPending: false,

      isError: true,

      error: new Error("Network down"),

      refetch,

    });



    render(<WatchHistoryList />, { wrapper: createQueryClientWrapper() });



    expect(screen.getByText("Network down")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /Retry/i }));

    expect(refetch).toHaveBeenCalled();

  });



  it("shows a generic error message for non-Error failures", () => {

    mockUseInfiniteQuery.mockReturnValue({

      isPending: false,

      isError: true,

      error: "failed",

      refetch: vi.fn(),

    });



    render(<WatchHistoryList />, { wrapper: createQueryClientWrapper() });



    expect(

      screen.getByText(/Check your connection and try again/i),

    ).toBeInTheDocument();

  });



  it("fetches the next page when the sentinel intersects", () => {

    const fetchNextPage = vi.fn();

    mockUseInfiniteQuery.mockReturnValue({

      isPending: false,

      isError: false,

      data: { pages: [{ items: [watchHistoryEntry()] }] },

      hasNextPage: true,

      isFetchingNextPage: false,

      fetchNextPage,

    });



    render(<WatchHistoryList />, { wrapper: createQueryClientWrapper() });



    expect(screen.getByText(/Scroll for more/i)).toBeInTheDocument();



    intersectionCallback?.(

      [{ isIntersecting: true } as IntersectionObserverEntry],

      {} as IntersectionObserver,

    );



    expect(fetchNextPage).toHaveBeenCalled();

  });



  it("shows a footer spinner while fetching the next page", () => {

    mockUseInfiniteQuery.mockReturnValue({

      isPending: false,

      isError: false,

      data: { pages: [{ items: [watchHistoryEntry()] }] },

      hasNextPage: true,

      isFetchingNextPage: true,

      fetchNextPage: vi.fn(),

    });



    const { container } = render(<WatchHistoryList />, {

      wrapper: createQueryClientWrapper(),

    });



    expect(container.querySelector(".animate-spin")).toBeInTheDocument();

  });

});



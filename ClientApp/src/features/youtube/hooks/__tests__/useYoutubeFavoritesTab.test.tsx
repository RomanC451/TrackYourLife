import { renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeChannel } from "@/features/youtube/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

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

import useYoutubeFavoritesTab from "../useYoutubeFavoritesTab";

describe("useYoutubeFavoritesTab", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("reports favorite channels when data is available", () => {
    mockUseQuery.mockReturnValue({
      data: [youtubeChannel("ch-1", { isFavorite: true })],
      isPending: false,
    });

    const { result } = renderHook(() => useYoutubeFavoritesTab(true), {
      wrapper: createQueryClientWrapper(),
    });

    expect(result.current.hasFavoriteChannels).toBe(true);
    expect(result.current.isFavoritesTabLoading).toBe(false);
  });

  it("stays idle when disabled", () => {
    mockUseQuery.mockReturnValue({
      data: undefined,
      isPending: true,
    });

    const { result } = renderHook(() => useYoutubeFavoritesTab(false), {
      wrapper: createQueryClientWrapper(),
    });

    expect(result.current.hasFavoriteChannels).toBe(false);
    expect(result.current.isFavoritesTabLoading).toBe(false);
  });
});

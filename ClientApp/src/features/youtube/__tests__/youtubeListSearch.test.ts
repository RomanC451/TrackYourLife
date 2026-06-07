import { renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { youtubeCategory } from "./fixtures";
import {
  listFilterFromSearch,
  sortYoutubeCategoriesByDisplayOrder,
  useSyncYoutubeListSearchParams,
} from "../youtubeListSearch";

describe("sortYoutubeCategoriesByDisplayOrder", () => {
  it("sorts categories by display order", () => {
    const sorted = sortYoutubeCategoriesByDisplayOrder([
      youtubeCategory("cat-1", { displayOrder: 2 }),
      youtubeCategory("cat-2", { displayOrder: 0 }),
      youtubeCategory("cat-3", { displayOrder: 1 }),
    ]);

    expect(sorted.map((c) => c.id)).toEqual([
      "22222222-2222-4222-8222-222222222222",
      "cat-3",
      "11111111-1111-4111-8111-111111111111",
    ]);
  });
});

describe("listFilterFromSearch", () => {
  it("returns null while search params are not ready", () => {
    expect(listFilterFromSearch("all", false)).toBeNull();
    expect(listFilterFromSearch(undefined, false)).toBeNull();
  });

  it("maps known search params to list filters", () => {
    expect(listFilterFromSearch("all", true)).toBe("all");
    expect(listFilterFromSearch("favorites", true)).toBe("favorites");
    expect(listFilterFromSearch("cat-1", true)).toBe("cat-1");
    expect(listFilterFromSearch(undefined, true)).toBeNull();
  });
});

describe("useSyncYoutubeListSearchParams", () => {
  it("redirects stale category ids to all", async () => {
    const navigate = vi.fn();

    renderHook(() =>
      useSyncYoutubeListSearchParams({
        isSettingsSuccess: true,
        categories: [youtubeCategory("cat-1")],
        youtubeCategoryId: "missing",
        navigate,
        base: "/youtube/videos",
      }),
    );

    await waitFor(() => {
      expect(navigate).toHaveBeenCalledWith({
        to: "/youtube/videos",
        search: { youtubeCategoryId: "all" },
        replace: true,
      });
    });
  });

  it("defaults undefined category to the first sorted category", async () => {
    const navigate = vi.fn();

    renderHook(() =>
      useSyncYoutubeListSearchParams({
        isSettingsSuccess: true,
        categories: [
          youtubeCategory("cat-1", { displayOrder: 2 }),
          youtubeCategory("cat-2", { displayOrder: 0 }),
        ],
        youtubeCategoryId: undefined,
        navigate,
        base: "/youtube/channels",
      }),
    );

    await waitFor(() => {
      expect(navigate).toHaveBeenCalledWith({
        to: "/youtube/channels",
        search: {
          youtubeCategoryId: "22222222-2222-4222-8222-222222222222",
        },
        replace: true,
      });
    });
  });

  it("redirects favorites to all when there are no favorite channels", async () => {
    const navigate = vi.fn();

    renderHook(() =>
      useSyncYoutubeListSearchParams({
        isSettingsSuccess: true,
        categories: [youtubeCategory("cat-1")],
        youtubeCategoryId: "favorites",
        hasFavoriteChannels: false,
        navigate,
        base: "/youtube/videos",
      }),
    );

    await waitFor(() => {
      expect(navigate).toHaveBeenCalledWith({
        to: "/youtube/videos",
        search: { youtubeCategoryId: "all" },
        replace: true,
      });
    });
  });

  it("does not navigate while settings are still loading", async () => {
    const navigate = vi.fn();

    renderHook(() =>
      useSyncYoutubeListSearchParams({
        isSettingsSuccess: false,
        categories: [youtubeCategory("cat-1")],
        youtubeCategoryId: undefined,
        navigate,
        base: "/youtube/videos",
      }),
    );

    await waitFor(() => {
      expect(navigate).not.toHaveBeenCalled();
    });
  });

  it("does not navigate when there are no categories", async () => {
    const navigate = vi.fn();

    renderHook(() =>
      useSyncYoutubeListSearchParams({
        isSettingsSuccess: true,
        categories: [],
        youtubeCategoryId: undefined,
        navigate,
        base: "/youtube/videos",
      }),
    );

    await waitFor(() => {
      expect(navigate).not.toHaveBeenCalled();
    });
  });

  it("keeps favorites when favorite channels exist", async () => {
    const navigate = vi.fn();

    renderHook(() =>
      useSyncYoutubeListSearchParams({
        isSettingsSuccess: true,
        categories: [youtubeCategory("cat-1")],
        youtubeCategoryId: "favorites",
        hasFavoriteChannels: true,
        navigate,
        base: "/youtube/videos",
      }),
    );

    await waitFor(() => {
      expect(navigate).not.toHaveBeenCalled();
    });
  });

  it("skips sync when disabled", async () => {
    const navigate = vi.fn();

    renderHook(() =>
      useSyncYoutubeListSearchParams({
        isSettingsSuccess: true,
        categories: [youtubeCategory("cat-1")],
        youtubeCategoryId: "favorites",
        hasFavoriteChannels: false,
        navigate,
        base: "/youtube/videos",
        syncSearch: false,
      }),
    );

    await waitFor(() => {
      expect(navigate).not.toHaveBeenCalled();
    });
  });
});

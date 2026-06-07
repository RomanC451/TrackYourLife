import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const {
  mockCreateYoutubeCategory,
  mockUpdateYoutubeCategoryMetadata,
  mockUpdateYoutubeCategoryLimit,
  mockDeleteYoutubeCategory,
  mockNavigate,
  mockToastError,
} = vi.hoisted(() => ({
  mockCreateYoutubeCategory: vi.fn(),
  mockUpdateYoutubeCategoryMetadata: vi.fn(),
  mockUpdateYoutubeCategoryLimit: vi.fn(),
  mockDeleteYoutubeCategory: vi.fn(),
  mockNavigate: vi.fn(),
  mockToastError: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockSettingsApi {
    createYoutubeCategory = mockCreateYoutubeCategory;
    updateYoutubeCategoryMetadata = mockUpdateYoutubeCategoryMetadata;
    updateYoutubeCategoryLimit = mockUpdateYoutubeCategoryLimit;
    deleteYoutubeCategory = mockDeleteYoutubeCategory;
  }
  return { ...actual, SettingsApi: MockSettingsApi };
});

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("sonner", () => ({
  toast: {
    error: mockToastError,
  },
}));

import {
  useCreateYoutubeCategoryMutation,
  useDeleteYoutubeCategoryMutation,
  useUpdateYoutubeCategoryLimitMutation,
  useUpdateYoutubeCategoryMetadataMutation,
} from "../useYoutubeCategoryMutations";

describe("useYoutubeCategoryMutations", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockCreateYoutubeCategory.mockResolvedValue({ data: undefined });
    mockUpdateYoutubeCategoryMetadata.mockResolvedValue({ data: undefined });
    mockUpdateYoutubeCategoryLimit.mockResolvedValue({ data: undefined });
    mockDeleteYoutubeCategory.mockResolvedValue({ data: undefined });
  });

  it("creates categories", async () => {
    const { result } = renderHook(() => useCreateYoutubeCategoryMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ name: "Learning", maxVideosPerDay: 3 });
    });

    expect(mockCreateYoutubeCategory).toHaveBeenCalledWith({
      name: "Learning",
      maxVideosPerDay: 3,
    });
  });

  it("updates category metadata and limits", async () => {
    const metadataHook = renderHook(() => useUpdateYoutubeCategoryMetadataMutation(), {
      wrapper: createQueryClientWrapper(),
    });
    const limitHook = renderHook(() => useUpdateYoutubeCategoryLimitMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await metadataHook.result.current.mutateAsync({
        id: "cat-1",
        body: { name: "Fitness", displayOrder: 0 },
      });
      await limitHook.result.current.mutateAsync({
        id: "cat-1",
        body: { maxVideosPerDay: 10 },
      });
    });

    expect(mockUpdateYoutubeCategoryMetadata).toHaveBeenCalledWith("cat-1", {
      name: "Fitness",
      displayOrder: 0,
    });
    expect(mockUpdateYoutubeCategoryLimit).toHaveBeenCalledWith("cat-1", {
      maxVideosPerDay: 10,
    });
  });

  it("deletes categories", async () => {
    const { result } = renderHook(() => useDeleteYoutubeCategoryMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        id: "cat-1",
        confirmUnsubscribeChannels: true,
        moveChannelsToCategoryId: "cat-2",
      });
    });

    expect(mockDeleteYoutubeCategory).toHaveBeenCalledWith("cat-1", true, "cat-2");
  });

  it("shows a Pro upgrade toast when category creation is forbidden", async () => {
    mockCreateYoutubeCategory.mockRejectedValue({
      response: {
        status: 403,
        data: { type: "Youtube.Category.ForbiddenForPlan" },
      },
    });

    const { result } = renderHook(() => useCreateYoutubeCategoryMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ name: "Learning", maxVideosPerDay: 3 }).catch(() => undefined);
    });

    expect(mockToastError).toHaveBeenCalledWith(
      "This action requires Pro",
      expect.objectContaining({
        action: expect.objectContaining({ label: "Get Pro" }),
      }),
    );

    const toastArgs = mockToastError.mock.calls[0]?.[1] as {
      action?: { onClick?: () => void };
    };
    toastArgs.action?.onClick?.();
    expect(mockNavigate).toHaveBeenCalledWith({ to: "/upgrade" });
  });

  it("shows API detail when category creation fails", async () => {
    mockCreateYoutubeCategory.mockRejectedValue({
      response: { status: 400, data: { detail: "Name taken" } },
    });

    const { result } = renderHook(() => useCreateYoutubeCategoryMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ name: "Learning", maxVideosPerDay: 3 }).catch(() => undefined);
    });

    expect(mockToastError).toHaveBeenCalledWith("Name taken");
  });

  it("shows an error toast when updating category limits fails", async () => {
    mockUpdateYoutubeCategoryLimit.mockRejectedValue({
      response: { status: 400, data: { detail: "Limit too high" } },
    });

    const { result } = renderHook(() => useUpdateYoutubeCategoryLimitMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current
        .mutateAsync({ id: "cat-1", body: { maxVideosPerDay: 99 } })
        .catch(() => undefined);
    });

    expect(mockToastError).toHaveBeenCalledWith("Limit too high");
  });
});

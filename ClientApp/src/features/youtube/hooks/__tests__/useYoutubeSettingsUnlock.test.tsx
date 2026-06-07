import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockMutateAsync } = vi.hoisted(() => ({
  mockMutateAsync: vi.fn(),
}));

vi.mock("../../mutations/useYoutubeSettingsPasswordMutations", () => ({
  useVerifyYoutubeSettingsPasswordMutation: () => ({
    mutateAsync: mockMutateAsync,
    isPending: false,
    error: null,
    reset: vi.fn(),
  }),
}));

import { useYoutubeSettingsUnlock } from "../useYoutubeSettingsUnlock";

describe("useYoutubeSettingsUnlock", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockMutateAsync.mockResolvedValue(undefined);
  });

  it("starts unlocked when no settings password exists", () => {
    const { result } = renderHook(() => useYoutubeSettingsUnlock(false), {
      wrapper: createQueryClientWrapper(),
    });

    expect(result.current.isUnlocked).toBe(true);
  });

  it("verifies the password before unlocking", async () => {
    const { result } = renderHook(() => useYoutubeSettingsUnlock(true), {
      wrapper: createQueryClientWrapper(),
    });

    expect(result.current.isUnlocked).toBe(false);

    await act(async () => {
      await result.current.unlock("secret");
    });

    expect(mockMutateAsync).toHaveBeenCalledWith({ password: "secret" });
    expect(result.current.isUnlocked).toBe(true);
  });
});

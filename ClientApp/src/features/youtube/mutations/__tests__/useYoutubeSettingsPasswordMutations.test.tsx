import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const {
  mockVerifyYoutubeSettingsPassword,
  mockSetYoutubeSettingsPassword,
  mockResetYoutubeSettingsPasswordViaEmail,
  mockHandleApiError,
  mockToastSuccess,
  mockToastError,
} = vi.hoisted(() => ({
  mockVerifyYoutubeSettingsPassword: vi.fn(),
  mockSetYoutubeSettingsPassword: vi.fn(),
  mockResetYoutubeSettingsPasswordViaEmail: vi.fn(),
  mockHandleApiError: vi.fn(),
  mockToastSuccess: vi.fn(),
  mockToastError: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockSettingsApi {
    verifyYoutubeSettingsPassword = mockVerifyYoutubeSettingsPassword;
    setYoutubeSettingsPassword = mockSetYoutubeSettingsPassword;
    resetYoutubeSettingsPasswordViaEmail = mockResetYoutubeSettingsPasswordViaEmail;
  }
  return { ...actual, SettingsApi: MockSettingsApi };
});

vi.mock("@/services/openapi/handleApiError", () => ({
  handleApiError: mockHandleApiError,
}));

vi.mock("sonner", () => ({
  toast: {
    success: mockToastSuccess,
    error: mockToastError,
  },
}));

import {
  useResetYoutubeSettingsPasswordViaEmailMutation,
  useSetYoutubeSettingsPasswordMutation,
  useVerifyYoutubeSettingsPasswordMutation,
} from "../useYoutubeSettingsPasswordMutations";

describe("useYoutubeSettingsPasswordMutations", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockVerifyYoutubeSettingsPassword.mockResolvedValue({ data: undefined });
    mockSetYoutubeSettingsPassword.mockResolvedValue({ data: undefined });
    mockResetYoutubeSettingsPasswordViaEmail.mockResolvedValue({ data: undefined });
  });

  it("verifies settings passwords", async () => {
    const { result } = renderHook(() => useVerifyYoutubeSettingsPasswordMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ password: "Aa1!aaaaaa" });
    });

    expect(mockVerifyYoutubeSettingsPassword).toHaveBeenCalledWith({
      password: "Aa1!aaaaaa",
    });
  });

  it("sets settings passwords", async () => {
    const { result } = renderHook(() => useSetYoutubeSettingsPasswordMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ newPassword: "Aa1!aaaaaa" });
    });

    expect(mockSetYoutubeSettingsPassword).toHaveBeenCalledWith({
      newPassword: "Aa1!aaaaaa",
    });
  });

  it("sends reset email on success", async () => {
    const { result } = renderHook(
      () => useResetYoutubeSettingsPasswordViaEmailMutation(),
      { wrapper: createQueryClientWrapper() },
    );

    await act(async () => {
      await result.current.mutateAsync();
    });

    expect(mockResetYoutubeSettingsPasswordViaEmail).toHaveBeenCalled();
    expect(mockToastSuccess).toHaveBeenCalledWith(
      "Check your email for a new settings password",
    );
  });

  it("delegates verify failures to handleApiError", async () => {
    const error = { response: { status: 401, data: { detail: "Wrong password" } } };
    mockVerifyYoutubeSettingsPassword.mockRejectedValue(error);

    const { result } = renderHook(() => useVerifyYoutubeSettingsPasswordMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ password: "bad" }).catch(() => undefined);
    });

    expect(mockHandleApiError).toHaveBeenCalledWith({ error });
  });

  it("maps reset email rate limit errors", async () => {
    const error = {
      response: {
        status: 429,
        data: { type: "Youtube.Settings.ResetEmailRateLimited" },
      },
    };
    mockResetYoutubeSettingsPasswordViaEmail.mockRejectedValue(error);
    mockHandleApiError.mockImplementation(({ defaultHandler }) => defaultHandler?.());

    const { result } = renderHook(
      () => useResetYoutubeSettingsPasswordViaEmailMutation(),
      { wrapper: createQueryClientWrapper() },
    );

    await act(async () => {
      await result.current.mutateAsync().catch(() => undefined);
    });

    expect(mockToastError).toHaveBeenCalledWith(
      "A reset email was sent recently. Please wait a few minutes and try again.",
    );
  });
});

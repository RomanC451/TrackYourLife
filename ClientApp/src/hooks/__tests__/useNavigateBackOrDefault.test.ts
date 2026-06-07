import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import useNavigateBackOrDefault from "../useNavigateBackOrDefault";

const mockBack = vi.fn();
const mockNavigate = vi.fn();
const mockCanGoBack = vi.fn();

vi.mock("@tanstack/react-router", () => ({
  useRouter: () => ({
    history: {
      canGoBack: mockCanGoBack,
      back: mockBack,
    },
  }),
  useNavigate: () => mockNavigate,
}));

describe("useNavigateBackOrDefault", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("navigates back when history allows it", () => {
    mockCanGoBack.mockReturnValue(true);

    const { result } = renderHook(() =>
      useNavigateBackOrDefault({ to: "/home" }),
    );

    act(() => {
      result.current();
    });

    expect(mockBack).toHaveBeenCalledTimes(1);
    expect(mockNavigate).not.toHaveBeenCalled();
  });

  it("navigates to the fallback route when history cannot go back", () => {
    mockCanGoBack.mockReturnValue(false);

    const { result } = renderHook(() =>
      useNavigateBackOrDefault({ to: "/home" }),
    );

    act(() => {
      result.current();
    });

    expect(mockNavigate).toHaveBeenCalledWith({ to: "/home" });
    expect(mockBack).not.toHaveBeenCalled();
  });
});

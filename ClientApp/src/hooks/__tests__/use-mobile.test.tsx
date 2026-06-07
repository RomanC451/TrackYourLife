import { act, renderHook } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { useIsMobile } from "../use-mobile";

describe("useIsMobile", () => {
  let changeHandler: (() => void) | undefined;

  beforeEach(() => {
    changeHandler = undefined;

    Object.defineProperty(window, "innerWidth", {
      writable: true,
      configurable: true,
      value: 500,
    });

    window.matchMedia = vi.fn().mockImplementation(() => ({
      matches: true,
      media: "(max-width: 767px)",
      addEventListener: (_event: string, handler: () => void) => {
        changeHandler = handler;
      },
      removeEventListener: vi.fn(),
    })) as typeof window.matchMedia;
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("returns true when the viewport is below the mobile breakpoint", () => {
    const { result } = renderHook(() => useIsMobile());

    expect(result.current).toBe(true);
  });

  it("updates when the viewport changes", () => {
    const { result } = renderHook(() => useIsMobile());

    Object.defineProperty(window, "innerWidth", {
      writable: true,
      configurable: true,
      value: 1200,
    });

    act(() => {
      changeHandler?.();
    });

    expect(result.current).toBe(false);
  });
});

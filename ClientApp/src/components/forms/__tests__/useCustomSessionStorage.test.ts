import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it } from "vitest";

import useCustomSessionStorage from "../useCustomSessionStorage";

describe("useCustomSessionStorage", () => {
  beforeEach(() => {
    sessionStorage.clear();
  });

  it("returns undefined session data when no key is provided", () => {
    const { result } = renderHook(() => useCustomSessionStorage());

    expect(result.current.sessionData).toBeUndefined();
    expect(result.current.isDirty).toBe(false);
  });

  it("initializes from session storage when a key is provided", () => {
    sessionStorage.setItem("form-key", JSON.stringify({ name: "saved" }));

    const { result } = renderHook(() =>
      useCustomSessionStorage<{ name: string }>("form-key"),
    );

    expect(result.current.sessionData).toEqual({ name: "saved" });
  });

  it("syncs queryData into session storage when it changes", () => {
    const { result, rerender } = renderHook(
      ({ queryData }: { queryData?: { name: string } }) =>
        useCustomSessionStorage("sync-key", queryData),
      { initialProps: { queryData: { name: "server" } } },
    );

    expect(result.current.sessionData).toEqual({ name: "server" });
    expect(sessionStorage.getItem("sync-key")).toBe(
      JSON.stringify({ name: "server" }),
    );

    rerender({ queryData: { name: "updated" } });

    expect(result.current.sessionData).toEqual({ name: "updated" });
    expect(sessionStorage.getItem("sync-key")).toBe(
      JSON.stringify({ name: "updated" }),
    );
  });

  it("marks the session as dirty when it differs from queryData", () => {
    const { result } = renderHook(() =>
      useCustomSessionStorage("dirty-key", { name: "server" }),
    );

    act(() => {
      result.current.setSessionData({ name: "edited" });
    });

    expect(result.current.isDirty).toBe(true);
  });
});

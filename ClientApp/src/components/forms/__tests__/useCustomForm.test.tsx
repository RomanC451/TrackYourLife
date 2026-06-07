import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { z } from "zod";

import useCustomForm from "../useCustomForm";

const formSchema = z.object({
  name: z.string().min(1, "Name is required"),
});

describe("useCustomForm", () => {
  beforeEach(() => {
    sessionStorage.clear();
    vi.clearAllMocks();
  });

  it("initializes with default values", () => {
    const onSubmit = vi.fn();

    const { result } = renderHook(() =>
      useCustomForm({
        formSchema,
        defaultValues: { name: "default" },
        onSubmit,
      }),
    );

    expect(result.current.form.getValues()).toEqual({ name: "default" });
  });

  it("restores values from session storage when a key is provided", () => {
    sessionStorage.setItem("exercise-form", JSON.stringify({ name: "saved" }));

    const { result } = renderHook(() =>
      useCustomForm({
        formSchema,
        defaultValues: { name: "" },
        onSubmit: vi.fn(),
        sessionStorageKey: "exercise-form",
      }),
    );

    expect(result.current.form.getValues()).toEqual({ name: "saved" });
  });

  it("submits valid form data", async () => {
    const onSubmit = vi.fn().mockResolvedValue(undefined);

    const { result } = renderHook(() =>
      useCustomForm({
        formSchema,
        defaultValues: { name: "valid" },
        onSubmit,
      }),
    );

    const event = {
      preventDefault: vi.fn(),
    } as unknown as React.FormEvent<HTMLFormElement>;

    await act(async () => {
      result.current.handleCustomSubmit(event);
    });

    await waitFor(() => {
      expect(onSubmit).toHaveBeenCalledWith(
        { name: "valid" },
        expect.anything(),
      );
    });
  });

  it("clears session storage via resetSessionStorage", () => {
    sessionStorage.setItem("exercise-form", JSON.stringify({ name: "saved" }));

    const { result } = renderHook(() =>
      useCustomForm({
        formSchema,
        defaultValues: { name: "" },
        onSubmit: vi.fn(),
        sessionStorageKey: "exercise-form",
      }),
    );

    act(() => {
      result.current.form.resetSessionStorage();
    });

    expect(sessionStorage.getItem("exercise-form")).toBe("undefined");
  });

  it("resets form values when there is no session storage data", () => {
    const { result } = renderHook(() =>
      useCustomForm({
        formSchema,
        defaultValues: { name: "" },
        onSubmit: vi.fn(),
        sessionStorageKey: "empty-form",
      }),
    );

    act(() => {
      result.current.form.setDataIfNoSessionStorage({ name: "loaded" });
    });

    expect(result.current.form.getValues()).toEqual({ name: "loaded" });
  });
});

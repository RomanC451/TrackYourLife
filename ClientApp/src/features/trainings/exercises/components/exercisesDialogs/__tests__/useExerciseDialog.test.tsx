import { act, renderHook } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { Difficulty } from "@/services/openapi";

import useExerciseDialog from "../useExerciseDialog";

describe("useExerciseDialog", () => {
  const defaultValues = {
    id: "ex-1",
    name: "Bench press",
    muscleGroups: ["chest"],
    difficulty: Difficulty.Easy,
    description: "",
    equipment: "Barbell",
    videoUrl: "",
    pictureUrl: "",
    exerciseSets: [
      {
        id: "set-1",
        name: "Set 1",
        orderIndex: 0,
        count1: 10,
        unit1: "reps",
      },
    ],
  };

  it("submits form data through the mutation", async () => {
    const mutate = vi.fn((_vars, opts) => opts?.onSuccess?.(undefined, _vars));
    const onSuccess = vi.fn();
    const setTab = vi.fn();

    const { result } = renderHook(() =>
      useExerciseDialog({
        onSuccess,
        mutation: { mutate } as never,
        defaultValues,
        setTab,
      }),
    );

    const event = {
      preventDefault: vi.fn(),
    } as unknown as React.FormEvent<HTMLFormElement>;

    await act(async () => {
      result.current.handleCustomSubmit(event);
    });

    expect(mutate).toHaveBeenCalledWith(
      expect.objectContaining({
        request: expect.objectContaining({ name: "Bench press" }),
        id: "ex-1",
      }),
      expect.any(Object),
    );
    expect(onSuccess).toHaveBeenCalledWith(
      expect.objectContaining({ name: "Bench press" }),
    );
  });

  it("switches tab on validation errors", async () => {
    const mutate = vi.fn((vars) => {
      vars.setError("exerciseSets", { message: "Invalid sets" });
      vars.setError("name", { message: "Required" });
    });
    const setTab = vi.fn();

    const { result } = renderHook(() =>
      useExerciseDialog({
        onSuccess: vi.fn(),
        mutation: { mutate } as never,
        defaultValues,
        setTab,
      }),
    );

    const event = {
      preventDefault: vi.fn(),
    } as unknown as React.FormEvent<HTMLFormElement>;

    await act(async () => {
      result.current.handleCustomSubmit(event);
    });

    expect(setTab).toHaveBeenCalledWith("sets");
    expect(setTab).toHaveBeenCalledWith("details");
  });
});

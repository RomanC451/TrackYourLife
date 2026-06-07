import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { exercise } from "@/features/trainings/__tests__/fixtures";
import { Difficulty } from "@/services/openapi";

import useTrainingDialog from "../useTrainingDialog";

describe("useTrainingDialog", () => {
  const defaultValues = {
    id: "training-1",
    name: "Push day",
    muscleGroups: ["chest"],
    difficulty: Difficulty.Easy,
    description: "",
    duration: 60,
    restSeconds: 60,
    exercises: [exercise("ex-1", { name: "Bench press" })],
  };

  beforeEach(() => {
    sessionStorage.clear();
    vi.clearAllMocks();
  });

  it("submits mapped training data through the mutation", async () => {
    const mutate = vi.fn((_vars, opts) => opts?.onSuccess?.());
    const onSuccess = vi.fn();
    const setTab = vi.fn();

    const { result } = renderHook(() =>
      useTrainingDialog({
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
        id: "training-1",
        request: expect.objectContaining({
          name: "Push day",
          exercisesIds: ["ex-1"],
        }),
      }),
      expect.any(Object),
    );
    expect(onSuccess).toHaveBeenCalledOnce();
  });

  it("switches tab on validation errors", async () => {
    const mutate = vi.fn((vars) => {
      vars.setError("exercises", { message: "Required" });
      vars.setError("name", { message: "Required" });
    });
    const setTab = vi.fn();

    const { result } = renderHook(() =>
      useTrainingDialog({
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

    expect(setTab).toHaveBeenCalledWith("exercises");
    expect(setTab).toHaveBeenCalledWith("details");
  });
});

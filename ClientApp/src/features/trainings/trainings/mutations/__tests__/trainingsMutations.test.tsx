import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { training } from "@/features/trainings/__tests__/fixtures";
import { queryClient } from "@/queryClient";
import { Difficulty, type TrainingDto } from "@/services/openapi";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

import { trainingsQueryKeys } from "../../queries/trainingsQueries";

const { mockCreateTraining, mockUpdateTraining, mockDeleteTraining } =
  vi.hoisted(() => ({
    mockCreateTraining: vi.fn(),
    mockUpdateTraining: vi.fn(),
    mockDeleteTraining: vi.fn(),
  }));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockTrainingsApi {
    createTraining = mockCreateTraining;
    updateTraining = mockUpdateTraining;
    deleteTraining = mockDeleteTraining;
  }
  return { ...actual, TrainingsApi: MockTrainingsApi };
});

vi.mock("uuid", () => ({ v4: () => "new-training-id" }));

import useCreateTrainingMutation from "../useCreateTrainingMutation";
import useDeleteTrainingMutation from "../useDeleteTrainingMutation";
import useUpdateTrainingMutation from "../useUpdateTrainingMutation";

const request = {
  name: "Leg day",
  muscleGroups: ["legs"],
  difficulty: Difficulty.Medium,
  duration: 45,
  restSeconds: 90,
  exercisesIds: ["ex-1"],
};

describe("trainings mutations", () => {
  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
    queryClient.setQueryData(trainingsQueryKeys.all, [
      training("a", { name: "Alpha" }),
      training("b", { name: "Beta" }),
    ]);
    mockCreateTraining.mockResolvedValue({ data: undefined });
    mockUpdateTraining.mockResolvedValue({ data: undefined });
    mockDeleteTraining.mockResolvedValue({ data: undefined });
  });

  it("appends an optimistic training on create", async () => {
    const { result } = renderHook(() => useCreateTrainingMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        id: undefined,
        request,
        setError: vi.fn(),
      });
    });

    const cached = queryClient.getQueryData<TrainingDto[]>(trainingsQueryKeys.all)!;
    expect(cached).toHaveLength(3);
    expect(cached.find((t) => t.name === "Leg day")).toMatchObject({
      name: "Leg day",
      isLoading: true,
    });
  });

  it("updates the cached training on success", async () => {
    const { result } = renderHook(() => useUpdateTrainingMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        id: "b",
        request: { ...request, name: "Beta updated" },
        setError: vi.fn(),
      });
    });

    await waitFor(() => {
      expect(
        queryClient.getQueryData<TrainingDto[]>(trainingsQueryKeys.all)?.find((t) => t.id === "b")
          ?.name,
      ).toBe("Beta updated");
    });
  });

  it("removes the training from the cache on delete", async () => {
    const { result } = renderHook(() => useDeleteTrainingMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ id: "a", name: "Alpha" });
    });

    const cached = queryClient.getQueryData<TrainingDto[]>(trainingsQueryKeys.all)!;
    expect(cached.map((t) => t.id)).toEqual(["b"]);
  });
});

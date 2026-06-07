import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockUpdateOngoingTraining } = vi.hoisted(() => ({
  mockUpdateOngoingTraining: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockOngoingTrainingsApi {
    updateOngoingTraining = mockUpdateOngoingTraining;
  }
  return { ...actual, OngoingTrainingsApi: MockOngoingTrainingsApi };
});

import useUpdateOngoingTrainingMutation from "../useUpdateOngoingTrainingMutation";

describe("useUpdateOngoingTrainingMutation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUpdateOngoingTraining.mockResolvedValue({ data: undefined });
  });

  it("updates an ongoing training", async () => {
    const { result } = renderHook(() => useUpdateOngoingTrainingMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        ongoingTrainingId: "ongoing-1",
        request: { caloriesBurned: 400, durationMinutes: 45 },
      });
    });

    expect(mockUpdateOngoingTraining).toHaveBeenCalledWith("ongoing-1", {
      caloriesBurned: 400,
      durationMinutes: 45,
    });
  });
});

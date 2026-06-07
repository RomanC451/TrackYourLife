import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { exercise, ongoingTraining, workoutHistory } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockUseQuery } = vi.hoisted(() => ({
  mockUseQuery: vi.fn(),
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({
    screenSize: { width: 1400 },
    queryToolsRef: { current: null },
    routerToolsRef: { current: null },
  }),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useQuery: (...args: unknown[]) => mockUseQuery(...args),
  };
});

import { WorkoutSessionDetailsDialog } from "../WorkoutSessionDetailsDialog";

describe("WorkoutSessionDetailsDialog", () => {
  const workout = workoutHistory("wh-1", {
    trainingName: "Push day",
    finishedOnUtc: "2026-06-05T18:30:00Z",
    durationSeconds: 3600,
    caloriesBurned: 420,
  });

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders workout summary and completed exercises", () => {
    mockUseQuery.mockReturnValue({
      data: ongoingTraining({
        completedExerciseIds: ["ex-1"],
        training: {
          ...ongoingTraining().training,
          exercises: [exercise("ex-1", { name: "Bench press" })],
        },
      }),
      isPending: false,
      isError: false,
    });

    const onClose = vi.fn();
    render(
      <WorkoutSessionDetailsDialog workout={workout} onClose={onClose} />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.getByText("Push day")).toBeInTheDocument();
    expect(screen.getByText(/420/)).toBeInTheDocument();
    expect(screen.getByText("Exercises completed (1)")).toBeInTheDocument();
    expect(screen.getByText("Bench press")).toBeInTheDocument();

    fireEvent.click(
      screen.getAllByRole("button", { name: "Close" }).at(-1)!,
    );
    expect(onClose).toHaveBeenCalledOnce();
  });

  it("shows error state with retry", () => {
    const refetch = vi.fn();
    mockUseQuery.mockReturnValue({
      isPending: false,
      isError: true,
      error: new Error("Network error"),
      refetch,
    });

    render(
      <WorkoutSessionDetailsDialog workout={workout} onClose={vi.fn()} />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(
      screen.getByText("Could not load session details"),
    ).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Try again" }));
    expect(refetch).toHaveBeenCalledOnce();
  });
});

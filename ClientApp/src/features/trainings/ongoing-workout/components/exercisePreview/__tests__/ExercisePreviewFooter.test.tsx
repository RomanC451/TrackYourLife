import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { ongoingTraining } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockNavigate, mockNextMutate, mockSkipMutate } = vi.hoisted(() => ({
  mockNavigate: vi.fn(),
  mockNextMutate: vi.fn(),
  mockSkipMutate: vi.fn(),
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("../../../mutations/useNextOngoingTrainingMutation", () => ({
  default: () => ({
    mutate: mockNextMutate,
    isPending: false,
    isDelayedPending: false,
  }),
}));

vi.mock("../../../mutations/useSkipExerciseMutation", () => ({
  default: () => ({
    mutate: mockSkipMutate,
    isPending: false,
    isDelayedPending: false,
  }),
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({
    screenSize: { width: 1400 },
  }),
}));

vi.mock(
  "@/features/trainings/common/components/workoutTimer/WorkoutTimerContext",
  () => ({
    useWorkoutTimerContext: () => ({
      isTimerPlaying: false,
      startTimer: vi.fn(),
    }),
  }),
);

vi.mock("../../CancelTrainingAlertDialog", () => ({
  default: () => <div data-testid="cancel-dialog" />,
}));

vi.mock("../../exerciseSelection/ExerciseSelectionDialog", () => ({
  default: () => <div data-testid="selection-dialog" />,
}));

vi.mock("../ExerciseOverviewDialog", () => ({
  default: () => <div data-testid="overview-dialog" />,
}));

vi.mock(
  "@/features/trainings/exercises/components/common/ExerciseStatsDialog",
  () => ({
    default: () => <div data-testid="stats-dialog" />,
  }),
);

vi.mock("@/features/trainings/overview/components/ExerciseHistoriesDialog", () => ({
  default: () => <div data-testid="histories-dialog" />,
}));

import ExercisePreviewFooter from "../ExercisePreviewFooter";

describe("ExercisePreviewFooter", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockNextMutate.mockImplementation((_vars, opts) => opts?.onSuccess?.());
  });

  it("advances to next set", () => {
    render(
      <ExercisePreviewFooter ongoingTraining={ongoingTraining()} />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: /Next set/i }));
    expect(mockNextMutate).toHaveBeenCalled();
  });

  it("navigates to adjust exercise on last set", () => {
    render(
      <ExercisePreviewFooter
        ongoingTraining={ongoingTraining({ isLastSet: true })}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: /Next exercise/i }));
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/trainings/ongoing-workout/adjust-exercise/$exerciseId",
      params: { exerciseId: "ex-1" },
    });
  });

  it("shows finish workout when all exercises are done", () => {
    render(
      <ExercisePreviewFooter
        ongoingTraining={ongoingTraining({
          completedExerciseIds: ["ex-1", "ex-2"],
          skippedExerciseIds: [],
        })}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: /Finish Workout/i }));
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/trainings/ongoing-workout/finish-workout-confirmation/$ongoingTrainingId",
      params: { ongoingTrainingId: "ongoing-1" },
    });
  });
});

import { fireEvent, render, screen } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { training, workoutPlan } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockUseSuspenseQuery, mockUseQuery, mockNavigate, mockCreateMutate } =
  vi.hoisted(() => ({
    mockUseSuspenseQuery: vi.fn(),
    mockUseQuery: vi.fn(),
    mockNavigate: vi.fn(),
    mockCreateMutate: vi.fn(),
  }));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
    useQuery: (...args: unknown[]) => mockUseQuery(...args),
  };
});

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock(
  "@/features/trainings/ongoing-workout/mutations/useCreateOngoingTrainingMutation",
  () => ({
    default: () => ({
      mutate: mockCreateMutate,
      isPending: false,
      isDelayedPending: false,
    }),
  }),
);

import WorkoutPlansTopSection from "../WorkoutPlansTopSection";

function setupQueries({
  activePlan = workoutPlan("plan-1", {
    name: "PPL Split",
    isActive: true,
    workouts: [
      training("w-1", { name: "Push Day" }),
      training("w-2", { name: "Pull Day" }),
      training("w-3", { name: "Leg Day" }),
      training("w-4", { name: "Rest" }),
    ],
  }),
  nextWorkout = training("w-2", { name: "Pull Day", muscleGroups: ["back"] }),
  workoutsGoal = { value: 4 },
  isGoalPending = false,
}: {
  activePlan?: ReturnType<typeof workoutPlan> | null;
  nextWorkout?: ReturnType<typeof training> | null;
  workoutsGoal?: { value: number } | null;
  isGoalPending?: boolean;
} = {}) {
  mockUseSuspenseQuery
    .mockReturnValueOnce({ data: activePlan })
    .mockReturnValueOnce({ data: nextWorkout })
    .mockReturnValueOnce({ data: { totalWorkoutsCompleted: 8 } })
    .mockReturnValueOnce({ data: { totalWorkoutsCompleted: 2 } })
    .mockReturnValueOnce({ data: { dayStreak: 5 } });
  mockUseQuery.mockReturnValue({
    data: workoutsGoal,
    isPending: isGoalPending,
  });
}

describe("WorkoutPlansTopSection", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
    mockCreateMutate.mockImplementation((_vars, opts) => opts?.onSuccess?.());
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("renders empty state when no active plan", () => {
    setupQueries({ activePlan: null });

    render(<WorkoutPlansTopSection />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByText("No active workout plan")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Create plan" }));
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/trainings/workouts/plan/create",
    });
  });

  it("renders active plan stats and next workout", () => {
    setupQueries();

    render(<WorkoutPlansTopSection />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByText("PPL Split")).toBeInTheDocument();
    expect(screen.getByText("2/4 workouts")).toBeInTheDocument();
    expect(screen.getByText("5")).toBeInTheDocument();
    expect(screen.getByText("8")).toBeInTheDocument();
    expect(screen.getByText("Up Next")).toBeInTheDocument();
    expect(screen.getAllByText("Pull Day").length).toBeGreaterThan(0);
    expect(screen.getByText("Workout Schedule")).toBeInTheDocument();
  });

  it("starts next workout and navigates", () => {
    setupQueries();

    render(<WorkoutPlansTopSection />, {
      wrapper: createQueryClientWrapper(),
    });

    fireEvent.click(screen.getByRole("button", { name: /Start Workout/i }));
    expect(mockCreateMutate).toHaveBeenCalledWith(
      { trainingId: "w-2" },
      expect.objectContaining({ onSuccess: expect.any(Function) }),
    );
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/trainings/ongoing-workout",
    });
  });

  it("shows weekly target overlay when goal is unset", () => {
    setupQueries({ workoutsGoal: null, isGoalPending: false });

    render(<WorkoutPlansTopSection />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(
      screen.getByText("Set your weekly workout target"),
    ).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Set weekly target" }));
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/trainings/workouts/workouts-goal",
    });
  });
});

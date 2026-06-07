import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { exercise, exerciseSet, ongoingTraining } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const {
  mockAdjustMutate,
  mockNextMutate,
  mockNavigate,
  mockFetchQuery,
  mockUseSuspenseQuery,
} = vi.hoisted(() => ({
  mockAdjustMutate: vi.fn(),
  mockNextMutate: vi.fn(),
  mockNavigate: vi.fn(),
  mockFetchQuery: vi.fn(),
  mockUseSuspenseQuery: vi.fn(),
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

vi.mock("../../../mutations/useAdjustExerciseMutation", () => ({
  default: () => ({
    mutate: mockAdjustMutate,
    pendingState: { isPending: false, isDelayedPending: false },
  }),
}));

vi.mock("../../../mutations/useNextOngoingTrainingMutation", () => ({
  default: () => ({
    mutate: mockNextMutate,
    pendingState: { isPending: false, isDelayedPending: false },
  }),
}));

vi.mock("@/queryClient", () => ({
  queryClient: {
    fetchQuery: mockFetchQuery,
  },
}));

import ExerciseSetChangeForm from "../ExerciseSetChangeForm";

describe("ExerciseSetChangeForm", () => {
  const bench = exercise("ex-1", {
    name: "Bench press",
    exerciseSets: [
      exerciseSet("set-1", { name: "Set 1", count1: 10, unit1: "reps" }),
      exerciseSet("set-2", {
        name: "Set 2",
        count1: 60,
        unit1: "kg",
        count2: 8,
        unit2: "reps",
      }),
    ],
  });

  const defaultValues = {
    newSets: bench.exerciseSets.map((set) => ({ ...set })),
  };

  beforeEach(() => {
    vi.clearAllMocks();
    mockUseSuspenseQuery.mockReturnValue({
      data: ongoingTraining({ hasNext: true }),
    });
    mockAdjustMutate.mockImplementation((_vars, opts) => opts?.onSuccess?.());
    mockNextMutate.mockImplementation((_vars, opts) => opts?.onSuccess?.());
    mockFetchQuery.mockResolvedValue(ongoingTraining({ hasNext: true }));
  });

  it("renders set fields and shows modified badge after changes", () => {
    render(
      <ExerciseSetChangeForm defaultValues={defaultValues} exercise={bench} />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.getByText("Set 1")).toBeInTheDocument();
    expect(screen.getByText("Set 2")).toBeInTheDocument();

    const repsInput = screen.getAllByRole("spinbutton")[0];
    fireEvent.change(repsInput, { target: { value: "12" } });
    expect(screen.getByText("Modified")).toBeInTheDocument();
  });

  it("resets a modified set to original values", () => {
    render(
      <ExerciseSetChangeForm defaultValues={defaultValues} exercise={bench} />,
      { wrapper: createQueryClientWrapper() },
    );

    const repsInput = screen.getAllByRole("spinbutton")[0];
    fireEvent.change(repsInput, { target: { value: "12" } });

    const resetButton = screen.getAllByRole("button", { name: "Reset" })[0];
    expect(resetButton).toBeEnabled();
    fireEvent.click(resetButton);
    expect(screen.queryByText("Modified")).not.toBeInTheDocument();
  });

  it("submits adjustments and navigates to ongoing workout when more sets remain", async () => {
    render(
      <ExerciseSetChangeForm defaultValues={defaultValues} exercise={bench} />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: "Save adjustments" }));

    await waitFor(() => {
      expect(mockAdjustMutate).toHaveBeenCalled();
      expect(mockNextMutate).toHaveBeenCalled();
      expect(mockNavigate).toHaveBeenCalledWith({
        to: "/trainings/ongoing-workout",
      });
    });
  });

  it("navigates to finish confirmation when workout has no next step", async () => {
    mockFetchQuery.mockResolvedValue(ongoingTraining({ hasNext: false }));

    render(
      <ExerciseSetChangeForm defaultValues={defaultValues} exercise={bench} />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: "Save adjustments" }));

    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith({
        to: "/trainings/ongoing-workout/finish-workout-confirmation/$ongoingTrainingId",
        params: { ongoingTrainingId: "ongoing-1" },
      });
    });
  });

  it("navigates to finish when active session has no next without advancing", async () => {
    mockUseSuspenseQuery.mockReturnValue({
      data: ongoingTraining({ hasNext: false }),
    });

    render(
      <ExerciseSetChangeForm defaultValues={defaultValues} exercise={bench} />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: "Save adjustments" }));

    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith({
        to: "/trainings/ongoing-workout/finish-workout-confirmation/$ongoingTrainingId",
        params: { ongoingTrainingId: "ongoing-1" },
      });
      expect(mockNextMutate).not.toHaveBeenCalled();
    });
  });

  it("falls back to ongoing workout route when fetch fails", async () => {
    mockFetchQuery.mockRejectedValue(new Error("network"));

    render(
      <ExerciseSetChangeForm defaultValues={defaultValues} exercise={bench} />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: "Save adjustments" }));

    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith({
        to: "/trainings/ongoing-workout",
      });
    });
  });
});

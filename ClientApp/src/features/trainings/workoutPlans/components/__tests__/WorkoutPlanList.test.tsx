import { fireEvent, render, screen } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { training, workoutPlan } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockNavigate, mockDeleteMutate, mockUpdateMutate } = vi.hoisted(() => ({
  mockNavigate: vi.fn(),
  mockDeleteMutate: vi.fn(),
  mockUpdateMutate: vi.fn(),
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock(
  "@/features/trainings/workoutPlans/mutations/useDeleteWorkoutPlanMutation",
  () => ({
    default: () => ({ mutate: mockDeleteMutate, isPending: false }),
  }),
);

vi.mock(
  "@/features/trainings/workoutPlans/mutations/useUpdateWorkoutPlanMutation",
  () => ({
    default: () => ({ mutate: mockUpdateMutate, isPending: false }),
  }),
);

import WorkoutPlanList from "../WorkoutPlanList";

describe("WorkoutPlanList", () => {
  const plans = [
    workoutPlan("plan-1", {
      name: "PPL",
      isActive: true,
      workouts: [training("w-1"), training("w-2")],
      createdOnUtc: "2026-06-05T10:00:00Z",
    }),
    workoutPlan("plan-2", {
      name: "Upper/Lower",
      isActive: false,
      workouts: [training("w-3")],
    }),
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("renders plans with active badge and workout counts", () => {
    render(<WorkoutPlanList plans={plans} />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByText("PPL")).toBeInTheDocument();
    expect(screen.getByText("Active")).toBeInTheDocument();
    expect(screen.getByText("2 workouts")).toBeInTheDocument();
    expect(screen.getByText("Upper/Lower")).toBeInTheDocument();
  });

  it("navigates to edit plan on row click", () => {
    render(<WorkoutPlanList plans={plans} />, {
      wrapper: createQueryClientWrapper(),
    });

    fireEvent.click(screen.getByText("Upper/Lower"));
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/trainings/workouts/plan/edit/$planId",
      params: { planId: "plan-2" },
    });
  });

  it("navigates to create plan", () => {
    render(<WorkoutPlanList plans={plans} />, {
      wrapper: createQueryClientWrapper(),
    });

    fireEvent.click(screen.getByRole("button", { name: /Create new plan/i }));
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/trainings/workouts/plan/create",
    });
  });
});

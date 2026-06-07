import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { exercise } from "@/features/trainings/__tests__/fixtures";

vi.mock("@/features/trainings/common/components/workoutTimer/WorkoutTimer", () => ({
  default: () => <div data-testid="workout-timer" />,
}));

vi.mock("../../adjustmentsHistory/AdjustmentsHistory", () => ({
  default: ({ exerciseId }: { exerciseId: string }) => (
    <div data-testid={`adjustments-history-${exerciseId}`} />
  ),
}));

vi.mock("../ExerciseSetChangeForm", () => ({
  default: ({ exercise }: { exercise: { name: string } }) => (
    <div data-testid="exercise-set-change-form">{exercise.name}</div>
  ),
}));

import AdjustExercise from "../AdjustExercise";

describe("AdjustExercise", () => {
  it("renders the exercise details and child sections", () => {
    const benchPress = exercise("ex-1", { name: "Bench press" });

    render(<AdjustExercise exercise={benchPress} />);

    expect(screen.getByTestId("workout-timer")).toBeInTheDocument();
    expect(screen.getByTestId("adjustments-history-ex-1")).toBeInTheDocument();
    expect(
      screen.getByRole("heading", { name: "Bench press" }),
    ).toBeInTheDocument();
    expect(
      screen.getByText("Adjust sets for next workout session"),
    ).toBeInTheDocument();
    expect(screen.getByTestId("exercise-set-change-form")).toBeInTheDocument();
  });
});

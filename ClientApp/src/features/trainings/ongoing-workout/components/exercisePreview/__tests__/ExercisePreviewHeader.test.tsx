import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { exercise, ongoingTraining } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockUseSuspenseQuery } = vi.hoisted(() => ({
  mockUseSuspenseQuery: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

import ExercisePreviewHeader from "../ExercisePreviewHeader";

describe("ExercisePreviewHeader", () => {
  beforeEach(() => {
    mockUseSuspenseQuery.mockReturnValue({
      data: ongoingTraining({
        exerciseIndex: 1,
        setIndex: 1,
        completedExerciseIds: ["ex-1"],
        training: {
          ...ongoingTraining().training,
          exercises: [
            exercise("ex-1", { name: "Bench press", muscleGroups: ["chest"] }),
            exercise("ex-2", {
              name: "Squat",
              muscleGroups: ["legs"],
              exerciseSets: [
                { id: "s-1", name: "Set 1", orderIndex: 0, count1: 5, unit1: "reps" },
                { id: "s-2", name: "Set 2", orderIndex: 1, count1: 5, unit1: "reps" },
                { id: "s-3", name: "Set 3", orderIndex: 2, count1: 5, unit1: "reps" },
              ],
            }),
          ],
        },
      }),
    });
  });

  it("renders exercise progress and set indicators", () => {
    render(<ExercisePreviewHeader />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByText("Squat")).toBeInTheDocument();
    expect(screen.getByText("legs")).toBeInTheDocument();
    expect(screen.getByText("Exercise 2 of 2")).toBeInTheDocument();
    expect(screen.getByText("50% complete")).toBeInTheDocument();
    expect(screen.getAllByText(/Set \d/)).toHaveLength(3);
  });
});

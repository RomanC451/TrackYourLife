import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

import { exercise, exerciseSet, ongoingTraining } from "@/features/trainings/__tests__/fixtures";

import ExerciseOverviewDialog from "../ExerciseOverviewDialog";

describe("ExerciseOverviewDialog", () => {
  const session = ongoingTraining({
    setIndex: 1,
    training: {
      ...ongoingTraining().training,
      exercises: [
        exercise("ex-1", {
          name: "Bench press",
          equipment: "Barbell",
          exerciseSets: [
            exerciseSet("s-1", { name: "warmup", count1: 20, unit1: "kg" }),
            exerciseSet("s-2", {
              name: "working set",
              count1: 60,
              unit1: "kg",
              count2: 8,
              unit2: "reps",
            }),
          ],
        }),
      ],
    },
  });

  it("renders set overview with current and completed states", () => {
    render(
      <ExerciseOverviewDialog
        open
        onOpenChange={vi.fn()}
        ongoingTraining={session}
      />,
    );

    expect(
      screen.getByText("Bench press - Exercise Overview"),
    ).toBeInTheDocument();
    expect(screen.getByText("Equipment: Barbell")).toBeInTheDocument();
    expect(screen.getByText("Set 2 of 2")).toBeInTheDocument();
    expect(screen.getByText("Completed")).toBeInTheDocument();
    expect(screen.getByText("Current")).toBeInTheDocument();
    expect(screen.getByText("60 kg")).toBeInTheDocument();
    expect(screen.getByText("8 reps")).toBeInTheDocument();
  });
});

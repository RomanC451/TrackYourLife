import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

import { exercise, exerciseSet } from "@/features/trainings/__tests__/fixtures";

vi.mock("@/features/trainings/utils/muscleGroupWorkoutIcon", () => ({
  MuscleGroupWorkoutIcon: () => <div data-testid="muscle-icon" />,
}));

vi.mock("@/components/image-with-spinner", () => ({
  ImageWithSpinner: ({ alt }: { alt: string }) => (
    <img alt={alt} data-testid="exercise-image" />
  ),
}));

vi.mock("@/components/video-player-with-loading", () => ({
  default: ({ url }: { url: string }) => (
    <div data-testid="video-player">{url}</div>
  ),
}));

import ShowExerciseInfoDialog from "../ShowExerciseInfoDialog";

describe("ShowExerciseInfoDialog", () => {
  it("renders full exercise details", () => {
    const onClose = vi.fn();
    const bench = exercise("ex-1", {
      name: "Bench press",
      description: "Chest exercise",
      equipment: "Barbell",
      muscleGroups: ["chest", "triceps"],
      pictureUrl: "https://example.com/bench.jpg",
      videoUrl: "https://example.com/bench.mp4",
      modifiedOnUtc: "2026-02-01T09:00:00Z",
      exerciseSets: [
        exerciseSet("set-1", {
          name: "working set",
          count1: 10,
          unit1: "reps",
          count2: 60,
          unit2: "kg",
        }),
      ],
    });

    render(<ShowExerciseInfoDialog exercise={bench} onClose={onClose} />);

    expect(screen.getByText("Bench press")).toBeInTheDocument();
    expect(screen.getAllByText("Barbell").length).toBeGreaterThan(0);
    expect(screen.getByText("Chest exercise")).toBeInTheDocument();
    expect(screen.getByText("chest, triceps")).toBeInTheDocument();
    expect(screen.getByTestId("exercise-image")).toBeInTheDocument();
    expect(screen.getByTestId("video-player")).toHaveTextContent(
      "https://example.com/bench.mp4",
    );
    expect(screen.getByText(/10 reps/)).toBeInTheDocument();
    expect(screen.getByText(/Modified:/)).toBeInTheDocument();
  });

  it("renders fallbacks for missing optional fields", () => {
    render(
      <ShowExerciseInfoDialog
        exercise={exercise("ex-2", {
          name: "Push-up",
          description: undefined,
          equipment: undefined,
          muscleGroups: [],
          pictureUrl: undefined,
          videoUrl: undefined,
          exerciseSets: [],
        })}
        onClose={vi.fn()}
      />,
    );

    expect(screen.getAllByText("No equipment").length).toBeGreaterThan(0);
    expect(screen.getByText("No description available")).toBeInTheDocument();
    expect(screen.queryByTestId("exercise-image")).not.toBeInTheDocument();
    expect(screen.queryByTestId("video-player")).not.toBeInTheDocument();
  });

  it("calls onClose when dialog is dismissed", () => {
    const onClose = vi.fn();

    render(
      <ShowExerciseInfoDialog
        exercise={exercise("ex-1")}
        onClose={onClose}
      />,
    );

    fireEvent.keyDown(document, { key: "Escape" });
    expect(onClose).toHaveBeenCalled();
  });
});

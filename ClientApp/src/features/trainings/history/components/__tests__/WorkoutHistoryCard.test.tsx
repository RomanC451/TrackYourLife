import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { workoutHistory } from "@/features/trainings/__tests__/fixtures";

import { WorkoutHistoryCard } from "../WorkoutHistoryCard";

describe("WorkoutHistoryCard", () => {
  it("opens session details and shows the newest badge", () => {
    const onViewSessionDetails = vi.fn();
    render(
      <WorkoutHistoryCard
        workout={workoutHistory("session-1", {
          trainingName: "Leg day",
          caloriesBurned: 420,
        })}
        muscleGroups={["legs"]}
        isNewest
        onViewSessionDetails={onViewSessionDetails}
      />,
    );

    expect(screen.getByText("Most recent")).toBeInTheDocument();
    expect(screen.getByText("Leg day")).toBeInTheDocument();
    fireEvent.click(screen.getByText("Leg day"));
    expect(onViewSessionDetails).toHaveBeenCalledWith(
      expect.objectContaining({ id: "session-1" }),
    );
  });
});

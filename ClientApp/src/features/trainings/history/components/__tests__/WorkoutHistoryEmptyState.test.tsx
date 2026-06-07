import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { WorkoutHistoryEmptyState } from "../WorkoutHistoryEmptyState";

describe("WorkoutHistoryEmptyState", () => {
  it("renders the empty state and starts a workout on click", () => {
    const onStartWorkout = vi.fn();
    render(<WorkoutHistoryEmptyState onStartWorkout={onStartWorkout} />);

    expect(screen.getByText("No workouts in this period")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Start a workout" }));
    expect(onStartWorkout).toHaveBeenCalledOnce();
  });
});

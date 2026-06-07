import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import WorkoutTimer from "../WorkoutTimer";
import { useWorkoutTimerContext } from "../WorkoutTimerContext";

vi.mock("../WorkoutTimerContext", () => ({
  useWorkoutTimerContext: vi.fn(),
}));

describe("WorkoutTimer", () => {
  it("renders the countdown and skip action while time remains", () => {
    const stopTimer = vi.fn();
    vi.mocked(useWorkoutTimerContext).mockReturnValue({
      secondsLeft: 30,
      stopTimer,
      progress: 50,
      isTimerPlaying: true,
      startTimer: vi.fn(),
    });

    render(<WorkoutTimer />);

    expect(screen.getByText("Rest Timer")).toBeInTheDocument();
    expect(screen.getByText("30 sec")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: /skip/i }));
    expect(stopTimer).toHaveBeenCalledOnce();
  });

  it("hides the timer panel when the countdown reaches zero", () => {
    vi.mocked(useWorkoutTimerContext).mockReturnValue({
      secondsLeft: 0,
      stopTimer: vi.fn(),
      progress: 100,
      isTimerPlaying: false,
      startTimer: vi.fn(),
    });

    const { container } = render(<WorkoutTimer />);

    expect(screen.queryByText("Rest Timer")).not.toBeInTheDocument();
    expect(container.querySelector("audio")).toBeTruthy();
  });
});

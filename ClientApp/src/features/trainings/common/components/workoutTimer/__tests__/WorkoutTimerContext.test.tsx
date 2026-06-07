import { act, fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { training } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockUseSuspenseQuery, mockSetTimerStartedAt } = vi.hoisted(() => ({
  mockUseSuspenseQuery: vi.fn(),
  mockSetTimerStartedAt: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

vi.mock("usehooks-ts", () => ({
  useLocalStorage: () => [undefined, mockSetTimerStartedAt],
}));

import {
  useWorkoutTimerContext,
  WorkoutTimerContextProvider,
} from "../WorkoutTimerContext";

function TimerConsumer() {
  const { secondsLeft, isTimerPlaying, startTimer, stopTimer } =
    useWorkoutTimerContext();

  return (
    <div>
      <span data-testid="seconds-left">{secondsLeft}</span>
      <span data-testid="is-playing">{String(isTimerPlaying)}</span>
      <button type="button" onClick={startTimer}>
        Start
      </button>
      <button type="button" onClick={stopTimer}>
        Stop
      </button>
    </div>
  );
}

describe("WorkoutTimerContext", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseSuspenseQuery.mockReturnValue({
      data: {
        id: "ongoing-1",
        training: training("training-1", { restSeconds: 60 }),
        exerciseIndex: 0,
        setIndex: 0,
        startedOnUtc: "2026-06-05T10:00:00Z",
        isLoading: false,
        isDeleting: false,
      },
    });
  });

  it("starts and stops the rest timer", () => {
    render(
      <WorkoutTimerContextProvider>
        <TimerConsumer />
      </WorkoutTimerContextProvider>,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.getByTestId("seconds-left")).toHaveTextContent("0");
    expect(screen.getByTestId("is-playing")).toHaveTextContent("false");

    act(() => {
      fireEvent.click(screen.getByRole("button", { name: "Start" }));
    });

    expect(screen.getByTestId("seconds-left")).toHaveTextContent("60");
    expect(screen.getByTestId("is-playing")).toHaveTextContent("true");
    expect(mockSetTimerStartedAt).toHaveBeenCalledWith(expect.any(Date));

    act(() => {
      fireEvent.click(screen.getByRole("button", { name: "Stop" }));
    });

    expect(screen.getByTestId("seconds-left")).toHaveTextContent("0");
    expect(screen.getByTestId("is-playing")).toHaveTextContent("false");
    expect(mockSetTimerStartedAt).toHaveBeenCalledWith(undefined);
  });
});

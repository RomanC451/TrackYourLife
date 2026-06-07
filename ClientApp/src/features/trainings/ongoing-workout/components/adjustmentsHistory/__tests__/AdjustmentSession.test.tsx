import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import {
  exerciseHistory,
  exerciseSet,
} from "@/features/trainings/__tests__/fixtures";

const { mockMutate } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
}));

vi.mock("../../../mutations/useDeleteExerciseHistoryMutation", () => ({
  default: () => ({
    mutate: mockMutate,
    isPending: false,
  }),
}));

import AdjustmentSession from "../AdjustmentSession";

describe("AdjustmentSession", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("shows the adjustment count and formatted date", () => {
    render(
      <AdjustmentSession
        history={exerciseHistory("history-1", {
          createdOnUtc: "2026-06-05T10:00:00Z",
        })}
      />,
    );

    expect(screen.getByText("1 adjustment")).toBeInTheDocument();
    expect(screen.getByText(/Jun 05/)).toBeInTheDocument();
    expect(screen.getByText("2 hours ago")).toBeInTheDocument();
  });

  it("deletes the adjustment history when the trash button is clicked", () => {
    render(
      <AdjustmentSession
        history={exerciseHistory("history-1", { exerciseId: "ex-42" })}
      />,
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Delete adjustment history" }),
    );

    expect(mockMutate).toHaveBeenCalledWith({
      id: "history-1",
      exerciseId: "ex-42",
    });
  });

  it("expands hidden adjustments when there are more than four changes", () => {
    const oldSets = Array.from({ length: 5 }, (_, index) =>
      exerciseSet(`old-${index}`, { count1: 10 }),
    );
    const newSets = Array.from({ length: 5 }, (_, index) =>
      exerciseSet(`old-${index}`, { count1: 10 + index + 1 }),
    );

    render(
      <AdjustmentSession
        history={exerciseHistory("history-1", {
          oldExerciseSets: oldSets,
          newExerciseSets: newSets,
        })}
      />,
    );

    expect(screen.getByText("5 adjustments")).toBeInTheDocument();
    expect(screen.getByText("Show 1 more adjustments")).toBeInTheDocument();
    fireEvent.click(screen.getByText("Show 1 more adjustments"));
    expect(screen.getByText("Show less adjustments")).toBeInTheDocument();
  });
});

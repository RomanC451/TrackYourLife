import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { exercise } from "@/features/trainings/__tests__/fixtures";
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

vi.mock("../../common/MuscleGroupsFilter", () => ({
  default: ({
    selectedMuscleGroup,
    setSelectedMuscleGroup,
  }: {
    selectedMuscleGroup: string;
    setSelectedMuscleGroup: (value: string) => void;
  }) => (
    <button
      type="button"
      onClick={() => setSelectedMuscleGroup("legs")}
    >
      Filter: {selectedMuscleGroup || "all"}
    </button>
  ),
}));

vi.mock("../exercisesListItem", () => ({
  default: ({ exercise: item }: { exercise: { name: string } }) => (
    <div data-testid="exercise-item">{item.name}</div>
  ),
}));

import ExercisesList from "../exercisesList";

describe("ExercisesList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseSuspenseQuery.mockReturnValue({
      data: [
        exercise("ex-1", { name: "Bench press", muscleGroups: ["chest"] }),
        exercise("ex-2", { name: "Squat", muscleGroups: ["legs"] }),
        exercise("ex-3", { name: "Bench fly", muscleGroups: ["chest"] }),
      ],
    });
  });

  it("filters exercises by search query", () => {
    render(<ExercisesList />, { wrapper: createQueryClientWrapper() });

    expect(screen.getAllByTestId("exercise-item")).toHaveLength(3);

    fireEvent.change(screen.getByPlaceholderText("Search"), {
      target: { value: "Bench" },
    });
    expect(screen.getAllByTestId("exercise-item")).toHaveLength(2);
  });

  it("filters exercises by muscle group", () => {
    render(<ExercisesList />, { wrapper: createQueryClientWrapper() });

    fireEvent.click(screen.getByRole("button", { name: /Filter:/ }));
    expect(screen.getByTestId("exercise-item")).toHaveTextContent("Squat");
  });

  it("resets search and muscle group filters", () => {
    render(<ExercisesList />, { wrapper: createQueryClientWrapper() });

    fireEvent.change(screen.getByPlaceholderText("Search"), {
      target: { value: "Bench" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Filter:/ }));
    fireEvent.click(screen.getByRole("button", { name: "Reset filters" }));

    expect(screen.getAllByTestId("exercise-item")).toHaveLength(3);
    expect(screen.getByRole("button", { name: "Filter: all" })).toBeInTheDocument();
  });
});

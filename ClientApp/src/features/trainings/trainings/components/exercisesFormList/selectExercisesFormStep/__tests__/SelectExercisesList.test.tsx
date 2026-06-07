import { fireEvent, render, screen } from "@testing-library/react";
import { beforeAll, describe, expect, it, vi } from "vitest";

class ResizeObserverMock {
  observe() {}
  unobserve() {}
  disconnect() {}
}

beforeAll(() => {
  vi.stubGlobal("ResizeObserver", ResizeObserverMock);
});

import { exercise } from "@/features/trainings/__tests__/fixtures";

const { mockNavigate, mockPreloadRoute } = vi.hoisted(() => ({
  mockNavigate: vi.fn(),
  mockPreloadRoute: vi.fn(),
}));

vi.mock("@tanstack/react-router", () => ({
  Link: ({
    children,
    to,
  }: {
    children: React.ReactNode;
    to: string;
  }) => <a href={to}>{children}</a>,
  useNavigate: () => mockNavigate,
}));

vi.mock("@/App", () => ({
  router: { preloadRoute: mockPreloadRoute },
}));

vi.mock("../SelectExercisesListElement", () => ({
  default: ({
    exercise,
    isSelected,
    onSelect,
  }: {
    exercise: { id: string; name: string };
    isSelected: boolean;
    onSelect: (exercise: { id: string; name: string }) => void;
  }) => (
    <button type="button" onClick={() => onSelect(exercise)}>
      {exercise.name}
      {isSelected ? " (selected)" : ""}
    </button>
  ),
}));

import SelectExercisesList from "../SelectExercisesList";

describe("SelectExercisesList", () => {
  const exercises = [
    exercise("ex-1", { name: "Bench press" }),
    exercise("ex-2", { name: "Squat" }),
    exercise("ex-3", { name: "Deadlift" }),
    exercise("ex-4", { name: "Row" }),
  ];

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders exercises and handles selection", () => {
    const onSelect = vi.fn();

    render(
      <SelectExercisesList
        exercises={exercises}
        selectedExercises={[exercises[0]]}
        onSelect={onSelect}
      />,
    );

    expect(screen.getByText("Bench press (selected)")).toBeInTheDocument();
    fireEvent.click(screen.getByText("Squat"));
    expect(onSelect).toHaveBeenCalledWith(exercises[1]);
  });

  it("navigates to create exercise on add button click", () => {
    render(
      <SelectExercisesList
        exercises={exercises}
        selectedExercises={[]}
        onSelect={vi.fn()}
      />,
    );

    const addButton = document.querySelector(".lucide-plus")!.closest("button")!;
    fireEvent.click(addButton);
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/trainings/workouts/exercises/create",
    });
  });

  it("preloads create route on hover and touch", () => {
    render(
      <SelectExercisesList
        exercises={exercises}
        selectedExercises={[]}
        onSelect={vi.fn()}
      />,
    );

    const addButton = document.querySelector(".lucide-plus")!.closest("button")!;
    fireEvent.mouseEnter(addButton);
    fireEvent.touchStart(addButton);

    expect(mockPreloadRoute).toHaveBeenCalledTimes(2);
    expect(mockPreloadRoute).toHaveBeenCalledWith({
      to: "/trainings/workouts/exercises/create",
    });
  });

  it("renders loading and empty states with create link", () => {
    const { rerender } = render(<SelectExercisesList.Loading />);
    expect(screen.getByRole("link")).toHaveAttribute(
      "href",
      "/trainings/workouts/exercises/create",
    );

    rerender(<SelectExercisesList.Empty />);
    expect(screen.getByRole("link")).toHaveAttribute(
      "href",
      "/trainings/workouts/exercises/create",
    );
  });
});

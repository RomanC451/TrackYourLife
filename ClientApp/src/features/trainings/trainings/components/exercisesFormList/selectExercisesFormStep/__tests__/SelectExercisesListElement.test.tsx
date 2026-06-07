import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { exercise } from "@/features/trainings/__tests__/fixtures";

vi.mock(
  "@/features/trainings/exercises/components/common/ExerciseMenu",
  () => ({
    default: ({
      onOpen,
      onClose,
    }: {
      onOpen: () => void;
      onClose: () => void;
    }) => (
      <button type="button" onClick={onOpen} onBlur={onClose}>
        Exercise menu
      </button>
    ),
  }),
);

import SelectExercisesListElement from "../SelectExercisesListElement";

describe("SelectExercisesListElement", () => {
  const onSelect = vi.fn();
  const onOpenMenu = vi.fn();
  const onCloseMenu = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders exercise name and equipment badge", () => {
    render(
      <SelectExercisesListElement
        exercise={exercise("ex-1", { name: "Bench press", equipment: "Barbell" })}
        isSelected={false}
        isMenuOpen={false}
        onOpenMenu={onOpenMenu}
        onCloseMenu={onCloseMenu}
        onSelect={onSelect}
      />,
    );

    expect(screen.getByText("Bench press")).toBeInTheDocument();
    expect(screen.getByText("Barbell")).toBeInTheDocument();
  });

  it("shows no equipment fallback and selected indicator", () => {
    render(
      <SelectExercisesListElement
        exercise={exercise("ex-1", { name: "Push-up", equipment: undefined })}
        isSelected
        isMenuOpen={false}
        onOpenMenu={onOpenMenu}
        onCloseMenu={onCloseMenu}
        onSelect={onSelect}
      />,
    );

    expect(screen.getByText("No equipment")).toBeInTheDocument();
    expect(document.querySelector(".lucide-circle-check")).toBeTruthy();
  });

  it("calls onSelect when the card is clicked", () => {
    const bench = exercise("ex-1", { name: "Bench press" });

    render(
      <SelectExercisesListElement
        exercise={bench}
        isSelected={false}
        isMenuOpen={false}
        onOpenMenu={onOpenMenu}
        onCloseMenu={onCloseMenu}
        onSelect={onSelect}
      />,
    );

    fireEvent.click(screen.getByText("Bench press"));
    expect(onSelect).toHaveBeenCalledWith(bench);
  });

  it("applies disabled styles for loading and deleting exercises", () => {
    const { rerender } = render(
      <SelectExercisesListElement
        exercise={exercise("ex-1", { isLoading: true })}
        isSelected={false}
        isMenuOpen={false}
        onOpenMenu={onOpenMenu}
        onCloseMenu={onCloseMenu}
        onSelect={onSelect}
      />,
    );

    expect(screen.getByText("Exercise ex-1").closest(".cursor-not-allowed")).toBeTruthy();

    rerender(
      <SelectExercisesListElement
        exercise={exercise("ex-1", { isDeleting: true })}
        isSelected={false}
        isMenuOpen={false}
        onOpenMenu={onOpenMenu}
        onCloseMenu={onCloseMenu}
        onSelect={onSelect}
      />,
    );

    expect(screen.getByText("Exercise ex-1").closest(".opacity-50")).toBeTruthy();
  });
});

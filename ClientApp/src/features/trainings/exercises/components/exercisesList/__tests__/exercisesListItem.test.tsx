import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { exercise, exerciseSet } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockMutate, mockPreloadRoute } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
  mockPreloadRoute: vi.fn(),
}));

vi.mock("../../../mutations/useDeleteExerciseMutation", () => ({
  default: () => ({ mutate: mockMutate, isPending: false }),
}));

vi.mock("@tanstack/react-router", () => ({
  Link: ({
    children,
    onMouseEnter,
  }: {
    children: React.ReactNode;
    onMouseEnter?: () => void;
  }) => (
    <a href="#" onMouseEnter={onMouseEnter}>
      {children}
    </a>
  ),
}));

vi.mock("@/App", () => ({
  router: { preloadRoute: mockPreloadRoute },
}));

vi.mock("@/components/ui/dropdown-menu", () => ({
  DropdownMenu: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuTrigger: ({
    children,
  }: {
    children: React.ReactNode;
  }) => <div>{children}</div>,
  DropdownMenuContent: ({ children }: { children: React.ReactNode }) => (
    <div role="menu">{children}</div>
  ),
  DropdownMenuItem: ({
    children,
    onSelect,
  }: {
    children: React.ReactNode;
    onSelect?: () => void;
  }) => (
    <button type="button" role="menuitem" onClick={() => onSelect?.()}>
      {children}
    </button>
  ),
  DropdownMenuSeparator: () => <hr />,
}));

vi.mock("../../common/ForceDeleteExerciseAlertDialog", () => ({
  default: () => <div data-testid="force-delete-alert" />,
}));

import ExercisesListItem from "../exercisesListItem";

describe("ExercisesListItem", () => {
  const bench = exercise("ex-1", {
    name: "Bench press",
    equipment: "Barbell",
    muscleGroups: ["chest", "triceps"],
    exerciseSets: [
      exerciseSet("s-1", { count1: 60, unit1: "kg", count2: 8, unit2: "reps" }),
      exerciseSet("s-2", { count1: 10, unit1: "reps" }),
      exerciseSet("s-3", { count1: 10, unit1: "reps" }),
      exerciseSet("s-4", { count1: 10, unit1: "reps" }),
    ],
  });

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders exercise details and set preview", () => {
    render(<ExercisesListItem exercise={bench} />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByText("Bench press")).toBeInTheDocument();
    expect(screen.getByText("chest · triceps")).toBeInTheDocument();
    expect(screen.getByText("4 sets")).toBeInTheDocument();
    expect(screen.getByText("Barbell")).toBeInTheDocument();
    expect(screen.getByText("60 kg")).toBeInTheDocument();
    expect(screen.getByText("+1 more sets")).toBeInTheDocument();
  });

  it("calls delete mutation from the menu", () => {
    render(<ExercisesListItem exercise={bench} />, {
      wrapper: createQueryClientWrapper(),
    });

    fireEvent.click(screen.getByRole("button", { name: "Exercise menu" }));
    fireEvent.click(screen.getByText("Delete"));
    expect(mockMutate).toHaveBeenCalledWith(
      expect.objectContaining({
        id: "ex-1",
        forceDelete: false,
        name: "Bench press",
      }),
    );
  });
});

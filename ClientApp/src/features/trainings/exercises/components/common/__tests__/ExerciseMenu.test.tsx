import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { exercise } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockMutate } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
}));

vi.mock("../../../mutations/useDeleteExerciseMutation", () => ({
  default: () => ({ mutate: mockMutate, isPending: false }),
}));

vi.mock("@tanstack/react-router", () => ({
  Link: ({ children, className }: { children: React.ReactNode; className?: string }) => (
    <a href="#" className={className}>
      {children}
    </a>
  ),
}));

vi.mock("./ForceDeleteExerciseAlertDialog", () => ({
  default: () => <div data-testid="force-delete-alert" />,
}));

import ExerciseMenu from "../ExerciseMenu";

describe("ExerciseMenu", () => {
  const bench = exercise("ex-1", { name: "Bench press" });

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("opens menu and shows actions", () => {
    render(<ExerciseMenu exercise={bench} />, {
      wrapper: createQueryClientWrapper(),
    });

    fireEvent.click(screen.getByRole("button", { name: "Exercise menu" }));
    expect(screen.getByText("View Exercise")).toBeInTheDocument();
    expect(screen.getByText("View Stats")).toBeInTheDocument();
    expect(screen.getByText("Edit Exercise")).toBeInTheDocument();
    expect(screen.getByText("Delete Exercise")).toBeInTheDocument();
  });

  it("deletes exercise and closes menu", () => {
    render(<ExerciseMenu exercise={bench} />, {
      wrapper: createQueryClientWrapper(),
    });

    fireEvent.click(screen.getByRole("button", { name: "Exercise menu" }));
    fireEvent.click(screen.getByRole("button", { name: /Delete Exercise/i }));
    expect(mockMutate).toHaveBeenCalled();
    expect(screen.queryByRole("menu")).not.toBeInTheDocument();
  });

  it("supports controlled open state", () => {
    const onClose = vi.fn();

    render(
      <ExerciseMenu
        exercise={bench}
        defaultOpen
        onClose={onClose}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.getByRole("menu")).toBeInTheDocument();
    fireEvent.keyDown(screen.getByRole("menu"), { key: "Escape" });
    expect(onClose).toHaveBeenCalledOnce();
  });
});

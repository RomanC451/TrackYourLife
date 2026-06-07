import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { exercise } from "@/features/trainings/__tests__/fixtures";

vi.mock("../../../mutations/useUpdateExerciseMutation", () => ({
  default: () => ({
    mutate: vi.fn(),
    pendingState: { isPending: false, isDelayedPending: false },
  }),
}));

vi.mock("../ExerciseDialog", () => ({
  default: ({
    dialogType,
    defaultValues,
  }: {
    dialogType: string;
    defaultValues: { name: string };
  }) => (
    <div data-testid="exercise-dialog">
      {dialogType}:{defaultValues.name}
    </div>
  ),
}));

import EditExerciseDialog from "../EditExerciseDialog";

describe("EditExerciseDialog", () => {
  it("renders edit exercise dialog with exercise values", () => {
    const bench = exercise("ex-1", { name: "Bench press" });
    render(<EditExerciseDialog exercise={bench} />);
    expect(screen.getByTestId("exercise-dialog")).toHaveTextContent(
      "edit:Bench press",
    );
  });
});

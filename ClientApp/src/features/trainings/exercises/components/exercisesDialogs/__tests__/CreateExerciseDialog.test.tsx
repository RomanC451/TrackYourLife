import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("../../../mutations/useCreateExerciseMutation", () => ({
  default: () => ({
    mutate: vi.fn(),
    pendingState: { isPending: false, isDelayedPending: false },
  }),
}));

vi.mock("../ExerciseDialog", () => ({
  default: ({
    dialogType,
    defaultValues,
    submitButtonText,
  }: {
    dialogType: string;
    defaultValues: { name: string };
    submitButtonText?: string;
  }) => (
    <div data-testid="exercise-dialog">
      {dialogType}:{defaultValues.name}:{submitButtonText}
    </div>
  ),
}));

import CreateExerciseDialog from "../CreateExerciseDialog";

describe("CreateExerciseDialog", () => {
  it("renders create exercise dialog with default values", () => {
    render(<CreateExerciseDialog />);
    expect(screen.getByTestId("exercise-dialog")).toHaveTextContent("create:");
  });
});

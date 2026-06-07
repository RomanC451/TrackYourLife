import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { exercise, training } from "@/features/trainings/__tests__/fixtures";

const { mockMutate } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
}));

vi.mock("../../../mutations/useUpdateTrainingMutation", () => ({
  default: () => ({
    mutate: mockMutate,
    pendingState: { isPending: false, isDelayedPending: false },
  }),
}));

vi.mock("../TrainingDialog", () => ({
  default: ({
    dialogType,
    defaultValues,
  }: {
    dialogType: string;
    defaultValues: { name: string; exercises: { name: string }[] };
  }) => (
    <div data-testid="training-dialog">
      <span>{dialogType}</span>
      <span>{defaultValues.name}</span>
      <span>{defaultValues.exercises[0]?.name}</span>
    </div>
  ),
}));

import EditTrainingDialog from "../EditTrainingDialog";

describe("EditTrainingDialog", () => {
  it("renders edit training dialog with mapped default values", () => {
    const pushDay = training("training-1", {
      name: "Push day",
      exercises: [exercise("ex-1", { name: "Bench press", equipment: "Barbell" })],
    });

    render(<EditTrainingDialog training={pushDay} />);

    const dialog = screen.getByTestId("training-dialog");
    expect(dialog).toHaveTextContent("edit");
    expect(dialog).toHaveTextContent("Push day");
    expect(dialog).toHaveTextContent("Bench press");
  });
});

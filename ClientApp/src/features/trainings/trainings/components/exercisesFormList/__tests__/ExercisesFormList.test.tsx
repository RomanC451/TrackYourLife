import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("../selectExercisesFormStep/SelectExercisesFormStep", () => ({
  default: ({
    setStep,
    onCancel,
  }: {
    setStep: (step: "select" | "order") => void;
    onCancel: () => void;
  }) => (
    <div>
      <span>Select step</span>
      <button type="button" onClick={() => setStep("order")}>
        Go to order
      </button>
      <button type="button" onClick={onCancel}>
        Cancel select
      </button>
    </div>
  ),
}));

vi.mock("../orderExercisesFormStep/OrderExercisesFormStep", () => ({
  default: ({ submitButtonText }: { submitButtonText: string }) => (
    <div>Order step: {submitButtonText}</div>
  ),
}));

import ExercisesFormList from "../ExercisesFormList";

describe("ExercisesFormList", () => {
  it("starts on the select step and switches to order", () => {
    const onCancel = vi.fn();

    render(
      <ExercisesFormList
        pendingState={{ isPending: false, isDelayedPending: false }}
        onCancel={onCancel}
        submitButtonText="Save training"
      />,
    );

    expect(screen.getByText("Select step")).toBeInTheDocument();
    fireEvent.click(screen.getByText("Cancel select"));
    expect(onCancel).toHaveBeenCalledOnce();

    fireEvent.click(screen.getByText("Go to order"));
    expect(screen.getByText("Order step: Save training")).toBeInTheDocument();
  });
});

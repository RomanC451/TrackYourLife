import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import OrderExercisesFormStepFooter from "../OrderExercisesFormStepFooter";

describe("OrderExercisesFormStepFooter", () => {
  it("calls cancel and navigates back to selection", () => {
    const onCancel = vi.fn();
    const setStep = vi.fn();

    render(
      <OrderExercisesFormStepFooter
        onCancel={onCancel}
        setStep={setStep}
        submitButtonText="Save training"
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Cancel" }));
    fireEvent.click(screen.getByRole("button", { name: "Back to Selection" }));

    expect(onCancel).toHaveBeenCalledOnce();
    expect(setStep).toHaveBeenCalledWith("select");
    expect(screen.getByRole("button", { name: "Save training" })).toBeEnabled();
  });

  it("disables submit while the mutation is pending", () => {
    render(
      <OrderExercisesFormStepFooter
        onCancel={vi.fn()}
        setStep={vi.fn()}
        submitButtonText="Save training"
        pendingState={{ isPending: true, isDelayedPending: true }}
      />,
    );

    expect(
      screen.getByRole("button", { name: /Save training/ }),
    ).toBeDisabled();
  });
});

import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { TooltipProvider } from "@/components/ui/tooltip";

import SelectExercisesFormStepFooter from "../SelectExercisesFormStepFooter";

function renderFooter(
  props: React.ComponentProps<typeof SelectExercisesFormStepFooter>,
) {
  return render(
    <TooltipProvider>
      <SelectExercisesFormStepFooter {...props} />
    </TooltipProvider>,
  );
}

describe("SelectExercisesFormStepFooter", () => {
  it("disables progression when no exercises are selected", () => {
    const onCancel = vi.fn();
    renderFooter({
      selectedExercises: [],
      onCancel,
      setStep: vi.fn(),
    });

    expect(screen.getByText("0 exercises selected")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Cancel" }));
    expect(onCancel).toHaveBeenCalledOnce();
    expect(
      screen.getByRole("button", { name: "Next: Order Exercises" }),
    ).toHaveClass("opacity-50");
  });

  it("moves to the order step when exercises are selected", () => {
    const setStep = vi.fn();
    renderFooter({
      selectedExercises: [
        {
          id: "ex-1",
          name: "Squat",
          muscleGroups: ["legs"],
          difficulty: "Easy",
          exerciseSets: [
            {
              id: "set-1",
              name: "Set 1",
              orderIndex: 0,
              count1: 10,
              unit1: "reps",
            },
          ],
        },
      ],
      onCancel: vi.fn(),
      setStep,
    });

    fireEvent.click(screen.getByRole("button", { name: "Next: Order Exercises" }));
    expect(setStep).toHaveBeenCalledWith("order");
  });
});

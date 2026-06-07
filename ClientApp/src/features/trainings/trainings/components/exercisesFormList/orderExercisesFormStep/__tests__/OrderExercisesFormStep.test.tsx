import { fireEvent, render, screen } from "@testing-library/react";
import { FormProvider, useForm } from "react-hook-form";
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
import { Difficulty } from "@/services/openapi";

import { TrainingFormSchema } from "../../../../data/trainingsSchemas";
import OrderExercisesFormStep from "../OrderExercisesFormStep";

const exercises = [
  exercise("ex-1", { name: "Bench press" }),
  exercise("ex-2", { name: "Squat" }),
  exercise("ex-3", { name: "Deadlift" }),
];

function Harness() {
  const form = useForm<TrainingFormSchema>({
    defaultValues: {
      name: "Workout",
      muscleGroups: ["chest"],
      difficulty: Difficulty.Easy,
      duration: 60,
      restSeconds: 60,
      exercises,
    },
  });

  return (
    <FormProvider {...form}>
      <OrderExercisesFormStep
        setStep={vi.fn()}
        onCancel={vi.fn()}
        pendingState={{ isPending: false, isDelayedPending: false }}
        submitButtonText="Save training"
      />
    </FormProvider>
  );
}

describe("OrderExercisesFormStep", () => {
  it("renders selected exercises with order numbers", () => {
    render(<Harness />);

    expect(screen.getByText("Step 2: Order Exercises")).toBeInTheDocument();
    expect(screen.getByText("Bench press")).toBeInTheDocument();
    expect(screen.getByText("Squat")).toBeInTheDocument();
    expect(screen.getByText("Deadlift")).toBeInTheDocument();
    expect(screen.getByText("1")).toBeInTheDocument();
    expect(screen.getByText("2")).toBeInTheDocument();
    expect(screen.getByText("3")).toBeInTheDocument();
  });

  it("removes an exercise from the form", () => {
    render(<Harness />);

    const removeButtons = screen.getAllByRole("button").filter((btn) => {
      return btn.querySelector(".lucide-x");
    });

    fireEvent.click(removeButtons[0]);
    expect(screen.queryByText("Bench press")).not.toBeInTheDocument();
    expect(screen.getByText("Squat")).toBeInTheDocument();
  });

  it("syncs when form exercises change externally", () => {
    function SyncHarness() {
      const form = useForm<TrainingFormSchema>({
        defaultValues: {
          name: "Workout",
          muscleGroups: ["chest"],
          difficulty: Difficulty.Easy,
          duration: 60,
          restSeconds: 60,
          exercises: [exercises[0]],
        },
      });

      return (
        <FormProvider {...form}>
          <button
            type="button"
            onClick={() => form.setValue("exercises", exercises)}
          >
            Add exercises
          </button>
          <OrderExercisesFormStep
            setStep={vi.fn()}
            onCancel={vi.fn()}
            pendingState={{ isPending: false, isDelayedPending: false }}
            submitButtonText="Save training"
          />
        </FormProvider>
      );
    }

    render(<SyncHarness />);
    expect(screen.getByText("Bench press")).toBeInTheDocument();
    expect(screen.queryByText("Squat")).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Add exercises" }));
    expect(screen.getByText("Squat")).toBeInTheDocument();
  });
});

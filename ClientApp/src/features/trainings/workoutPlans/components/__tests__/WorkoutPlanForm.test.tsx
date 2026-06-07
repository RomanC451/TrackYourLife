import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { training } from "@/features/trainings/__tests__/fixtures";

import WorkoutPlanForm from "../WorkoutPlanForm";

describe("WorkoutPlanForm", () => {
  const trainings = [
    training("w-1", { name: "Push" }),
    training("w-2", { name: "Pull" }),
    training("w-3", { name: "Legs" }),
  ];

  it("renders plan name and selected workouts", () => {
    render(
      <WorkoutPlanForm
        trainings={trainings}
        defaultValues={{
          name: "PPL",
          isActive: false,
          trainingIds: ["w-1", "w-2"],
        }}
        submitButtonText="Save plan"
        isPending={false}
        onCancel={vi.fn()}
        onSubmit={vi.fn()}
      />,
    );

    expect(screen.getByDisplayValue("PPL")).toBeInTheDocument();
    expect(screen.getByText("Push")).toBeInTheDocument();
    expect(screen.getByText("Pull")).toBeInTheDocument();
  });

  it("requires at least two workouts before submit", () => {
    const onSubmit = vi.fn();

    render(
      <WorkoutPlanForm
        trainings={trainings}
        defaultValues={{
          name: "Solo",
          isActive: false,
          trainingIds: ["w-1"],
        }}
        submitButtonText="Save plan"
        isPending={false}
        onCancel={vi.fn()}
        onSubmit={onSubmit}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: /Save plan/i }));
    expect(
      screen.getByText("Please add at least 2 workouts to the plan."),
    ).toBeInTheDocument();
    expect(onSubmit).not.toHaveBeenCalled();
  });

  it("submits valid plan values", () => {
    const onSubmit = vi.fn();
    const onCancel = vi.fn();

    render(
      <WorkoutPlanForm
        trainings={trainings}
        defaultValues={{
          name: "PPL",
          isActive: true,
          trainingIds: ["w-1", "w-2"],
        }}
        submitButtonText="Save plan"
        isPending={false}
        onCancel={onCancel}
        onSubmit={onSubmit}
      />,
    );

    fireEvent.change(screen.getByLabelText(/Plan Name/i), {
      target: { value: "  My Plan  " },
    });
    fireEvent.click(screen.getByRole("button", { name: /Save plan/i }));

    expect(onSubmit).toHaveBeenCalledWith({
      name: "My Plan",
      isActive: true,
      trainingIds: ["w-1", "w-2"],
    });

    fireEvent.click(screen.getByRole("button", { name: "Cancel" }));
    expect(onCancel).toHaveBeenCalledOnce();
  });

  it("adds an available workout from the picker", () => {
    const onSubmit = vi.fn();

    render(
      <WorkoutPlanForm
        trainings={trainings}
        defaultValues={{
          name: "Plan",
          isActive: false,
          trainingIds: ["w-1", "w-2"],
        }}
        submitButtonText="Create"
        isPending={false}
        onCancel={vi.fn()}
        onSubmit={onSubmit}
      />,
    );

    fireEvent.click(
      screen.getByRole("button", { name: /Add workout to plan/i }),
    );
    fireEvent.click(screen.getByRole("button", { name: "Legs" }));
    fireEvent.click(screen.getByRole("button", { name: /Create/i }));

    expect(onSubmit).toHaveBeenCalledWith({
      name: "Plan",
      isActive: false,
      trainingIds: ["w-1", "w-2", "w-3"],
    });
  });
});

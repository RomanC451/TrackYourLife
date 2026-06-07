import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { training } from "@/features/trainings/__tests__/fixtures";

const mockOnClose = vi.fn();
const mockOnSubmit = vi.fn();

vi.mock("@/components/ui/dialog", () => ({
  Dialog: ({
    children,
    onOpenChange,
  }: {
    children: React.ReactNode;
    onOpenChange?: (open: boolean) => void;
  }) => (
    <div>
      <button type="button" onClick={() => onOpenChange?.(false)}>
        Close dialog
      </button>
      {children}
    </div>
  ),
  DialogContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DialogHeader: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DialogTitle: ({ children }: { children: React.ReactNode }) => (
    <h1>{children}</h1>
  ),
  DialogDescription: ({ children }: { children: React.ReactNode }) => (
    <p>{children}</p>
  ),
}));

vi.mock("../WorkoutPlanForm", () => ({
  default: ({
    submitButtonText,
    onCancel,
    onSubmit,
  }: {
    submitButtonText: string;
    onCancel: () => void;
    onSubmit: (values: {
      name: string;
      isActive: boolean;
      trainingIds: string[];
    }) => void;
  }) => (
    <div>
      <button type="button" onClick={onCancel}>
        Cancel form
      </button>
      <button
        type="button"
        onClick={() =>
          onSubmit({ name: "Plan A", isActive: true, trainingIds: ["a"] })
        }
      >
        {submitButtonText}
      </button>
    </div>
  ),
}));

import WorkoutPlanDialog from "../WorkoutPlanDialog";

describe("WorkoutPlanDialog", () => {
  const defaultValues = {
    name: "Plan A",
    isActive: true,
    trainingIds: ["a"],
  };

  it("renders the dialog content and forwards form actions", () => {
    render(
      <WorkoutPlanDialog
        title="Create plan"
        description="Build a weekly workout plan"
        submitButtonText="Create plan"
        trainings={[training("a", { name: "Leg day" })]}
        defaultValues={defaultValues}
        isPending={false}
        onClose={mockOnClose}
        onSubmit={mockOnSubmit}
      />,
    );

    expect(screen.getByRole("heading", { name: "Create plan" })).toBeInTheDocument();
    expect(
      screen.getByText("Build a weekly workout plan"),
    ).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Create plan" }));
    expect(mockOnSubmit).toHaveBeenCalledWith(defaultValues);

    fireEvent.click(screen.getByRole("button", { name: "Cancel form" }));
    expect(mockOnClose).toHaveBeenCalledOnce();

    fireEvent.click(screen.getByRole("button", { name: "Close dialog" }));
    expect(mockOnClose).toHaveBeenCalledTimes(2);
  });
});

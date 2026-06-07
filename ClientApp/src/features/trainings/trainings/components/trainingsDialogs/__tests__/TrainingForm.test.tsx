import { render, screen } from "@testing-library/react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { describe, expect, it, vi } from "vitest";

import { TrainingFormSchema } from "../../../data/trainingsSchemas";
import TrainingForm from "../TrainingForm";

vi.mock(
  "@/features/trainings/exercises/components/exercisesDialogs/MuscleGroupSelect",
  () => ({
    MuscleGroupSelect: () => <div data-testid="muscle-group-select" />,
  }),
);

vi.mock("../../exercisesFormList/ExercisesFormList", () => ({
  default: ({ submitButtonText }: { submitButtonText: string }) => (
    <button type="submit">{submitButtonText}</button>
  ),
}));

function TrainingFormHarness({
  defaultValues,
  tab = "details",
}: {
  defaultValues: TrainingFormSchema;
  tab?: "details" | "exercises";
}) {
  const form = useForm<TrainingFormSchema>({ defaultValues });
  const [activeTab, setActiveTab] = useState(tab);

  return (
    <TrainingForm
      tab={activeTab}
      setTab={setActiveTab}
      form={form}
      handleCustomSubmit={vi.fn()}
      submitButtonText="Save workout"
      pendingState={{ isPending: false, isDelayedPending: false }}
      onCancel={vi.fn()}
    />
  );
}

describe("TrainingForm", () => {
  it("renders details tab fields", () => {
    render(
      <TrainingFormHarness
        defaultValues={{
          name: "Push day",
          muscleGroups: ["chest"],
          difficulty: "Easy",
          description: "Chest focus",
          duration: 45,
          restSeconds: 60,
          exercises: [],
        }}
      />,
    );

    expect(screen.getByDisplayValue("Push day")).toBeInTheDocument();
    expect(screen.getByDisplayValue("Chest focus")).toBeInTheDocument();
    expect(screen.getByTestId("muscle-group-select")).toBeInTheDocument();
  });

  it("renders exercises tab trigger", () => {
    render(
      <TrainingFormHarness
        defaultValues={{
          name: "",
          muscleGroups: [],
          difficulty: "Easy",
          description: "",
          duration: 0,
          restSeconds: 30,
          exercises: [],
        }}
      />,
    );

    expect(screen.getByRole("tab", { name: "Exercises" })).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: "Details" })).toBeInTheDocument();
  });
});

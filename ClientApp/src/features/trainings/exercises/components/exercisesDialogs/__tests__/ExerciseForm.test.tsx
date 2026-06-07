import { render, screen } from "@testing-library/react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/services/openapi/api", () => ({
  ImagesApi: class {
    upload() {
      return Promise.resolve("");
    }
  },
  Difficulty: { Easy: "Easy", Medium: "Medium", Hard: "Hard" },
}));

vi.mock("../MuscleGroupSelect", () => ({
  MuscleGroupSelect: () => <div data-testid="muscle-group-select" />,
}));

vi.mock("../FileDropZone", () => ({
  default: () => <div data-testid="file-drop-zone" />,
}));

vi.mock("../exerciseSetRows/ExerciseSetRow", () => ({
  default: () => <div data-testid="exercise-set-row" />,
}));

vi.mock("../exerciseSetRows/SetTypeDropdownMenu", () => ({
  default: () => <div data-testid="set-type-menu" />,
}));

import { ExerciseFormSchema } from "../../../data/exercisesSchemas";
import ExerciseForm from "../ExerciseForm";

function ExerciseFormHarness() {
  const [tab, setTab] = useState("details");
  const form = useForm<ExerciseFormSchema>({
    defaultValues: {
      id: "",
      name: "Bench press",
      muscleGroups: ["chest"],
      difficulty: "Easy",
      description: "",
      equipment: "Barbell",
      videoUrl: "",
      pictureUrl: "",
      exerciseSets: [
        {
          id: "set-1",
          name: "working set",
          orderIndex: 0,
          count1: 10,
          unit1: "reps",
        },
      ],
    },
  });

  return (
    <ExerciseForm
      tab={tab}
      setTab={setTab}
      form={form}
      handleCustomSubmit={vi.fn()}
      submitButtonText="Save exercise"
      pendingState={{ isPending: false, isDelayedPending: false }}
    />
  );
}

describe("ExerciseForm", () => {
  it("renders details tab with mocked child fields", () => {
    render(<ExerciseFormHarness />);

    expect(screen.getByDisplayValue("Bench press")).toBeInTheDocument();
    expect(screen.getByTestId("muscle-group-select")).toBeInTheDocument();
    expect(screen.getByTestId("file-drop-zone")).toBeInTheDocument();
  });

  it("renders sets tab trigger", () => {
    render(<ExerciseFormHarness />);

    expect(screen.getByRole("tab", { name: "Sets" })).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: "Details" })).toBeInTheDocument();
  });
});

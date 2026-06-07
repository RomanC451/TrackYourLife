import { fireEvent, render, screen } from "@testing-library/react";
import { FormProvider, useForm } from "react-hook-form";
import { describe, expect, it } from "vitest";

import { ExerciseFormSchema } from "../../../../data/exercisesSchemas";
import ExerciseSetRow from "../ExerciseSetRow";

function Harness({
  exerciseSets,
}: {
  exerciseSets: ExerciseFormSchema["exerciseSets"];
}) {
  const form = useForm<ExerciseFormSchema>({
    defaultValues: {
      id: "",
      name: "Bench press",
      muscleGroups: ["chest"],
      difficulty: "Easy",
      description: "",
      equipment: "",
      videoUrl: "",
      pictureUrl: "",
      exerciseSets,
    },
  });

  return (
    <FormProvider {...form}>
      {exerciseSets.map((_, index) => (
        <ExerciseSetRow key={index} index={index} form={form} />
      ))}
    </FormProvider>
  );
}

describe("ExerciseSetRow", () => {
  it("renders set name and count fields", () => {
    render(
      <Harness
        exerciseSets={[
          {
            id: "set-1",
            name: "working set",
            orderIndex: 0,
            count1: 10,
            unit1: "reps",
          },
        ]}
      />,
    );

    expect(screen.getByDisplayValue("working set")).toBeInTheDocument();
    expect(screen.getByDisplayValue("10")).toBeInTheDocument();
    expect(screen.queryByRole("button")).not.toBeInTheDocument();
  });

  it("renders count2 field for weight sets and allows removal", () => {
    render(
      <Harness
        exerciseSets={[
          {
            id: "set-1",
            name: "Set 1",
            orderIndex: 0,
            count1: 60,
            unit1: "kg",
            count2: 10,
            unit2: "reps",
          },
          {
            id: "set-2",
            name: "Set 2",
            orderIndex: 1,
            count1: 55,
            unit1: "kg",
            count2: 8,
            unit2: "reps",
          },
        ]}
      />,
    );

    expect(screen.getAllByDisplayValue("60").length).toBeGreaterThan(0);
    expect(screen.getAllByDisplayValue("10").length).toBeGreaterThan(0);

    const removeButton = screen.getByRole("button");
    expect(removeButton).toBeInTheDocument();
    fireEvent.click(removeButton);
  });

  it("shows empty input for negative count values", () => {
    render(
      <Harness
        exerciseSets={[
          {
            id: "set-1",
            name: "Set 1",
            orderIndex: 0,
            count1: -1,
            unit1: "reps",
          },
        ]}
      />,
    );

    const countInput = screen.getByPlaceholderText("reps");
    expect(countInput).toHaveValue(null);
  });
});

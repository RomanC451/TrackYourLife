import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { exercise } from "@/features/trainings/__tests__/fixtures";

import ExerciseDescriptionCollapsible from "../ExerciseDescriptionCollapsible";

describe("ExerciseDescriptionCollapsible", () => {
  it("reveals the exercise description when expanded", () => {
    render(
      <ExerciseDescriptionCollapsible
        exercise={exercise("ex-1", { description: "Keep your back flat." })}
      />,
    );

    expect(screen.queryByText("Keep your back flat.")).not.toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: /show description/i }));
    expect(screen.getByText("Keep your back flat.")).toBeVisible();
  });
});

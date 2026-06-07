import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { exerciseSet } from "@/features/trainings/__tests__/fixtures";

import CurrentSet from "../CurrentSet";

describe("CurrentSet", () => {
  it("renders equipment, set name, and both tracked values", () => {
    render(
      <CurrentSet
        equipment="Barbell"
        currentSet={exerciseSet("set-1", {
          name: "working set",
          count1: 60,
          unit1: "kg",
          count2: 8,
          unit2: "reps",
        })}
      />,
    );

    expect(screen.getByText("Equipment: Barbell")).toBeInTheDocument();
    expect(screen.getByText("Working set")).toBeInTheDocument();
    expect(screen.getByText("60 kg")).toBeInTheDocument();
    expect(screen.getByText("8 reps")).toBeInTheDocument();
  });
});

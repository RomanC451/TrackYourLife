import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { exerciseSet } from "@/features/trainings/__tests__/fixtures";

import AdjustmentSetChange from "../AdjustmentSetChange";

describe("AdjustmentSetChange", () => {
  it("shows only changed metrics", () => {
    render(
      <AdjustmentSetChange
        index={0}
        oldSet={exerciseSet("old", { count1: 50, count2: 10, unit2: "reps" })}
        newSet={exerciseSet("new", { count1: 55, count2: 10, unit2: "reps" })}
      />,
    );

    expect(screen.getByText("1. Set old")).toBeInTheDocument();
    expect(screen.getByText("+5.00 reps")).toBeInTheDocument();
    expect(screen.queryByText(/10\.00/)).not.toBeInTheDocument();
  });
});

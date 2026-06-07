import { render } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { MuscleGroupWorkoutIcon } from "../muscleGroupWorkoutIcon";

describe("MuscleGroupWorkoutIcon", () => {
  it("renders a dumbbell when no muscle groups are provided", () => {
    const { container } = render(<MuscleGroupWorkoutIcon muscleGroups={[]} />);
    expect(container.querySelector("svg")).toBeTruthy();
  });

  it("renders a masked asset icon when a known group is present", () => {
    const { container } = render(
      <MuscleGroupWorkoutIcon muscleGroups={["Upper Chest"]} />,
    );

    const icon = container.querySelector("span[aria-hidden='true']");
    expect(icon).toBeTruthy();
    expect(icon?.getAttribute("style")).toContain("chest.svg");
  });

  it("falls back to dumbbell for unknown groups", () => {
    const { container } = render(
      <MuscleGroupWorkoutIcon muscleGroups={["Unknown group"]} />,
    );
    expect(container.querySelector("span[aria-hidden='true']")).toBeNull();
    expect(container.querySelector("svg")).toBeTruthy();
  });
});

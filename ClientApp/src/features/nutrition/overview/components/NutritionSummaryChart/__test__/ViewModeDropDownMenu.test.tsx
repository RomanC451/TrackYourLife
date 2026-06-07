import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import ViewModeDropDownMenu from "../ViewModeDropDownMenu";

describe("ViewModeDropDownMenu", () => {
  it("shows the label for the selected view mode", () => {
    render(
      <ViewModeDropDownMenu viewMode="calories" setViewMode={() => {}} />,
    );

    expect(screen.getByText("Calories Overview")).toBeInTheDocument();
  });
});

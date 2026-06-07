import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import AggregationModeDropDownMenu from "../AggregationModeDropDownMenu";

describe("AggregationModeDropDownMenu", () => {
  it("shows daily average for average mode", () => {
    render(
      <AggregationModeDropDownMenu
        aggregationMode="Average"
        setAggregationMode={() => {}}
      />,
    );

    expect(screen.getByText("Daily Average")).toBeInTheDocument();
  });

  it("shows total sum for sum mode", () => {
    render(
      <AggregationModeDropDownMenu
        aggregationMode="Sum"
        setAggregationMode={() => {}}
      />,
    );

    expect(screen.getByText("Total Sum")).toBeInTheDocument();
  });
});

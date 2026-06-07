import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import OverviewTypeDropDownMenu from "../OverviewTypeDropDownMenu";

describe("OverviewTypeDropDownMenu", () => {
  it("shows the selected overview type", () => {
    render(
      <OverviewTypeDropDownMenu
        overviewType="Daily"
        setOverviewType={() => {}}
      />,
    );

    expect(screen.getByText("Daily")).toBeInTheDocument();
  });
});

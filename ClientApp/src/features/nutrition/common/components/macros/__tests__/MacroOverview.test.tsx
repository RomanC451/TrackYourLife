import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { colors } from "@/constants/tailwindColors";

import MacroOverview from "../MacroOverview";

describe("MacroOverview", () => {
  it("renders macro name, mass, and percentage", () => {
    render(
      <MacroOverview
        name="Protein"
        color={colors.violet}
        percentage={42}
        mass="35.5"
      />,
    );

    expect(screen.getByText("42%")).toBeInTheDocument();
    expect(screen.getByText("35.5g")).toBeInTheDocument();
    expect(screen.getByText("Protein")).toBeInTheDocument();
  });
});

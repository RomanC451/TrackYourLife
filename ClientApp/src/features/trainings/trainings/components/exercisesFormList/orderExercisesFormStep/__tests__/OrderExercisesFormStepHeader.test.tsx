import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import OrderExercisesFormStepHeader from "../OrderExercisesFormStepHeader";

describe("OrderExercisesFormStepHeader", () => {
  it("renders the step title and helper text", () => {
    render(<OrderExercisesFormStepHeader />);

    expect(screen.getByText("Step 2: Order Exercises")).toBeInTheDocument();
    expect(
      screen.getByText(/Drag and drop exercises to set the order/),
    ).toBeInTheDocument();
  });
});

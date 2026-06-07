import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import AdjustmentBadge from "../AdjustmentBadge";

describe("AdjustmentBadge", () => {
  it("renders positive, negative, and zero values", () => {
    const { rerender } = render(<AdjustmentBadge value={2.5} unit="kg" />);
    expect(screen.getByText("+2.50 kg")).toHaveClass("bg-green-600");

    rerender(<AdjustmentBadge value={-1.25} unit="reps" />);
    expect(screen.getByText("-1.25 reps")).toHaveClass("bg-red-600");

    rerender(<AdjustmentBadge value={0} unit="kg" />);
    expect(screen.getByText("0.00 kg")).toHaveClass("bg-muted-foreground");
  });
});

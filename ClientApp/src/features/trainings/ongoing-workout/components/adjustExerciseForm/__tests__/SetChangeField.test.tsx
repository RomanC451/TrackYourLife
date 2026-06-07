import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/components/ui/form", () => ({
  FormItem: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  FormMessage: () => null,
}));

import SetChangeField from "../SetChangeField";

describe("SetChangeField", () => {
  it("renders the current value and increments by the step", () => {
    const onChange = vi.fn();

    render(
      <SetChangeField
        originalValue={10}
        field={{
          name: "newSets.0.count1",
          value: 12,
          onChange,
          onBlur: vi.fn(),
          ref: vi.fn(),
        }}
        label="Reps"
        unit="reps"
        step={2}
      />,
    );

    expect(screen.getByText(/current: 10 reps/)).toBeInTheDocument();
    fireEvent.click(screen.getAllByRole("button")[1]);
    expect(onChange).toHaveBeenCalledWith(14);
  });

  it("decrements without going below zero", () => {
    const onChange = vi.fn();

    render(
      <SetChangeField
        originalValue={10}
        field={{
          name: "newSets.0.count1",
          value: 1,
          onChange,
          onBlur: vi.fn(),
          ref: vi.fn(),
        }}
        label="Reps"
        unit="reps"
        step={2}
      />,
    );

    fireEvent.click(screen.getAllByRole("button")[0]);
    expect(onChange).toHaveBeenCalledWith(0);
  });
});

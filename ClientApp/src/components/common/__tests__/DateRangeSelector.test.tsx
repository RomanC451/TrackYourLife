import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { DateRangeSelector } from "../DateRangeSelector";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1280 } }),
}));

describe("DateRangeSelector", () => {
  it('displays "All time" when no range is selected', () => {
    render(
      <DateRangeSelector handleRangeSelect={vi.fn()} selectedRange={undefined} />,
    );

    expect(screen.getByRole("button", { name: /all time/i })).toBeInTheDocument();
  });

  it("displays a formatted date range", () => {
    render(
      <DateRangeSelector
        handleRangeSelect={vi.fn()}
        selectedRange={{
          from: new Date("2026-01-15"),
          to: new Date("2026-02-20"),
        }}
      />,
    );

    expect(
      screen.getByRole("button", { name: /jan 15, 2026 - feb 20, 2026/i }),
    ).toBeInTheDocument();
  });

  it("disables the trigger when disabled or loading", () => {
    const { rerender } = render(
      <DateRangeSelector handleRangeSelect={vi.fn()} disabled />,
    );

    expect(screen.getByRole("button", { name: /all time/i })).toBeDisabled();

    rerender(<DateRangeSelector handleRangeSelect={vi.fn()} loading />);

    expect(screen.getByRole("button", { name: /all time/i })).toBeDisabled();
  });

  it("calls handleRangeSelect when a preset is chosen", async () => {
    const handleRangeSelect = vi.fn();

    render(<DateRangeSelector handleRangeSelect={handleRangeSelect} />);

    fireEvent.click(screen.getByRole("button", { name: /all time/i }));

    await waitFor(() => {
      expect(screen.getByRole("button", { name: "Last week" })).toBeInTheDocument();
    });

    fireEvent.click(screen.getByRole("button", { name: "Last week" }));

    expect(handleRangeSelect).toHaveBeenCalledWith(
      expect.objectContaining({
        from: expect.any(Date),
        to: expect.any(Date),
      }),
    );
  });
});

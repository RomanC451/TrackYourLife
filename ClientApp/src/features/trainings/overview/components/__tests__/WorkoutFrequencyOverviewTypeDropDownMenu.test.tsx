import { fireEvent, render, screen } from "@testing-library/react";
import { useState } from "react";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/components/ui/dropdown-menu", () => ({
  DropdownMenu: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuTrigger: ({
    children,
  }: {
    children: React.ReactNode;
  }) => <div>{children}</div>,
  DropdownMenuContent: ({ children }: { children: React.ReactNode }) => (
    <div role="menu">{children}</div>
  ),
  DropdownMenuItem: ({
    children,
    onSelect,
  }: {
    children: React.ReactNode;
    onSelect?: () => void;
  }) => (
    <button type="button" role="menuitem" onClick={() => onSelect?.()}>
      {children}
    </button>
  ),
}));

import WorkoutFrequencyOverviewTypeDropDownMenu from "../WorkoutFrequencyOverviewTypeDropDownMenu";

function Harness() {
  const [overviewType, setOverviewType] = useState<"Daily" | "Weekly" | "Monthly">(
    "Weekly",
  );

  return (
    <WorkoutFrequencyOverviewTypeDropDownMenu
      overviewType={overviewType}
      setOverviewType={setOverviewType}
    />
  );
}

describe("WorkoutFrequencyOverviewTypeDropDownMenu", () => {
  it("renders current overview type and updates selection", () => {
    render(<Harness />);

    expect(screen.getByRole("combobox")).toHaveTextContent("Weekly");
    fireEvent.click(screen.getByRole("menuitem", { name: "Monthly" }));
    expect(screen.getByRole("combobox")).toHaveTextContent("Monthly");
  });

  it("disables trigger when loading", () => {
    render(
      <WorkoutFrequencyOverviewTypeDropDownMenu
        overviewType="Daily"
        setOverviewType={vi.fn()}
        loading
      />,
    );

    expect(screen.getByRole("combobox")).toBeDisabled();
  });
});

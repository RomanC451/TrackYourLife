import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { MealTypes } from "@/services/openapi";

import MealTypeDropDownMenu from "../MealTypeDropDownMenu";

vi.mock("@/components/ui/spinner", () => ({
  default: () => <div data-testid="spinner" />,
}));

vi.mock("@/components/ui/dropdown-menu", () => ({
  DropdownMenu: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuTrigger: ({
    children,
    asChild,
  }: {
    children: React.ReactNode;
    asChild?: boolean;
  }) => (asChild ? children : <div>{children}</div>),
  DropdownMenuContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuLabel: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuItem: ({
    children,
    onClick,
  }: {
    children: React.ReactNode;
    onClick?: (e: { stopPropagation: () => void }) => void;
  }) => (
    <button
      type="button"
      onClick={() => onClick?.({ stopPropagation: () => undefined })}
    >
      {children}
    </button>
  ),
  DropdownMenuSeparator: () => <hr />,
}));

describe("MealTypeDropDownMenu", () => {
  it("renders the add meal trigger button", () => {
    render(
      <MealTypeDropDownMenu
        selectCallback={vi.fn()}
        date="2026-06-05"
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    expect(screen.getAllByRole("button")[0]).toBeEnabled();
  });

  it("shows a spinner while delayed pending", () => {
    render(
      <MealTypeDropDownMenu
        selectCallback={vi.fn()}
        date="2026-06-05"
        pendingState={{ isPending: false, isDelayedPending: true }}
      />,
    );

    expect(screen.getByTestId("spinner")).toBeInTheDocument();
  });

  it("calls selectCallback with the chosen meal type", () => {
    const selectCallback = vi.fn();

    render(
      <MealTypeDropDownMenu
        selectCallback={selectCallback}
        date="2026-06-05"
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    fireEvent.click(screen.getByText(MealTypes.Lunch));
    expect(selectCallback).toHaveBeenCalledWith(MealTypes.Lunch);
  });

  it("disables the trigger while pending", () => {
    render(
      <MealTypeDropDownMenu
        selectCallback={vi.fn()}
        date="2026-06-05"
        pendingState={{ isPending: true, isDelayedPending: false }}
      />,
    );

    expect(screen.getAllByRole("button")[0]).toBeDisabled();
  });

  it("stops mouse down propagation on the wrapper", () => {
    const onParentMouseDown = vi.fn();

    const { container } = render(
      <div onMouseDown={onParentMouseDown}>
        <MealTypeDropDownMenu
          selectCallback={vi.fn()}
          date="2026-06-05"
          pendingState={{ isPending: false, isDelayedPending: false }}
        />
      </div>,
    );

    const wrapper = container.firstChild?.firstChild as HTMLElement;
    fireEvent.mouseDown(wrapper);

    expect(onParentMouseDown).not.toHaveBeenCalled();
  });

  it("stops blur propagation on the wrapper", () => {
    const onParentBlur = vi.fn();

    const { container } = render(
      <div onBlur={onParentBlur}>
        <MealTypeDropDownMenu
          selectCallback={vi.fn()}
          date="2026-06-05"
          pendingState={{ isPending: false, isDelayedPending: false }}
        />
      </div>,
    );

    const wrapper = container.firstChild?.firstChild as HTMLElement;
    fireEvent.blur(wrapper);

    expect(onParentBlur).not.toHaveBeenCalled();
  });
});

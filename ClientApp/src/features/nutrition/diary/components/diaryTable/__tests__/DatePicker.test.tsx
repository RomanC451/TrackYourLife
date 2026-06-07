import { fireEvent, render, screen } from "@testing-library/react";

import { describe, expect, it, vi } from "vitest";



import DatePicker from "../DatePicker";



vi.mock("@/components/ui/popover", () => ({

  Popover: ({ children }: { children: React.ReactNode }) => (

    <div>{children}</div>

  ),

  PopoverTrigger: ({ children }: { children: React.ReactNode }) => (

    <>{children}</>

  ),

  PopoverContent: ({ children }: { children: React.ReactNode }) => (

    <div data-testid="popover-content">{children}</div>

  ),

}));



vi.mock("@/components/ui/calendar", () => ({

  Calendar: ({

    onSelect,

  }: {

    onSelect?: (date: Date | undefined) => void;

  }) => (

    <div>

      <button

        type="button"

        onClick={() => onSelect?.(new Date("2026-06-10T12:00:00"))}

      >

        Pick June 10

      </button>

      <button type="button" onClick={() => onSelect?.(undefined)}>

        Clear date

      </button>

    </div>

  ),

}));



describe("DatePicker", () => {

  it("renders the selected date", () => {

    render(

      <DatePicker dateOnly="2026-06-05" setDate={vi.fn()} disabled={false} />,

    );



    expect(
      screen.getByRole("button", { name: /June 5th, 2026/i }),
    ).toBeInTheDocument();

  });



  it("disables the trigger when disabled", () => {

    render(

      <DatePicker dateOnly="2026-06-05" setDate={vi.fn()} disabled={true} />,

    );



    expect(
      screen.getByRole("button", { name: /June 5th, 2026/i }),
    ).toBeDisabled();

  });



  it("calls setDate when a calendar day is selected", () => {

    const setDate = vi.fn();



    render(

      <DatePicker dateOnly="2026-06-05" setDate={setDate} disabled={false} />,

    );



    fireEvent.click(screen.getByRole("button", { name: "Pick June 10" }));



    expect(setDate).toHaveBeenCalledTimes(1);

    expect(setDate.mock.calls[0][0]).toBeInstanceOf(Date);

  });



  it("ignores a cleared calendar selection", () => {

    const setDate = vi.fn();



    render(

      <DatePicker dateOnly="2026-06-05" setDate={setDate} disabled={false} />,

    );



    fireEvent.click(screen.getByRole("button", { name: "Clear date" }));



    expect(setDate).not.toHaveBeenCalled();

  });

});



import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import GuitarTabAutoscroll, {
  changeAutoscrollSpeed,
} from "../GuitarTabAutoscroll";

describe("changeAutoscrollSpeed", () => {
  it("steps between 0.1 and 1.5", () => {
    expect(changeAutoscrollSpeed(0.2, 1)).toBe(0.3);
    expect(changeAutoscrollSpeed(0.2, -1)).toBe(0.1);
    expect(changeAutoscrollSpeed(0.1, -1)).toBe(0.1);
    expect(changeAutoscrollSpeed(1.5, 1)).toBe(1.5);
  });
});

describe("GuitarTabAutoscroll", () => {
  it("starts and pauses scrolling", () => {
    render(<GuitarTabAutoscroll />);

    fireEvent.click(screen.getByRole("button", { name: "Autoscroll" }));
    expect(screen.getByRole("button", { name: "Pause" })).toHaveAttribute(
      "aria-pressed",
      "true",
    );

    fireEvent.click(screen.getByRole("button", { name: "Pause" }));
    expect(screen.getByRole("button", { name: "Autoscroll" })).toHaveAttribute(
      "aria-pressed",
      "false",
    );
  });

  it("toggles autoscroll with the space bar", () => {
    render(<GuitarTabAutoscroll />);

    fireEvent.keyDown(window, { code: "Space", key: " " });
    expect(screen.getByRole("button", { name: "Pause" })).toHaveAttribute(
      "aria-pressed",
      "true",
    );

    fireEvent.keyDown(window, { code: "Space", key: " " });
    expect(screen.getByRole("button", { name: "Autoscroll" })).toHaveAttribute(
      "aria-pressed",
      "false",
    );
  });

  it("leaves autoscroll unchanged when space is typed in a field", () => {
    render(
      <>
        <input aria-label="Note" />
        <GuitarTabAutoscroll />
      </>,
    );

    fireEvent.keyDown(screen.getByRole("textbox", { name: "Note" }), {
      code: "Space",
      key: " ",
    });

    expect(screen.getByRole("button", { name: "Autoscroll" })).toHaveAttribute(
      "aria-pressed",
      "false",
    );
  });

  it("changes the speed with the arrow keys", () => {
    render(<GuitarTabAutoscroll />);

    fireEvent.keyDown(window, { code: "ArrowUp", key: "ArrowUp" });
    expect(screen.getByRole("spinbutton", { name: "Speed" })).toHaveAttribute(
      "aria-valuenow",
      "0.3",
    );

    fireEvent.keyDown(window, { code: "ArrowDown", key: "ArrowDown" });
    expect(screen.getByRole("spinbutton", { name: "Speed" })).toHaveAttribute(
      "aria-valuenow",
      "0.2",
    );
  });

  it("leaves the speed unchanged when an arrow key is pressed in a field", () => {
    render(
      <>
        <input aria-label="Note" />
        <GuitarTabAutoscroll />
      </>,
    );

    fireEvent.keyDown(screen.getByRole("textbox", { name: "Note" }), {
      code: "ArrowUp",
      key: "ArrowUp",
    });

    expect(screen.getByRole("spinbutton", { name: "Speed" })).toHaveAttribute(
      "aria-valuenow",
      "0.2",
    );
  });

  it("changes the speed with the plus and minus buttons", () => {
    render(<GuitarTabAutoscroll />);

    expect(screen.getByRole("spinbutton", { name: "Speed" })).toHaveAttribute(
      "aria-valuenow",
      "0.2",
    );

    fireEvent.click(screen.getByRole("button", { name: "Increase speed" }));
    expect(screen.getByRole("spinbutton", { name: "Speed" })).toHaveTextContent(
      "0.3",
    );

    fireEvent.click(screen.getByRole("button", { name: "Decrease speed" }));
    expect(screen.getByRole("spinbutton", { name: "Speed" })).toHaveTextContent(
      "0.2",
    );
  });
});
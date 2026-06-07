import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import withDate, { withOnSuccess, withProp, withProps } from "../with";

describe("with HOC helpers", () => {
  it("withDate injects a date prop", () => {
    function Component({ date, label }: { date: string; label: string }) {
      return (
        <div>
          {label}:{date}
        </div>
      );
    }

    const Wrapped = withDate(Component, "2026-06-05");
    render(<Wrapped label="Day" />);

    expect(screen.getByText("Day:2026-06-05")).toBeInTheDocument();
  });

  it("withOnSuccess injects an onSuccess callback", () => {
    let called = false;
    function Component({ onSuccess }: { onSuccess: () => void }) {
      return <button onClick={onSuccess}>Done</button>;
    }

    const Wrapped = withOnSuccess(Component, () => {
      called = true;
    });

    render(<Wrapped />);
    fireEvent.click(screen.getByRole("button", { name: "Done" }));

    expect(called).toBe(true);
  });

  it("withProp injects a single prop", () => {
    function Component({ status }: { status: string }) {
      return <span>{status}</span>;
    }

    const Wrapped = withProp(Component, "status", "ready");
    render(<Wrapped />);

    expect(screen.getByText("ready")).toBeInTheDocument();
  });

  it("withProps injects multiple props", () => {
    function Component({ a, b }: { a: string; b: string }) {
      return (
        <span>
          {a}-{b}
        </span>
      );
    }

    const Wrapped = withProps(Component, { a: "one", b: "two" });
    render(<Wrapped />);

    expect(screen.getByText("one-two")).toBeInTheDocument();
  });
});

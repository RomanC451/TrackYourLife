import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import {
  AppGeneralContextProvider,
  useAppGeneralStateContext,
} from "../AppGeneralContextProvider";

function ContextReader() {
  const { screenSize } = useAppGeneralStateContext();

  return <div>width:{screenSize.width}</div>;
}

describe("AppGeneralContextProvider", () => {
  it("provides the current screen width", () => {
    Object.defineProperty(window, "innerWidth", {
      writable: true,
      configurable: true,
      value: 1440,
    });

    render(
      <AppGeneralContextProvider>
        <ContextReader />
      </AppGeneralContextProvider>,
    );

    expect(screen.getByText("width:1440")).toBeInTheDocument();
  });

  it("throws when the hook is used outside the provider", () => {
    expect(() => render(<ContextReader />)).toThrow(
      "useAppGeneralStateContext must be used within a AppGeneralContextProvider!",
    );
  });
});

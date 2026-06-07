import { act, fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it } from "vitest";

import { ThemeProvider, useTheme } from "../theme-provider";

function ThemeReader() {
  const { theme, setTheme } = useTheme();

  return (
    <div>
      <span>theme:{theme}</span>
      <button type="button" onClick={() => setTheme("dark")}>
        Set dark
      </button>
    </div>
  );
}

describe("ThemeProvider", () => {
  beforeEach(() => {
    localStorage.clear();
    document.documentElement.className = "";
  });

  it("provides the default theme", () => {
    render(
      <ThemeProvider defaultTheme="light" storageKey="test-theme">
        <ThemeReader />
      </ThemeProvider>,
    );

    expect(screen.getByText("theme:light")).toBeInTheDocument();
  });

  it("updates theme and persists it to localStorage", () => {
    render(
      <ThemeProvider defaultTheme="light" storageKey="test-theme">
        <ThemeReader />
      </ThemeProvider>,
    );

    act(() => {
      fireEvent.click(screen.getByRole("button", { name: "Set dark" }));
    });

    expect(screen.getByText("theme:dark")).toBeInTheDocument();
    expect(localStorage.getItem("test-theme")).toBe("dark");
    expect(document.documentElement.classList.contains("dark")).toBe(true);
  });

  it("throws when useTheme is used outside the provider", () => {
    expect(() => render(<ThemeReader />)).toThrow(
      "useTheme must be used within a ThemeProvider",
    );
  });
});

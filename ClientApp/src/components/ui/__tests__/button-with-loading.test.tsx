import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import ButtonWithLoading from "../button-with-loading";

describe("ButtonWithLoading", () => {
  it("renders children when not loading", () => {
    render(<ButtonWithLoading isLoading={false}>Click me</ButtonWithLoading>);
    expect(screen.getByRole("button", { name: "Click me" })).toBeInTheDocument();
  });

  it("shows loading spinner when loading", () => {
    render(<ButtonWithLoading isLoading={true}>Click me</ButtonWithLoading>);
    expect(screen.getByRole("status")).toBeInTheDocument();
    expect(screen.getByText("Click me")).toBeInTheDocument();
  });

  it("enables button when not loading", () => {
    render(<ButtonWithLoading isLoading={false}>Click me</ButtonWithLoading>);
    expect(screen.getByRole("button")).toBeEnabled();
  });

  it("handles button click event when not loading", () => {
    const handleClick = vi.fn();
    render(
      <ButtonWithLoading isLoading={false} onClick={handleClick}>
        Click me
      </ButtonWithLoading>,
    );
    fireEvent.click(screen.getByRole("button"));
    expect(handleClick).toHaveBeenCalledTimes(1);
  });
});

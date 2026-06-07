import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import CancelableLoadingPage from "../CancelableLoadingPage";

const mockNavigateBackOrDefault = vi.fn();

vi.mock("lottie-react", () => ({
  default: () => <div data-testid="lottie-animation" />,
}));

vi.mock("@/hooks/useNavigateBackOrDefault", () => ({
  default: () => mockNavigateBackOrDefault,
}));

describe("CancelableLoadingPage", () => {
  it("renders the cancel button and loading animation", () => {
    render(<CancelableLoadingPage defaultRouteOnCancel="/home" />);

    expect(screen.getByTestId("lottie-animation")).toBeInTheDocument();
    expect(screen.getByRole("button")).toBeInTheDocument();
  });

  it("navigates back when the cancel button is clicked", () => {
    render(<CancelableLoadingPage defaultRouteOnCancel="/home" />);

    fireEvent.click(screen.getByRole("button"));

    expect(mockNavigateBackOrDefault).toHaveBeenCalledTimes(1);
  });
});

import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import LoadingPage from "../LoadingPage";

vi.mock("lottie-react", () => ({
  default: () => <div data-testid="lottie-animation" />,
}));

describe("LoadingPage", () => {
  it("renders the loading animation", () => {
    render(<LoadingPage />);

    expect(screen.getByTestId("lottie-animation")).toBeInTheDocument();
  });
});

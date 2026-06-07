import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

class ResizeObserverMock {
  observe() {}
  unobserve() {}
  disconnect() {}
}

vi.stubGlobal("ResizeObserver", ResizeObserverMock);

import { exerciseHistory } from "@/features/trainings/__tests__/fixtures";

const { mockUseCustomQuery } = vi.hoisted(() => ({
  mockUseCustomQuery: vi.fn(),
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

vi.mock("../AdjustmentSession", () => ({
  default: ({ history }: { history: { id: string } }) => (
    <div data-testid={`session-${history.id}`} />
  ),
}));

import AdjustmentsHistory from "../AdjustmentsHistory";

describe("AdjustmentsHistory", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders the loading skeleton while pending", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { isPending: true },
    });

    const { container } = render(<AdjustmentsHistory exerciseId="ex-1" />);

    expect(container.querySelector(".animate-pulse")).toBeTruthy();
    expect(screen.queryByText("No adjustments history")).not.toBeInTheDocument();
  });

  it("shows an empty state when there is no history", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { isPending: false, isSuccess: true, data: [] },
    });

    render(<AdjustmentsHistory exerciseId="ex-1" />);

    expect(screen.getByText("No adjustments history")).toBeInTheDocument();
  });

  it("shows only the newest session until expanded", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        isPending: false,
        isSuccess: true,
        data: [
          exerciseHistory("history-1"),
          exerciseHistory("history-2"),
          exerciseHistory("history-3"),
        ],
      },
    });

    render(<AdjustmentsHistory exerciseId="ex-1" />);

    expect(screen.getByTestId("session-history-1")).toBeInTheDocument();
    expect(screen.queryByTestId("session-history-2")).not.toBeInTheDocument();
    expect(screen.getByText("Show 2 more sessions")).toBeInTheDocument();
  });

  it("shows all sessions after expanding", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        isPending: false,
        isSuccess: true,
        data: [exerciseHistory("history-1"), exerciseHistory("history-2")],
      },
    });

    render(<AdjustmentsHistory exerciseId="ex-1" />);

    fireEvent.click(screen.getByText("Show 1 more sessions"));

    expect(screen.getByTestId("session-history-1")).toBeInTheDocument();
    expect(screen.getByTestId("session-history-2")).toBeInTheDocument();
    expect(screen.getByText("Show less sessions")).toBeInTheDocument();
  });
});

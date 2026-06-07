import { render, screen } from "@testing-library/react";
import { beforeAll, describe, expect, it, vi } from "vitest";

class ResizeObserverMock {
  observe() {}
  unobserve() {}
  disconnect() {}
}

beforeAll(() => {
  vi.stubGlobal("ResizeObserver", ResizeObserverMock);
});

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

import { exercise, exerciseHistory } from "@/features/trainings/__tests__/fixtures";

const { mockUseCustomQuery } = vi.hoisted(() => ({
  mockUseCustomQuery: vi.fn(),
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

vi.mock(
  "@/features/trainings/ongoing-workout/components/adjustmentsHistory/AdjustmentSession",
  () => ({
    default: ({ history }: { history: { id: string } }) => (
      <div data-testid="adjustment-session">{history.id}</div>
    ),
  }),
);

import ExerciseHistoriesDialog from "../ExerciseHistoriesDialog";

describe("ExerciseHistoriesDialog", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders pending state", () => {
    mockUseCustomQuery.mockReturnValue({ query: { isPending: true } });

    render(
      <ExerciseHistoriesDialog
        exerciseId="ex-1"
        open
        onOpenChange={vi.fn()}
      />,
    );

    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(document.querySelector(".animate-pulse")).toBeTruthy();
  });

  it("renders exercise history entries", () => {
    let call = 0;
    mockUseCustomQuery.mockImplementation(() => {
      call += 1;
      if (call % 2 === 1) {
        return {
          query: {
            isPending: false,
            isSuccess: true,
            data: exercise("ex-1", { name: "Bench press" }),
          },
        };
      }
      return {
        query: {
          isPending: false,
          isSuccess: true,
          data: [exerciseHistory("hist-1"), exerciseHistory("hist-2")],
        },
      };
    });

    render(
      <ExerciseHistoriesDialog
        exerciseId="ex-1"
        open
        onOpenChange={vi.fn()}
      />,
    );

    expect(
      screen.getByText("Bench press - Exercise History"),
    ).toBeInTheDocument();
    expect(screen.getAllByTestId("adjustment-session")).toHaveLength(2);
  });

  it("renders empty state when no history exists", () => {
    let call = 0;
    mockUseCustomQuery.mockImplementation(() => {
      call += 1;
      if (call % 2 === 1) {
        return {
          query: {
            isPending: false,
            isSuccess: true,
            data: exercise("ex-1", { name: "Squat" }),
          },
        };
      }
      return {
        query: {
          isPending: false,
          isSuccess: true,
          data: [],
        },
      };
    });

    render(
      <ExerciseHistoriesDialog
        exerciseId="ex-1"
        open
        onOpenChange={vi.fn()}
      />,
    );

    expect(
      screen.getByText("No exercise history available"),
    ).toBeInTheDocument();
  });
});

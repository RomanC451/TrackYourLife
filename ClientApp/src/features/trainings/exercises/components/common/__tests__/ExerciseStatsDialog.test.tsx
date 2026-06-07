import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

const { mockUseQuery } = vi.hoisted(() => ({
  mockUseQuery: vi.fn(),
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useQuery: (...args: unknown[]) => mockUseQuery(...args),
  };
});

vi.mock("@/pages/trainings/ExerciseStatsPage", () => ({
  default: ({ stats }: { stats: { exerciseName: string } }) => (
    <div data-testid="stats-page">{stats.exerciseName}</div>
  ),
}));

import ExerciseStatsDialog from "../ExerciseStatsDialog";

describe("ExerciseStatsDialog", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows loading skeleton while fetching", () => {
    mockUseQuery.mockReturnValue({
      isPending: true,
      data: undefined,
      isError: false,
    });

    render(
      <ExerciseStatsDialog
        exerciseId="ex-1"
        open
        onOpenChange={vi.fn()}
      />,
    );

    expect(screen.getByRole("dialog")).toBeInTheDocument();
  });

  it("renders stats page when data is available", () => {
    mockUseQuery.mockReturnValue({
      isPending: false,
      isError: false,
      data: { exerciseName: "Bench press" },
    });

    render(
      <ExerciseStatsDialog
        exerciseId="ex-1"
        open
        onOpenChange={vi.fn()}
      />,
    );

    expect(screen.getByTestId("stats-page")).toHaveTextContent("Bench press");
  });

  it("shows error message when query fails", () => {
    mockUseQuery.mockReturnValue({
      isPending: false,
      isError: true,
      data: undefined,
    });

    render(
      <ExerciseStatsDialog
        exerciseId="ex-1"
        open
        onOpenChange={vi.fn()}
      />,
    );

    expect(
      screen.getByText("Could not load exercise stats."),
    ).toBeInTheDocument();
  });
});

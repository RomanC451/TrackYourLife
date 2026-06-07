import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { training, workoutPlan } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockUseSuspenseQuery, mockUseCustomQuery } = vi.hoisted(() => ({
  mockUseSuspenseQuery: vi.fn(),
  mockUseCustomQuery: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

vi.mock("../TrainingListItem", () => ({
  default: ({
    training,
    isActive,
    isInActivePlan,
  }: {
    training: { id: string; name: string };
    isActive: boolean;
    isInActivePlan: boolean;
  }) => (
    <div data-testid="training-item">
      {training.name}
      {isActive ? " (active)" : ""}
      {isInActivePlan ? " (in plan)" : ""}
    </div>
  ),
}));

import TrainingsList from "../TrainingsList";

describe("TrainingsList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseSuspenseQuery
      .mockReturnValueOnce({
        data: [
          training("t-1", { name: "Push" }),
          training("t-2", { name: "Pull" }),
        ],
      })
      .mockReturnValueOnce({
        data: [
          workoutPlan("plan-1", {
            isActive: true,
            workouts: [training("t-1")],
          }),
        ],
      });
    mockUseCustomQuery.mockReturnValue({
      query: { data: { training: { id: "t-1" } }, isError: false },
    });
  });

  it("renders trainings with active and plan flags", () => {
    render(<TrainingsList />, { wrapper: createQueryClientWrapper() });

    expect(screen.getAllByTestId("training-item")).toHaveLength(2);
    expect(screen.getByText("Push (active) (in plan)")).toBeInTheDocument();
    expect(screen.getByText("Pull")).toBeInTheDocument();
  });

  it("treats active ongoing training query errors as no active training", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { isError: true },
    });

    render(<TrainingsList />, { wrapper: createQueryClientWrapper() });

    expect(screen.getByText("Push (in plan)")).toBeInTheDocument();
    expect(screen.queryByText("(active)")).not.toBeInTheDocument();
  });
});

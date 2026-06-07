import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { workoutHistory } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockMutate } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({
    screenSize: { width: 1400 },
    queryToolsRef: { current: null },
    routerToolsRef: { current: null },
  }),
}));

vi.mock(
  "@/features/trainings/history/mutations/useUpdateOngoingTrainingMutation",
  () => ({
    default: () => ({
      mutate: mockMutate,
      isPending: false,
    }),
  }),
);

import { EditWorkoutHistoryDialog } from "../EditWorkoutHistoryDialog";

describe("EditWorkoutHistoryDialog", () => {
  const workout = workoutHistory("wh-1", {
    trainingName: "Push day",
    caloriesBurned: 300,
    durationSeconds: 2700,
  });

  beforeEach(() => {
    vi.clearAllMocks();
    mockMutate.mockImplementation((_vars, opts) => opts?.onSuccess?.());
  });

  it("renders form with workout defaults", () => {
    render(
      <EditWorkoutHistoryDialog workout={workout} onClose={vi.fn()} />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.getByText("Edit workout session")).toBeInTheDocument();
    expect(screen.getByDisplayValue("300")).toBeInTheDocument();
    expect(screen.getByDisplayValue("45")).toBeInTheDocument();
    expect(screen.getByText(/Push day/)).toBeInTheDocument();
  });

  it("calls onClose when cancel is clicked", () => {
    const onClose = vi.fn();

    render(
      <EditWorkoutHistoryDialog workout={workout} onClose={onClose} />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: "Cancel" }));
    expect(onClose).toHaveBeenCalledOnce();
  });
});

import { fireEvent, render, screen } from "@testing-library/react";
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

import { ongoingTraining } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockMutate } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
}));

vi.mock("../../../mutations/useJumpToExerciseMutation", () => ({
  default: () => ({
    mutate: mockMutate,
    isPending: false,
  }),
}));

import ExerciseSelectionDialog from "../ExerciseSelectionDialog";

describe("ExerciseSelectionDialog", () => {
  const session = ongoingTraining({
    completedExerciseIds: ["ex-1"],
    skippedExerciseIds: ["ex-2"],
    exerciseIndex: 0,
  });

  beforeEach(() => {
    vi.clearAllMocks();
    mockMutate.mockImplementation((_vars, opts) => opts?.onSuccess?.());
  });

  it("renders exercises with status indicators", () => {
    render(
      <ExerciseSelectionDialog
        open
        onOpenChange={vi.fn()}
        ongoingTraining={session}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.getByText("Choose next exercise")).toBeInTheDocument();
    expect(screen.getByText("Bench press")).toBeInTheDocument();
    expect(screen.getByText("Squat")).toBeInTheDocument();
    expect(screen.getByText("Current")).toBeInTheDocument();
  });

  it("closes without mutation when selecting current exercise", () => {
    const onOpenChange = vi.fn();

    render(
      <ExerciseSelectionDialog
        open
        onOpenChange={onOpenChange}
        ongoingTraining={session}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByText("Bench press"));
    expect(mockMutate).not.toHaveBeenCalled();
    expect(onOpenChange).toHaveBeenCalledWith(false);
  });

  it("jumps to a different exercise on selection", () => {
    const onOpenChange = vi.fn();
    const onSelectionSuccess = vi.fn();

    render(
      <ExerciseSelectionDialog
        open
        onOpenChange={onOpenChange}
        ongoingTraining={session}
        onSelectionSuccess={onSelectionSuccess}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByText("Squat"));
    expect(mockMutate).toHaveBeenCalledWith(
      { ongoingTraining: session, exerciseIndex: 1 },
      expect.objectContaining({ onSuccess: expect.any(Function) }),
    );
    expect(onOpenChange).toHaveBeenCalledWith(false);
    expect(onSelectionSuccess).toHaveBeenCalledOnce();
  });

  it("hides current status when hideCurrentStatus is true", () => {
    render(
      <ExerciseSelectionDialog
        open
        onOpenChange={vi.fn()}
        ongoingTraining={session}
        hideCurrentStatus
      />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.queryByText("Current")).not.toBeInTheDocument();
  });
});

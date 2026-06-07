import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { exercise, training } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const {
  mockNavigate,
  mockPreloadRoute,
  mockCreateMutate,
  mockDeleteMutate,
  mockToastError,
} = vi.hoisted(() => ({
  mockNavigate: vi.fn(),
  mockPreloadRoute: vi.fn(),
  mockCreateMutate: vi.fn(),
  mockDeleteMutate: vi.fn(),
  mockToastError: vi.fn(),
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("@/App", () => ({
  router: { preloadRoute: mockPreloadRoute },
}));

vi.mock("sonner", () => ({
  toast: { error: mockToastError },
}));

vi.mock(
  "@/features/trainings/ongoing-workout/mutations/useCreateOngoingTrainingMutation",
  () => ({
    default: () => ({
      mutate: mockCreateMutate,
      isPending: false,
      isDelayedPending: false,
    }),
  }),
);

vi.mock(
  "@/features/trainings/ongoing-workout/mutations/useDeleteOngoingTrainingMutation",
  () => ({
    default: () => ({
      mutate: mockDeleteMutate,
      isPending: false,
      isDelayedPending: false,
    }),
  }),
);

vi.mock("../../common/DeleteTrainingAlert", () => ({
  default: ({
    open,
    onOpenChange,
  }: {
    open: boolean;
    onOpenChange: (open: boolean) => void;
  }) =>
    open ? (
      <div data-testid="delete-alert">
        <button type="button" onClick={() => onOpenChange(false)}>
          Close delete
        </button>
      </div>
    ) : null,
}));

import TrainingListItem from "../TrainingListItem";

describe("TrainingListItem", () => {
  const workout = training("t-1", {
    name: "Push day",
    duration: 45,
    exercises: [
      exercise("ex-1", { name: "Bench press" }),
      exercise("ex-2", { name: "Flyes" }),
      exercise("ex-3", { name: "Dips" }),
      exercise("ex-4", { name: "Push-ups" }),
    ],
  });

  beforeEach(() => {
    vi.clearAllMocks();
    mockCreateMutate.mockImplementation((_vars, opts) => opts?.onSuccess?.());
  });

  it("renders training details and exercise preview", () => {
    render(
      <TrainingListItem
        training={workout}
        isActive={false}
        isInActivePlan={false}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.getByText("Push day")).toBeInTheDocument();
    expect(screen.getByText("chest")).toBeInTheDocument();
    expect(screen.getByText("4 exercises")).toBeInTheDocument();
    expect(screen.getByText("Bench press")).toBeInTheDocument();
    expect(screen.getByText("+1 more exercise")).toBeInTheDocument();
  });

  it("shows active plan badge when in active plan", () => {
    render(
      <TrainingListItem training={workout} isActive={false} isInActivePlan />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.getByText("In active plan")).toBeInTheDocument();
  });

  it("starts an ongoing training and navigates on success", () => {
    render(
      <TrainingListItem
        training={workout}
        isActive={false}
        isInActivePlan={false}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: /Start/i }));
    expect(mockCreateMutate).toHaveBeenCalledWith(
      { trainingId: "t-1" },
      expect.objectContaining({ onSuccess: expect.any(Function) }),
    );
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/trainings/ongoing-workout",
    });
  });

  it("continues an active training without creating a new session", () => {
    render(
      <TrainingListItem training={workout} isActive isInActivePlan={false} />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.getByRole("button", { name: /Continue/i })).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: /Continue/i }));
    expect(mockCreateMutate).not.toHaveBeenCalled();
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/trainings/ongoing-workout",
    });
  });

  it("cancels an active training", () => {
    render(
      <TrainingListItem training={workout} isActive isInActivePlan={false} />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: /Cancel/i }));
    expect(mockDeleteMutate).toHaveBeenCalledWith({
      ongoingTrainingId: "t-1",
    });
  });
});

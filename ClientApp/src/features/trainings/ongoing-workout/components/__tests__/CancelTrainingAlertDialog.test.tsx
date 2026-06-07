import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { ongoingTraining } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

const { mockUseSuspenseQuery, mockNavigate, mockDeleteMutate } = vi.hoisted(
  () => ({
    mockUseSuspenseQuery: vi.fn(),
    mockNavigate: vi.fn(),
    mockDeleteMutate: vi.fn(),
  }),
);

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock(
  "../../mutations/useDeleteOngoingTrainingMutation",
  () => ({
    default: () => ({
      mutate: mockDeleteMutate,
      isPending: false,
      isDelayedPending: false,
    }),
  }),
);

import CancelTrainingAlertDialog from "../CancelTrainingAlertDialog";

describe("CancelTrainingAlertDialog", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseSuspenseQuery.mockReturnValue({ data: ongoingTraining() });
    mockDeleteMutate.mockImplementation((_vars, opts) => opts?.onSuccess?.());
  });

  it("cancels training and navigates away", () => {
    const onOpenChange = vi.fn();

    render(
      <CancelTrainingAlertDialog open onOpenChange={onOpenChange} />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(
      screen.getByRole("heading", { name: "Cancel Training" }),
    ).toBeInTheDocument();
    fireEvent.click(
      screen.getByRole("button", { name: "Cancel Training" }),
    );
    expect(mockDeleteMutate).toHaveBeenCalledWith(
      { ongoingTrainingId: "training-1" },
      expect.objectContaining({ onSuccess: expect.any(Function) }),
    );
    expect(onOpenChange).toHaveBeenCalledWith(false);
    expect(mockNavigate).toHaveBeenCalledWith({ to: "/trainings/workouts" });
  });
});

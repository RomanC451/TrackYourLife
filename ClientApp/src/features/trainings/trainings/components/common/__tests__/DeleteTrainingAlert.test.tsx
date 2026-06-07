import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { training } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockMutate } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

vi.mock("../../../mutations/useDeleteTrainingMutation", () => ({
  default: () => ({
    mutate: mockMutate,
    isPending: false,
    isDelayedPending: false,
  }),
}));

import DeleteTrainingAlert from "../DeleteTrainingAlert";

describe("DeleteTrainingAlert", () => {
  const workout = training("t-1", { name: "Leg day" });

  beforeEach(() => {
    vi.clearAllMocks();
    mockMutate.mockImplementation((_vars, opts) => opts?.onSuccess?.());
  });

  it("renders uncontrolled trigger and deletes on confirm", () => {
    render(<DeleteTrainingAlert training={workout} />, {
      wrapper: createQueryClientWrapper(),
    });

    fireEvent.click(screen.getByRole("button", { name: /^Delete$/i }));
    expect(screen.getByRole("heading", { name: "Delete Training" })).toBeInTheDocument();

    fireEvent.click(
      screen.getByRole("button", { name: "Delete training" }),
    );
    expect(mockMutate).toHaveBeenCalledWith(
      { id: "t-1", name: "Leg day", force: false },
      expect.objectContaining({ onSuccess: expect.any(Function) }),
    );
  });

  it("shows force warning for active training in controlled mode", () => {
    const onOpenChange = vi.fn();

    render(
      <DeleteTrainingAlert
        training={workout}
        force
        open
        onOpenChange={onOpenChange}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(
      screen.getByText(/The training "Leg day" is currently active/i),
    ).toBeInTheDocument();
  });
});

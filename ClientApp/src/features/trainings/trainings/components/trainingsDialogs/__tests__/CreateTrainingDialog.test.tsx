import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("../../../mutations/useCreateTrainingMutation", () => ({
  default: () => ({
    mutate: vi.fn(),
    pendingState: { isPending: false, isDelayedPending: false },
  }),
}));

vi.mock("../TrainingDialog", () => ({
  default: ({ dialogType }: { dialogType: string }) => (
    <div data-testid="training-dialog">{dialogType}</div>
  ),
}));

import CreateTrainingDialog from "../CreateTrainingDialog";

describe("CreateTrainingDialog", () => {
  it("renders create training dialog", () => {
    render(<CreateTrainingDialog />);
    expect(screen.getByTestId("training-dialog")).toHaveTextContent("create");
  });
});

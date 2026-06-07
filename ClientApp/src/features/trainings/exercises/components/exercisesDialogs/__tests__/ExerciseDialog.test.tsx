import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

const { mockHandleCustomSubmit, mockReset } = vi.hoisted(() => ({
  mockHandleCustomSubmit: vi.fn(),
  mockReset: vi.fn(),
}));

vi.mock("../useExerciseDialog", () => ({
  default: ({ onSuccess }: { onSuccess: () => void }) => ({
    handleCustomSubmit: mockHandleCustomSubmit,
    form: {
      reset: mockReset,
      control: {},
    },
    onSuccess,
  }),
}));

vi.mock("../ExerciseForm", () => ({
  default: ({
    submitButtonText,
    tab,
    setTab,
  }: {
    submitButtonText: string;
    tab: string;
    setTab: (tab: string) => void;
  }) => (
    <div>
      <span>{submitButtonText}</span>
      <span data-testid="tab">{tab}</span>
      <button type="button" onClick={() => setTab("sets")}>
        Switch tab
      </button>
    </div>
  ),
}));

import ExerciseDialog from "../ExerciseDialog";

describe("ExerciseDialog", () => {
  const mutation = {
    mutate: vi.fn(),
    pendingState: { isPending: false, isDelayedPending: false },
  };

  const defaultValues = {
    id: "",
    name: "Bench press",
    muscleGroups: ["chest"],
    difficulty: "Easy" as const,
    description: "",
    equipment: "",
    videoUrl: "",
    pictureUrl: "",
    exerciseSets: [
      {
        id: "set-1",
        name: "Set 1",
        orderIndex: 0,
        count1: 10,
        unit1: "reps",
      },
    ],
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders create dialog content", () => {
    render(
      <ExerciseDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={defaultValues}
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    expect(
      screen.getByRole("heading", { name: "Create New Exercise" }),
    ).toBeInTheDocument();
    expect(screen.getByText("Create")).toBeInTheDocument();
  });

  it("renders edit dialog content", () => {
    render(
      <ExerciseDialog
        dialogType="edit"
        mutation={mutation as never}
        defaultValues={{ ...defaultValues, id: "ex-1" }}
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    expect(
      screen.getByRole("heading", { name: "Edit Exercise" }),
    ).toBeInTheDocument();
    expect(screen.getByText("Save")).toBeInTheDocument();
  });

  it("resets form and calls onClose when dialog closes", () => {
    const onClose = vi.fn();

    render(
      <ExerciseDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={defaultValues}
        pendingState={{ isPending: false, isDelayedPending: false }}
        onClose={onClose}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Switch tab" }));
    expect(screen.getByTestId("tab")).toHaveTextContent("sets");

    fireEvent.keyDown(document, { key: "Escape" });
    expect(mockReset).toHaveBeenCalledWith(defaultValues);
    expect(onClose).toHaveBeenCalled();
  });
});

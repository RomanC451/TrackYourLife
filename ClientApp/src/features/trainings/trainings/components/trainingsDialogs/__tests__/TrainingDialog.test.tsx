import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

const { mockNavigate, mockUseSearch, mockUseSuspenseQuery } = vi.hoisted(() => ({
  mockNavigate: vi.fn(),
  mockUseSearch: vi.fn(),
  mockUseSuspenseQuery: vi.fn(),
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
  useRouter: () => ({ history: { location: { pathname: "/create" } } }),
  useSearch: (...args: unknown[]) => mockUseSearch(...args),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

vi.mock("../useTrainingDialog", () => ({
  default: () => ({
    handleCustomSubmit: vi.fn(),
    form: {
      isDirty: false,
      resetSessionStorage: vi.fn(),
      control: {},
    },
  }),
}));

vi.mock("../TrainingForm", () => ({
  default: ({
    submitButtonText,
    onCancel,
  }: {
    submitButtonText: string;
    onCancel: () => void;
  }) => (
    <div>
      <span>{submitButtonText}</span>
      <button type="button" onClick={onCancel}>
        Cancel form
      </button>
    </div>
  ),
}));

import TrainingDialog from "../TrainingDialog";

describe("TrainingDialog", () => {
  const mutation = {
    mutate: vi.fn(),
    pendingState: { isPending: false, isDelayedPending: false },
  };

  beforeEach(() => {
    vi.clearAllMocks();
    mockUseSearch.mockReturnValue({ tab: "details" });
    mockUseSuspenseQuery.mockReturnValue({ data: [] });
  });

  it("renders create dialog content", () => {
    const onClose = vi.fn();

    render(
      <TrainingDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={{
          name: "",
          muscleGroups: [],
          difficulty: "Easy",
          description: "",
          duration: 0,
          restSeconds: 30,
          exercises: [],
        }}
        onClose={onClose}
      />,
    );

    expect(
      screen.getByRole("heading", { name: "Create New Workout" }),
    ).toBeInTheDocument();
    expect(screen.getByText("Create")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Cancel form" }));
    expect(onClose).toHaveBeenCalledOnce();
  });
});

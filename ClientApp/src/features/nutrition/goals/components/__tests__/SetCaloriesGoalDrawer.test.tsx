import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { macroGoals } from "@/features/nutrition/__tests__/fixtures";

import { SetCaloriesGoalDrawer } from "../SetCaloriesGoalDrawer";

const mockMutateAsync = vi.fn().mockResolvedValue(undefined);
const mockMutationState = vi.hoisted(() => ({
  isPending: false,
  isError: false,
  error: undefined as string | undefined,
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: () => ({
    query: { data: macroGoals() },
    pendingState: { isPending: false },
  }),
}));

vi.mock("../../mutations/useUpdateCaloriesGoalMutation", () => ({
  default: () => ({
    setCaloriesGoalMutation: {
      mutateAsync: mockMutateAsync,
      pendingState: { isPending: mockMutationState.isPending },
      isError: mockMutationState.isError,
    },
    error: mockMutationState.error,
  }),
}));

vi.mock("recharts", () => ({
  ResponsiveContainer: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="chart">{children}</div>
  ),
  BarChart: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="bar-chart">{children}</div>
  ),
  Bar: () => <div data-testid="bar" />,
}));

describe("SetCaloriesGoalDrawer", () => {
  beforeEach(() => {
    mockMutationState.isPending = false;
    mockMutationState.isError = false;
    mockMutationState.error = undefined;
    mockMutateAsync.mockResolvedValue(undefined);
  });

  it("loads the current calories goal into the drawer", () => {
    render(<SetCaloriesGoalDrawer />);

    fireEvent.click(screen.getByRole("button", { name: "Set goal" }));

    expect(screen.getByDisplayValue("2000")).toBeInTheDocument();
    expect(screen.getByText("Calories/day")).toBeInTheDocument();
  });

  it("submits the adjusted calories goal", () => {
    render(<SetCaloriesGoalDrawer />);

    fireEvent.click(screen.getByRole("button", { name: "Set goal" }));
    fireEvent.click(screen.getByRole("button", { name: "Increase" }));
    fireEvent.click(screen.getByRole("button", { name: "Submit" }));

    expect(mockMutateAsync).toHaveBeenCalledWith(
      { value: 2050 },
      expect.objectContaining({ onSuccess: expect.any(Function) }),
    );
  });

  it("decreases the goal with the minus button", () => {
    render(<SetCaloriesGoalDrawer />);

    fireEvent.click(screen.getByRole("button", { name: "Set goal" }));
    fireEvent.click(screen.getByRole("button", { name: "Decrease" }));

    expect(screen.getByDisplayValue("1950")).toBeInTheDocument();
  });

  it("adjusts the goal with scroll wheel events", () => {
    render(<SetCaloriesGoalDrawer />);

    fireEvent.click(screen.getByRole("button", { name: "Set goal" }));
    const valueContainer = screen.getByDisplayValue("2000").parentElement!;

    fireEvent.wheel(valueContainer, { deltaY: -100 });
    expect(screen.getByDisplayValue("2010")).toBeInTheDocument();

    fireEvent.wheel(valueContainer, { deltaY: 100 });
    expect(screen.getByDisplayValue("2000")).toBeInTheDocument();
  });

  it("sanitizes manual input and ignores invalid characters", () => {
    render(<SetCaloriesGoalDrawer />);

    fireEvent.click(screen.getByRole("button", { name: "Set goal" }));
    fireEvent.change(screen.getByDisplayValue("2000"), {
      target: { value: "21abc.5.6" },
    });

    expect(screen.getByDisplayValue("21.56")).toBeInTheDocument();
  });

  it("shows a mutation error message", () => {
    mockMutationState.error = "Value must be greater than 0.";
    mockMutationState.isError = true;

    render(<SetCaloriesGoalDrawer />);
    fireEvent.click(screen.getByRole("button", { name: "Set goal" }));

    expect(
      screen.getByText("Value must be greater than 0."),
    ).toBeInTheDocument();
  });

  it("invokes the submit onSuccess callback after a successful save", () => {
    const onSuccess = vi.fn();
    mockMutateAsync.mockImplementation(
      (_vars, options?: { onSuccess?: () => void }) => {
        options?.onSuccess?.();
        onSuccess();
        return Promise.resolve(undefined);
      },
    );

    render(<SetCaloriesGoalDrawer />);

    fireEvent.click(screen.getByRole("button", { name: "Set goal" }));
    fireEvent.click(screen.getByRole("button", { name: "Submit" }));

    expect(mockMutateAsync).toHaveBeenCalledWith(
      { value: 2000 },
      expect.objectContaining({ onSuccess: expect.any(Function) }),
    );
    expect(onSuccess).toHaveBeenCalled();
  });
});

import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import {
  ActivityLevel,
  FitnessGoal,
  Gender,
} from "@/services/openapi";

import CalculateNutritionGoalsForm from "../CalculateNutritionGoalsForm";

const { mockMutate, mockOnSuccess } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
  mockOnSuccess: vi.fn(),
}));

vi.mock("@/contexts/AuthenticationContextProvider", () => ({
  useAuthenticationContext: () => ({ userData: { id: "user-1" } }),
}));

const mockMutationState = vi.hoisted(() => ({
  isPending: false,
  isDelayedPending: false,
}));

vi.mock(
  "@/features/nutrition/goals/mutations/useCalculateNutritionGoalsMutation",
  () => ({
    default: () => ({
      mutate: mockMutate,
      get isPending() {
        return mockMutationState.isPending;
      },
      get isDelayedPending() {
        return mockMutationState.isDelayedPending;
      },
      pendingState: mockMutationState,
    }),
  }),
);

vi.mock("../AgeFormField", () => ({ default: () => <div>Age</div> }));
vi.mock("../WeightFormField", () => ({ default: () => <div>Weight</div> }));
vi.mock("../HeightFormField", () => ({ default: () => <div>Height</div> }));
vi.mock("../GenderFormSelect", () => ({ default: () => <div>Gender</div> }));
vi.mock("../ActivityLevelFormSelect", () => ({
  default: () => <div>Activity</div>,
}));
vi.mock("../FitnessGoalFormSelect", () => ({ default: () => <div>Goal</div> }));

const mockHandleSubmit = vi.hoisted(() =>
  vi.fn(
    (onValid: (values: unknown) => void) => (event?: { preventDefault?: () => void }) => {
      event?.preventDefault?.();
      onValid({
        age: "30",
        weight: "75",
        height: "180",
        gender: Gender.Male,
        activityLevel: ActivityLevel.Sedentary,
        fitnessGoal: FitnessGoal.Maintain,
      });
    },
  ),
);

const mockWatch = vi.hoisted(() =>
  vi.fn((callback?: (value: unknown) => void) => {
    callback?.({ age: "30", weight: "75" });
    return { unsubscribe: vi.fn() };
  }),
);

const capturedDefaultValues = vi.hoisted(() => ({
  current: undefined as Record<string, unknown> | undefined,
}));

vi.mock("react-hook-form", async (importOriginal) => {
  const actual = await importOriginal<typeof import("react-hook-form")>();
  return {
    ...actual,
    useForm: (options?: { defaultValues?: Record<string, unknown> }) => {
      capturedDefaultValues.current = options?.defaultValues;
      return {
        control: {},
        handleSubmit: mockHandleSubmit,
        watch: mockWatch,
      };
    },
  };
});

describe("CalculateNutritionGoalsForm", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    localStorage.clear();
    mockMutationState.isPending = false;
    mockMutationState.isDelayedPending = false;
    mockMutate.mockImplementation(
      (_vars, options?: { onSuccess?: () => void }) => {
        options?.onSuccess?.();
      },
    );
  });

  it("renders calculator fields and submit button", () => {
    render(<CalculateNutritionGoalsForm onSuccess={mockOnSuccess} />);

    expect(screen.getByText("Age")).toBeInTheDocument();
    expect(screen.getByText("Weight")).toBeInTheDocument();
    expect(screen.getByText("Height")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Calculate" })).toBeInTheDocument();
  });

  it("persists form values to localStorage when they change", async () => {
    const setItemSpy = vi.spyOn(Storage.prototype, "setItem");

    render(<CalculateNutritionGoalsForm onSuccess={mockOnSuccess} />);

    await waitFor(() => {
      expect(setItemSpy).toHaveBeenCalledWith(
        "nutrition-calculator-form-user-1",
        expect.any(String),
      );
    });
  });

  it("submits calculator values and calls onSuccess", () => {
    render(<CalculateNutritionGoalsForm onSuccess={mockOnSuccess} />);

    fireEvent.click(screen.getByRole("button", { name: "Calculate" }));

    expect(mockMutate).toHaveBeenCalledWith(
      expect.objectContaining({
        age: "30",
        weight: "75",
        height: "180",
        force: true,
      }),
      expect.objectContaining({ onSuccess: expect.any(Function) }),
    );
    expect(mockOnSuccess).toHaveBeenCalled();
  });

  it("restores default values when localStorage is empty", () => {
    render(<CalculateNutritionGoalsForm onSuccess={mockOnSuccess} />);

    expect(mockWatch).toHaveBeenCalled();
  });

  it("restores saved calculator values from localStorage", () => {
    localStorage.setItem(
      "nutrition-calculator-form-user-1",
      JSON.stringify({
        age: "28",
        weight: "82",
        height: "178",
        gender: Gender.Female,
        activityLevel: ActivityLevel.ModeratelyActive,
        fitnessGoal: FitnessGoal.Maintain,
      }),
    );

    render(<CalculateNutritionGoalsForm onSuccess={mockOnSuccess} />);

    expect(capturedDefaultValues.current).toMatchObject({
      age: "28",
      weight: "82",
      height: "178",
      gender: Gender.Female,
      activityLevel: ActivityLevel.ModeratelyActive,
      fitnessGoal: FitnessGoal.Maintain,
    });
  });

  it("disables submit while the calculator mutation is pending", () => {
    mockMutationState.isPending = true;

    render(<CalculateNutritionGoalsForm onSuccess={mockOnSuccess} />);

    expect(screen.getByRole("button", { name: "Calculate" })).toBeDisabled();
  });
});

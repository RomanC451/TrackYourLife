import { act, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { recipe, servingSize } from "@/features/nutrition/__tests__/fixtures";
import { MealTypes } from "@/services/openapi";

import { RecipeDiaryDialog } from "../RecipeDiaryDialog";

const mockFormWatch = vi.hoisted(() => vi.fn());

vi.mock("@/components/ui/dialog", () => ({
  Dialog: ({
    children,
    onOpenChange,
  }: {
    children: React.ReactNode;
    onOpenChange?: (open: boolean) => void;
  }) => (
    <div>
      <button type="button" onClick={() => onOpenChange?.(false)}>
        Close dialog
      </button>
      {children}
    </div>
  ),
  DialogContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DialogHeader: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DialogTitle: ({ children }: { children: React.ReactNode }) => (
    <h1>{children}</h1>
  ),
  DialogDescription: ({ children }: { children: React.ReactNode }) => (
    <p>{children}</p>
  ),
}));

vi.mock("../RecipeDiaryForm", () => ({
  RecipeDiaryForm: ({ submitButtonText }: { submitButtonText: string }) => (
    <button type="submit">{submitButtonText}</button>
  ),
}));

const mockMacrosHeader = vi.hoisted(() => vi.fn());

vi.mock("@/features/nutrition/common/components/macros/MacrosDialogHeader", () => ({
  default: (props: { nutritionMultiplier: number }) => {
    mockMacrosHeader(props);
    return <div data-testid="macros-header" />;
  },
}));

const mockFormReset = vi.fn();
const capturedDialogArgs = vi.hoisted(() => ({
  onSuccess: null as (() => void) | null,
}));

vi.mock("../useRecipeDiaryDialog", () => ({
  default: (args: { onSuccess: () => void }) => {
    capturedDialogArgs.onSuccess = args.onSuccess;
    return {
      handleCustomSubmit: vi.fn(),
      form: {
        watch: (callback: (values: unknown, info: { name?: string }) => void) => {
          mockFormWatch(callback);
          return { unsubscribe: vi.fn() };
        },
        reset: mockFormReset,
      },
    };
  },
}));

describe("RecipeDiaryDialog", () => {
  beforeEach(() => {
    mockMacrosHeader.mockClear();
    mockFormWatch.mockClear();
  });

  const breakfastBowl = recipe("recipe-1", "Breakfast bowl");
  breakfastBowl.servingSizes = [servingSize("ss-1", 1.5)];

  const mutation = {
    mutateAsync: vi.fn(),
    pendingState: { isPending: false },
  };
  const onSuccess = vi.fn();
  const onClose = vi.fn();

  it("renders create dialog content", () => {
    render(
      <RecipeDiaryDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={{
          recipeId: "recipe-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 1,
          entryDate: "2026-06-05",
        }}
        recipe={breakfastBowl}
        onSuccess={onSuccess}
        onClose={onClose}
      />,
    );

    expect(
      screen.getByRole("heading", { name: "Add recipe diary" }),
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Add" })).toBeInTheDocument();
    expect(screen.getByTestId("macros-header")).toBeInTheDocument();
  });

  it("renders edit dialog content and handles close", () => {
    const onClose = vi.fn();

    render(
      <RecipeDiaryDialog
        dialogType="edit"
        mutation={mutation as never}
        defaultValues={{
          id: "diary-1",
          recipeId: "recipe-1",
          mealType: MealTypes.Lunch,
          servingSizeId: "ss-1",
          quantity: 1,
          entryDate: "2026-06-05",
        }}
        recipe={breakfastBowl}
        onClose={onClose}
        onSuccess={onSuccess}
      />,
    );

    expect(
      screen.getByRole("heading", { name: "Edit recipe diary" }),
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Save" })).toBeInTheDocument();

    screen.getByRole("button", { name: "Close dialog" }).click();
    expect(onClose).toHaveBeenCalled();
  });

  it("updates nutrition multiplier when serving size or quantity changes", () => {
    render(
      <RecipeDiaryDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={{
          recipeId: "recipe-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 1,
          entryDate: "2026-06-05",
        }}
        recipe={breakfastBowl}
        onSuccess={onSuccess}
        onClose={onClose}
      />,
    );

    act(() => {
      mockFormWatch.mock.calls[0][0](
        { servingSizeId: "ss-1", quantity: 2 },
        { name: "quantity" },
      );
    });

    expect(mockMacrosHeader).toHaveBeenLastCalledWith(
      expect.objectContaining({ nutritionMultiplier: 3 }),
    );
  });

  it("ignores unrelated form watch updates", () => {
    render(
      <RecipeDiaryDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={{
          recipeId: "recipe-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 1,
          entryDate: "2026-06-05",
        }}
        recipe={breakfastBowl}
        onSuccess={onSuccess}
        onClose={onClose}
      />,
    );

    const callsBefore = mockMacrosHeader.mock.calls.length;

    act(() => {
      mockFormWatch.mock.calls.at(-1)![0](
        { servingSizeId: "ss-1", quantity: 2 },
        { name: "mealType" },
      );
    });

    expect(mockMacrosHeader.mock.calls.length).toBe(callsBefore);
  });

  it("falls back to the first serving size when the watched id is unknown", () => {
    render(
      <RecipeDiaryDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={{
          recipeId: "recipe-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 1,
          entryDate: "2026-06-05",
        }}
        recipe={breakfastBowl}
        onSuccess={onSuccess}
        onClose={onClose}
      />,
    );

    act(() => {
      mockFormWatch.mock.calls.at(-1)![0](
        { servingSizeId: "missing", quantity: 2 },
        { name: "quantity" },
      );
    });

    expect(mockMacrosHeader).toHaveBeenLastCalledWith(
      expect.objectContaining({ nutritionMultiplier: 3 }),
    );
  });

  it("resets the form and calls onSuccess after a successful submission", () => {
    const onSuccess = vi.fn();
    const defaultValues = {
      recipeId: "recipe-1",
      mealType: MealTypes.Breakfast,
      servingSizeId: "ss-1",
      quantity: 1,
      entryDate: "2026-06-05",
    };

    render(
      <RecipeDiaryDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={defaultValues}
        recipe={breakfastBowl}
        onSuccess={onSuccess}
        onClose={onClose}
      />,
    );

    capturedDialogArgs.onSuccess?.();

    expect(mockFormReset).toHaveBeenCalledWith(defaultValues);
    expect(onSuccess).toHaveBeenCalled();
  });
});

import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { food, servingSize } from "@/features/nutrition/__tests__/fixtures";
import { MealTypes } from "@/services/openapi";

import { FoodDiaryDialog } from "../FoodDiaryDialog";

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

vi.mock("../FoodDiaryForm", () => ({
  default: ({ submitButtonText }: { submitButtonText: string }) => (
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

const mockHandleCustomSubmit = vi.fn();
const mockFormReset = vi.fn();
const mockWatch = vi.fn((cb: (values: unknown, meta: unknown) => void) => {
  cb(
    { servingSizeId: "ss-1", quantity: 2 },
    { name: "quantity" },
  );
  return { unsubscribe: vi.fn() };
});
const capturedDialogArgs = vi.hoisted(() => ({
  onSuccess: null as (() => void) | null,
}));

vi.mock("../useFoodDiaryDialog", () => ({
  default: (args: { onSuccess: () => void }) => {
    capturedDialogArgs.onSuccess = args.onSuccess;
    return {
      handleCustomSubmit: mockHandleCustomSubmit,
      form: {
        watch: mockWatch,
        reset: mockFormReset,
      },
    };
  },
}));

describe("FoodDiaryDialog", () => {
  const oats = food("food-1", "Oats");
  oats.servingSizes = [servingSize("ss-1", 1.5)];

  const mutation = {
    mutateAsync: vi.fn(),
    pendingState: { isPending: false },
  };

  it("renders create dialog content", () => {
    render(
      <FoodDiaryDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={{
          foodId: "food-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 1,
          entryDate: "2026-06-05",
        }}
        food={oats}
      />,
    );

    expect(screen.getByText("Add food diary")).toBeInTheDocument();
    expect(screen.getByTestId("macros-header")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Add" })).toBeInTheDocument();
  });

  it("renders edit dialog content", () => {
    render(
      <FoodDiaryDialog
        dialogType="edit"
        mutation={mutation as never}
        defaultValues={{
          id: "diary-1",
          foodId: "food-1",
          mealType: MealTypes.Lunch,
          servingSizeId: "ss-1",
          quantity: 2,
          entryDate: "2026-06-05",
        }}
        food={oats}
      />,
    );

    expect(screen.getByText("Edit food diary")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Save" })).toBeInTheDocument();
  });

  it("calls onClose when the dialog closes", () => {
    const onClose = vi.fn();

    render(
      <FoodDiaryDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={{
          foodId: "food-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 1,
          entryDate: "2026-06-05",
        }}
        food={oats}
        onClose={onClose}
      />,
    );

    screen.getByRole("button", { name: "Close dialog" }).click();
    expect(onClose).toHaveBeenCalled();
  });

  it("ignores unrelated form watch updates", () => {
    mockWatch.mockImplementation((cb: (values: unknown, meta: unknown) => void) => {
      cb({ servingSizeId: "ss-1", quantity: 2 }, { name: "mealType" });
      return { unsubscribe: vi.fn() };
    });

    render(
      <FoodDiaryDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={{
          foodId: "food-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 1,
          entryDate: "2026-06-05",
        }}
        food={oats}
      />,
    );

    expect(mockWatch).toHaveBeenCalled();
  });

  it("falls back to the first serving size when the watched id is unknown", () => {
    mockWatch.mockImplementation((cb: (values: unknown, meta: unknown) => void) => {
      cb({ servingSizeId: "missing", quantity: 2 }, { name: "servingSizeId" });
      return { unsubscribe: vi.fn() };
    });

    render(
      <FoodDiaryDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={{
          foodId: "food-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 1,
          entryDate: "2026-06-05",
        }}
        food={oats}
      />,
    );

    expect(mockMacrosHeader).toHaveBeenCalledWith(
      expect.objectContaining({ nutritionMultiplier: 3 }),
    );
  });

  it("resets the form and calls onSuccess after a successful submission", () => {
    const onSuccess = vi.fn();
    const defaultValues = {
      foodId: "food-1",
      mealType: MealTypes.Breakfast,
      servingSizeId: "ss-1",
      quantity: 1,
      entryDate: "2026-06-05",
    };

    render(
      <FoodDiaryDialog
        dialogType="create"
        mutation={mutation as never}
        defaultValues={defaultValues}
        food={oats}
        onSuccess={onSuccess}
      />,
    );

    capturedDialogArgs.onSuccess?.();

    expect(mockFormReset).toHaveBeenCalledWith(defaultValues);
    expect(onSuccess).toHaveBeenCalled();
  });
});

import { act, renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { MealTypes } from "@/services/openapi";

import useFoodDiaryDialog from "../useFoodDiaryDialog";

describe("useFoodDiaryDialog", () => {
  const defaultValues = {
    id: "diary-1",
    foodId: "food-1",
    mealType: MealTypes.Breakfast,
    servingSizeId: "ss-1",
    quantity: 2,
    entryDate: "2026-06-05",
  };

  it("maps form data into the mutation payload on submit", async () => {
    const mutateAsync = vi.fn().mockResolvedValue({ id: "diary-1" });
    const onSuccess = vi.fn();

    const { result } = renderHook(() =>
      useFoodDiaryDialog({
        defaultValues,
        onSuccess,
        mutation: { mutateAsync } as never,
      }),
    );

    const event = {
      preventDefault: vi.fn(),
    } as unknown as React.FormEvent<HTMLFormElement>;

    await act(async () => {
      result.current.handleCustomSubmit(event);
    });

    await waitFor(() => {
      expect(mutateAsync).toHaveBeenCalledWith({
        id: "diary-1",
        foodId: "food-1",
        mealType: MealTypes.Breakfast,
        servingSizeId: "ss-1",
        quantity: 2,
        entryDate: "2026-06-05",
      });
    });
    expect(onSuccess).toHaveBeenCalled();
  });

  it("maps create-form data without an id into the mutation payload", async () => {
    const mutateAsync = vi.fn().mockResolvedValue({ id: "new-diary" });
    const onSuccess = vi.fn();

    const { result } = renderHook(() =>
      useFoodDiaryDialog({
        defaultValues: {
          foodId: "food-1",
          mealType: MealTypes.Lunch,
          servingSizeId: "ss-1",
          quantity: 1,
          entryDate: "2026-06-05",
        },
        onSuccess,
        mutation: { mutateAsync } as never,
      }),
    );

    const event = {
      preventDefault: vi.fn(),
    } as unknown as React.FormEvent<HTMLFormElement>;

    await act(async () => {
      result.current.handleCustomSubmit(event);
    });

    await waitFor(() => {
      expect(mutateAsync).toHaveBeenCalledWith({
        id: undefined,
        foodId: "food-1",
        mealType: MealTypes.Lunch,
        servingSizeId: "ss-1",
        quantity: 1,
        entryDate: "2026-06-05",
      });
    });
    expect(onSuccess).toHaveBeenCalled();
  });
});

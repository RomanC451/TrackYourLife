import { act, renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { MealTypes } from "@/services/openapi";

import useRecipeDiaryDialog from "../useRecipeDiaryDialog";

describe("useRecipeDiaryDialog", () => {
  const defaultValues = {
    id: "diary-1",
    recipeId: "recipe-1",
    mealType: MealTypes.Lunch,
    servingSizeId: "ss-1",
    quantity: 1.5,
    entryDate: "2026-06-05",
  };

  it("submits recipe diary form values through the mutation", async () => {
    const mutateAsync = vi.fn().mockResolvedValue({ id: "diary-1" });
    const onSuccess = vi.fn();

    const { result } = renderHook(() =>
      useRecipeDiaryDialog({
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
      expect(mutateAsync).toHaveBeenCalledWith(defaultValues);
    });
    expect(onSuccess).toHaveBeenCalled();
  });

  it("maps create-form data without an id into the mutation payload", async () => {
    const mutateAsync = vi.fn().mockResolvedValue({ id: "new-diary" });
    const onSuccess = vi.fn();

    const { result } = renderHook(() =>
      useRecipeDiaryDialog({
        defaultValues: {
          recipeId: "recipe-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 2,
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
        recipeId: "recipe-1",
        mealType: MealTypes.Breakfast,
        servingSizeId: "ss-1",
        quantity: 2,
        entryDate: "2026-06-05",
      });
    });
    expect(onSuccess).toHaveBeenCalled();
  });
});

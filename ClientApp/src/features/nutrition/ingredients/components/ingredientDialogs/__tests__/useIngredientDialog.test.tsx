import { act, renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import useIngredientDialog from "../useIngredientDialog";

describe("useIngredientDialog", () => {
  const defaultValues = {
    foodId: "food-1",
    servingSizeId: "ss-1",
    quantity: 1.5,
  };

  it("calls mutate with validated form data", async () => {
    const mutate = vi.fn((_variables, options) => {
      options?.onSuccess?.();
    });
    const onSuccess = vi.fn();

    const { result } = renderHook(() =>
      useIngredientDialog({
        defaultValues,
        onSuccess,
        mutation: { mutate, trigger: vi.fn() } as never,
      }),
    );

    const event = {
      preventDefault: vi.fn(),
    } as unknown as React.FormEvent<HTMLFormElement>;

    await act(async () => {
      result.current.handleCustomSubmit(event);
    });

    await waitFor(() => {
      expect(mutate).toHaveBeenCalledWith(
        defaultValues,
        expect.objectContaining({ onSuccess: expect.any(Function) }),
      );
    });
    expect(onSuccess).toHaveBeenCalled();
  });

  it("does not call onSuccess when form validation fails", async () => {
    const mutate = vi.fn();
    const onSuccess = vi.fn();

    const { result } = renderHook(() =>
      useIngredientDialog({
        defaultValues,
        onSuccess,
        mutation: { mutate, trigger: vi.fn() } as never,
      }),
    );

    const triggerSpy = vi
      .spyOn(result.current.form, "trigger")
      .mockResolvedValue(false);

    const event = {
      preventDefault: vi.fn(),
    } as unknown as React.FormEvent<HTMLFormElement>;

    await act(async () => {
      result.current.handleCustomSubmit(event);
    });

    expect(triggerSpy).toHaveBeenCalled();
    expect(mutate).not.toHaveBeenCalled();
    expect(onSuccess).not.toHaveBeenCalled();
  });
});

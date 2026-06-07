import { act, renderHook } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import useMutationStore from "../useMutationStore";

describe("useMutationStore", () => {
  it("reports concurrent mutations when more than one is active", () => {
    const first = renderHook(() => useMutationStore("recipe-delete"));
    const second = renderHook(() => useMutationStore("recipe-delete"));
    const third = renderHook(() => useMutationStore("recipe-delete"));

    act(() => {
      first.result.current.updateMutationState(true);
      second.result.current.updateMutationState(true);
    });

    first.rerender();
    third.rerender();

    expect(first.result.current.mutationState).toBe(true);
    expect(third.result.current.mutationState).toBe(true);

    act(() => {
      first.result.current.updateMutationState(false);
      second.result.current.updateMutationState(false);
    });

    first.rerender();

    expect(first.result.current.mutationState).toBe(false);
  });

  it("isolates mutation state by mutation type", () => {
    const recipeDelete = renderHook(() => useMutationStore("recipe-delete"));
    const ingredientAdd = renderHook(() => useMutationStore("ingredient-add"));
    const recipeDeletePeer = renderHook(() =>
      useMutationStore("recipe-delete"),
    );

    act(() => {
      recipeDelete.result.current.updateMutationState(true);
      recipeDeletePeer.result.current.updateMutationState(true);
      ingredientAdd.result.current.updateMutationState(true);
    });

    recipeDelete.rerender();
    ingredientAdd.rerender();

    expect(recipeDelete.result.current.mutationState).toBe(true);
    expect(ingredientAdd.result.current.mutationState).toBe(false);
  });
});

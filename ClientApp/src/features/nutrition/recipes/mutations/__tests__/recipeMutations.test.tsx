import { act, renderHook, waitFor } from "@testing-library/react";
import { HttpStatusCode } from "axios";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { recipe } from "@/features/nutrition/__tests__/fixtures";
import { queryClient } from "@/queryClient";
import { QueryClientProvider } from "@tanstack/react-query";
import type { ReactNode } from "react";

function singletonQueryClientWrapper({ children }: { children: ReactNode }) {
  return (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
}

import { recipesQueryKeys } from "../../queries/useRecipeQuery";

const {
  mockCreateRecipe,
  mockUpdateRecipe,
  mockDeleteRecipe,
  mockDeleteRecipes,
  mockUndoDeleteRecipe,
  capturedMutationOptions,
} = vi.hoisted(() => ({
  mockCreateRecipe: vi.fn(),
  mockUpdateRecipe: vi.fn(),
  mockDeleteRecipe: vi.fn(),
  mockDeleteRecipes: vi.fn(),
  mockUndoDeleteRecipe: vi.fn(),
  capturedMutationOptions: [] as Array<{
    meta?: {
      onSuccessToast?: {
        data?: { action?: { onClick: () => void } };
      };
    };
  }>,
}));

vi.mock("uuid", () => ({
  v4: () => "optimistic-recipe-id",
}));

vi.mock("@/hooks/useCustomMutation", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/hooks/useCustomMutation")>();
  return {
    ...actual,
    useCustomMutation: <
      TData,
      TVariables,
      TContext,
      TError = Error,
    >(
      options: Parameters<typeof actual.useCustomMutation<TData, TVariables, TContext, TError>>[0],
    ) => {
      capturedMutationOptions.push(options as never);
      return actual.useCustomMutation(options);
    },
  };
});

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockRecipesApi {
    createRecipe = mockCreateRecipe;
    updateRecipe = mockUpdateRecipe;
    deleteRecipe = mockDeleteRecipe;
    deleteRecipes = mockDeleteRecipes;
    undoDeleteRecipe = mockUndoDeleteRecipe;
  }
  return { ...actual, RecipesApi: MockRecipesApi };
});

import useCreateRecipeMutation from "../useCreateRecipeMutation";
import useDeleteRecipeMutation from "../useDeleteRecipeMutation";
import useDeleteRecipesMutation from "../useDeleteRecipesMutation";
import useUndoDeleteRecipeMutation from "../useUndoDeleteRecipeMutation";
import useUpdateRecipeMutation from "../useUpdateRecipeMutation";

describe("recipe mutations", () => {
  beforeEach(() => {
    queryClient.clear();
    capturedMutationOptions.length = 0;
    vi.clearAllMocks();
    mockCreateRecipe.mockResolvedValue({ data: { id: "recipe-new" } });
    mockUpdateRecipe.mockResolvedValue({ data: { id: "recipe-1" } });
    mockDeleteRecipe.mockResolvedValue(undefined);
    mockDeleteRecipes.mockResolvedValue(undefined);
    mockUndoDeleteRecipe.mockResolvedValue(undefined);
  });

  describe("useCreateRecipeMutation", () => {
    it("optimistically appends a loading recipe", async () => {
      queryClient.setQueryData(recipesQueryKeys.all, [
        recipe("r-1", "Soup"),
      ]);
      let resolveCreate: (value: { data: { id: string } }) => void = () => {};
      mockCreateRecipe.mockImplementation(
        () =>
          new Promise((resolve) => {
            resolveCreate = resolve;
          }),
      );

      const { result } = renderHook(
        () =>
          useCreateRecipeMutation({
            setError: vi.fn(),
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      let mutationPromise!: Promise<unknown>;
      await act(async () => {
        mutationPromise = result.current.mutateAsync({
          name: "Salad",
          portions: 2,
          weight: 200,
        });
      });

      const cached = queryClient.getQueryData<ReturnType<typeof recipe>[]>(
        recipesQueryKeys.all,
      )!;
      expect(cached).toHaveLength(2);
      expect(cached[1]).toMatchObject({
        id: "optimistic-recipe-id",
        name: "Salad",
        portions: 2,
        isLoading: true,
      });

      await act(async () => {
        resolveCreate({ data: { id: "recipe-new" } });
        await mutationPromise;
      });
    });

    it("rolls back optimistic data on error", async () => {
      const previous = [recipe("r-1", "Soup")];
      queryClient.setQueryData(recipesQueryKeys.all, previous);
      mockCreateRecipe.mockRejectedValue(new Error("failed"));

      const { result } = renderHook(
        () =>
          useCreateRecipeMutation({
            setError: vi.fn(),
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current.mutateAsync({
          name: "Salad",
          portions: 2,
          weight: 200,
        }).catch(() => undefined);
      });

      expect(queryClient.getQueryData(recipesQueryKeys.all)).toEqual(previous);
    });
  });

  describe("useUpdateRecipeMutation", () => {
    it("updates recipe detail and list caches on success", async () => {
      queryClient.setQueryData(recipesQueryKeys.byId("recipe-1"), recipe("recipe-1", "Old name"));
      queryClient.setQueryData(recipesQueryKeys.all, [recipe("recipe-1", "Old name")]);
      const onSuccess = vi.fn();

      const { result } = renderHook(
        () =>
          useUpdateRecipeMutation({
            recipeId: "recipe-1",
            onSuccess,
            setError: vi.fn(),
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current.mutateAsync({
          name: "New name",
          portions: 4,
          weight: 500,
        });
      });

      expect(
        queryClient.getQueryData(recipesQueryKeys.byId("recipe-1")),
      ).toMatchObject({
        name: "New name",
        portions: 4,
        isLoading: true,
      });
      expect(queryClient.getQueryData<ReturnType<typeof recipe>[]>(recipesQueryKeys.all)?.[0]).toMatchObject({
        name: "New name",
        portions: 4,
      });
      expect(onSuccess).toHaveBeenCalled();
    });

    it("maps validation errors onto the form through setError", async () => {
      const setError = vi.fn();
      mockUpdateRecipe.mockRejectedValue({
        response: {
          status: HttpStatusCode.BadRequest,
          data: {
            type: "ValidationError",
            errors: [{ name: "Name", message: "Name is required" }],
          },
        },
      });

      const { result } = renderHook(
        () =>
          useUpdateRecipeMutation({
            recipeId: "recipe-1",
            setError,
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current
          .mutateAsync({
            name: "",
            portions: 2,
            weight: 400,
          })
          .catch(() => undefined);
      });

      expect(setError).toHaveBeenCalledWith(
        "name",
        expect.objectContaining({ message: "Name is required" }),
        { shouldFocus: false },
      );
    });
  });

  describe("useDeleteRecipeMutation", () => {
    it("marks a recipe as deleting optimistically", async () => {
      const target = recipe("recipe-1", "Soup");
      queryClient.setQueryData(recipesQueryKeys.all, [
        target,
        recipe("recipe-2", "Salad"),
      ]);
      let resolveDelete: (value?: unknown) => void = () => {};
      mockDeleteRecipe.mockImplementation(
        () =>
          new Promise((resolve) => {
            resolveDelete = resolve;
          }),
      );

      const { result } = renderHook(
        () => useDeleteRecipeMutation({ recipeId: "recipe-1" }),
        { wrapper: singletonQueryClientWrapper },
      );

      let mutationPromise!: Promise<unknown>;
      await act(async () => {
        mutationPromise = result.current.mutateAsync({ recipe: target });
      });

      expect(queryClient.getQueryData<ReturnType<typeof recipe>[]>(recipesQueryKeys.all)?.[0]).toMatchObject({
        id: "recipe-1",
        isDeleting: true,
      });

      await act(async () => {
        resolveDelete();
        await mutationPromise;
      });
    });

    it("rolls back optimistic deleting state on error", async () => {
      const target = recipe("recipe-1", "Soup");
      const previous = [target, recipe("recipe-2", "Salad")];
      queryClient.setQueryData(recipesQueryKeys.all, previous);
      mockDeleteRecipe.mockRejectedValue(new Error("failed"));

      const { result } = renderHook(
        () => useDeleteRecipeMutation({ recipeId: "recipe-1" }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current
          .mutateAsync({ recipe: target })
          .catch(() => undefined);
      });

      expect(queryClient.getQueryData(recipesQueryKeys.all)).toEqual(previous);
    });

    it("registers an undo action on the success toast metadata", async () => {
      const target = recipe("recipe-1", "Soup");
      queryClient.setQueryData(recipesQueryKeys.all, [target]);

      renderHook(() => useDeleteRecipeMutation({ recipeId: "recipe-1" }), {
        wrapper: singletonQueryClientWrapper,
      });

      const deleteOptions = capturedMutationOptions.find(
        (options) =>
          (options as { meta?: { onSuccessToast?: { message?: string } } }).meta
            ?.onSuccessToast?.message === "Recipe deleted",
      ) as {
        meta?: {
          onSuccessToast?: {
            message?: string;
            data?: { action?: { label?: string; onClick?: () => void } };
          };
        };
      };

      const undoAction = deleteOptions?.meta?.onSuccessToast?.data?.action;

      expect(undoAction?.label).toBe("Undo");

      act(() => {
        undoAction?.onClick?.();
      });

      await waitFor(() =>
        expect(mockUndoDeleteRecipe).toHaveBeenCalledWith({ id: "recipe-1" }),
      );
    });
  });

  describe("useDeleteRecipesMutation", () => {
    it("marks multiple recipes as deleting optimistically", async () => {
      queryClient.setQueryData(recipesQueryKeys.all, [
        recipe("recipe-1", "Soup"),
        recipe("recipe-2", "Salad"),
        recipe("recipe-3", "Stew"),
      ]);
      let resolveDelete: (value?: unknown) => void = () => {};
      mockDeleteRecipes.mockImplementation(
        () =>
          new Promise((resolve) => {
            resolveDelete = resolve;
          }),
      );

      const { result } = renderHook(() => useDeleteRecipesMutation(), {
        wrapper: singletonQueryClientWrapper,
      });

      let mutationPromise!: Promise<unknown>;
      await act(async () => {
        mutationPromise = result.current.mutateAsync({
          ids: ["recipe-1", "recipe-3"],
        });
      });

      const cached = queryClient.getQueryData<ReturnType<typeof recipe>[]>(
        recipesQueryKeys.all,
      )!;
      expect(cached[0].isDeleting).toBe(true);
      expect(cached[1].isDeleting).toBe(false);
      expect(cached[2].isDeleting).toBe(true);

      await act(async () => {
        resolveDelete();
        await mutationPromise;
      });
    });

    it("rolls back optimistic deleting state on error", async () => {
      const previous = [
        recipe("recipe-1", "Soup"),
        recipe("recipe-2", "Salad"),
        recipe("recipe-3", "Stew"),
      ];
      queryClient.setQueryData(recipesQueryKeys.all, previous);
      mockDeleteRecipes.mockRejectedValue(new Error("failed"));

      const { result } = renderHook(() => useDeleteRecipesMutation(), {
        wrapper: singletonQueryClientWrapper,
      });

      await act(async () => {
        await result.current
          .mutateAsync({
            ids: ["recipe-1", "recipe-3"],
          })
          .catch(() => undefined);
      });

      expect(queryClient.getQueryData(recipesQueryKeys.all)).toEqual(previous);
    });
  });

  describe("useUndoDeleteRecipeMutation", () => {
    it("calls the undo delete API", async () => {
      mockUndoDeleteRecipe.mockResolvedValue(undefined);

      const { result } = renderHook(() => useUndoDeleteRecipeMutation(), {
        wrapper: singletonQueryClientWrapper,
      });

      await act(async () => {
        await result.current.mutateAsync({ id: "recipe-1" });
      });

      expect(mockUndoDeleteRecipe).toHaveBeenCalledWith({ id: "recipe-1" });
    });
  });
});

import { act, renderHook } from "@testing-library/react";
import { HttpStatusCode } from "axios";
import { beforeEach, describe, expect, it, vi } from "vitest";

import {
  food,
  ingredient,
  recipe,
} from "@/features/nutrition/__tests__/fixtures";
import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { queryClient } from "@/queryClient";
import { QueryClientProvider } from "@tanstack/react-query";
import type { ReactNode } from "react";

function singletonQueryClientWrapper({ children }: { children: ReactNode }) {
  return (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
}

import { recipesQueryKeys } from "../../../recipes/queries/useRecipeQuery";

const {
  mockAddIngredient,
  mockUpdateIngredient,
  mockRemoveIngredients,
  mockToastError,
} = vi.hoisted(() => ({
  mockAddIngredient: vi.fn(),
  mockUpdateIngredient: vi.fn(),
  mockRemoveIngredients: vi.fn(),
  mockToastError: vi.fn(),
}));

function differentServingSizeError() {
  return {
    response: {
      status: HttpStatusCode.BadRequest,
      data: { type: "Ingredient.DifferentServingSize" },
    },
  };
}

vi.mock("uuid", () => ({
  v4: () => "optimistic-ingredient-id",
}));

vi.mock("sonner", () => ({
  toast: Object.assign(vi.fn(), {
    error: mockToastError,
    success: vi.fn(),
  }),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockRecipesApi {
    addIngredient = mockAddIngredient;
    updateIngredient = mockUpdateIngredient;
    removeIngredients = mockRemoveIngredients;
  }
  return { ...actual, RecipesApi: MockRecipesApi };
});

import useAddIngredientMutation from "../useAddIngredientMutation";
import useRemoveIngredientsMutation from "../useRemoveIngredientsMutation";
import useUpdateIngredientMutation from "../useUpdateIngredientMutation";

describe("ingredient mutations", () => {
  const oats = food("food-1", "Oats");
  oats.nutritionalContents = createEmptyNutritionalContent();
  oats.nutritionalContents.protein = 10;
  oats.nutritionalContents.energy = { unit: "calories", value: 100 };

  const baseRecipe = recipe("recipe-1", "Breakfast bowl", {
    ingredients: [ingredient("ing-1", oats, 1)],
    nutritionalContents: (() => {
      const nutrition = createEmptyNutritionalContent();
      nutrition.protein = 10;
      nutrition.energy = { unit: "calories", value: 100 };
      return nutrition;
    })(),
  });

  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
    mockAddIngredient.mockResolvedValue({ data: { id: "ing-new" } });
    mockUpdateIngredient.mockResolvedValue({ data: { id: "ing-1" } });
    mockRemoveIngredients.mockResolvedValue({ data: undefined });
    queryClient.setQueryData(recipesQueryKeys.byId("recipe-1"), baseRecipe);
  });

  describe("useAddIngredientMutation", () => {
    it("optimistically adds an ingredient to the recipe cache", async () => {
      const eggs = food("food-2", "Eggs");
      eggs.nutritionalContents = createEmptyNutritionalContent();
      eggs.nutritionalContents.protein = 6;
      let resolveAdd: (value: { data: { id: string } }) => void = () => {};
      mockAddIngredient.mockImplementation(
        () =>
          new Promise((resolve) => {
            resolveAdd = resolve;
          }),
      );

      const { result } = renderHook(
        () =>
          useAddIngredientMutation({
            recipe: baseRecipe,
            food: eggs,
            servingSizes: oats.servingSizes,
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      let mutationPromise!: Promise<unknown>;
      await act(async () => {
        mutationPromise = result.current.mutateAsync({
          foodId: "food-2",
          servingSizeId: "ss-1",
          quantity: 2,
        });
      });

      const cached = queryClient.getQueryData<import("@/services/openapi").RecipeDto>(
        recipesQueryKeys.byId("recipe-1"),
      )!;
      expect(cached.ingredients).toHaveLength(2);
      expect(cached.ingredients[1]).toMatchObject({
        id: "optimistic-ingredient-id",
        quantity: 2,
        isLoading: true,
      });
      expect(cached.nutritionalContents.protein).toBe(22);

      await act(async () => {
        resolveAdd({ data: { id: "ing-new" } });
        await mutationPromise;
      });
    });

    it("rolls back optimistic data on error", async () => {
      const eggs = food("food-2", "Eggs");
      mockAddIngredient.mockRejectedValue(new Error("failed"));

      const { result } = renderHook(
        () =>
          useAddIngredientMutation({
            recipe: baseRecipe,
            food: eggs,
            servingSizes: oats.servingSizes,
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current
          .mutateAsync({
            foodId: "food-2",
            servingSizeId: "ss-1",
            quantity: 1,
          })
          .catch(() => undefined);
      });

      const cached = queryClient.getQueryData<import("@/services/openapi").RecipeDto>(
        recipesQueryKeys.byId("recipe-1"),
      )!;
      expect(cached.ingredients).toHaveLength(1);
    });

    it("sets a serving size error when the API rejects a duplicate", async () => {
      const eggs = food("food-2", "Eggs");
      const setError = vi.fn();
      mockAddIngredient.mockRejectedValue(differentServingSizeError());

      const recipeWithEggs = {
        ...baseRecipe,
        ingredients: [
          ...baseRecipe.ingredients,
          ingredient("ing-2", eggs, 1),
        ],
      };
      queryClient.setQueryData(
        recipesQueryKeys.byId("recipe-1"),
        recipeWithEggs,
      );

      const { result } = renderHook(
        () =>
          useAddIngredientMutation({
            recipe: recipeWithEggs,
            food: eggs,
            servingSizes: oats.servingSizes,
            setError,
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current
          .mutateAsync({
            foodId: "food-2",
            servingSizeId: "ss-1",
            quantity: 1,
          })
          .catch(() => undefined);
      });

      expect(setError).toHaveBeenCalledWith(
        "servingSizeId",
        expect.objectContaining({
          message: expect.stringContaining("different serving size"),
        }),
        { shouldFocus: true },
      );
    });

    it("skips optimistic updates when recipe cache is missing", async () => {
      queryClient.removeQueries({ queryKey: recipesQueryKeys.byId("recipe-1") });
      const eggs = food("food-2", "Eggs");

      const { result } = renderHook(
        () =>
          useAddIngredientMutation({
            recipe: baseRecipe,
            food: eggs,
            servingSizes: oats.servingSizes,
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current.mutateAsync({
          foodId: "food-2",
          servingSizeId: "ss-1",
          quantity: 1,
        });
      });

      expect(queryClient.getQueryData(recipesQueryKeys.byId("recipe-1"))).toBeUndefined();
    });

    it("does not roll back when optimistic context is missing", async () => {
      const eggs = food("food-2", "Eggs");
      mockAddIngredient.mockRejectedValue(new Error("failed"));
      queryClient.removeQueries({ queryKey: recipesQueryKeys.byId("recipe-1") });

      const { result } = renderHook(
        () =>
          useAddIngredientMutation({
            recipe: baseRecipe,
            food: eggs,
            servingSizes: oats.servingSizes,
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current
          .mutateAsync({
            foodId: "food-2",
            servingSizeId: "ss-1",
            quantity: 1,
          })
          .catch(() => undefined);
      });

      expect(queryClient.getQueryData(recipesQueryKeys.byId("recipe-1"))).toBeUndefined();
    });

    it("shows a toast when no setError handler is provided", async () => {
      const eggs = food("food-2", "Eggs");
      mockAddIngredient.mockRejectedValue(differentServingSizeError());

      const recipeWithEggs = {
        ...baseRecipe,
        ingredients: [
          ...baseRecipe.ingredients,
          ingredient("ing-2", eggs, 1),
        ],
      };
      queryClient.setQueryData(
        recipesQueryKeys.byId("recipe-1"),
        recipeWithEggs,
      );

      const { result } = renderHook(
        () =>
          useAddIngredientMutation({
            recipe: recipeWithEggs,
            food: eggs,
            servingSizes: oats.servingSizes,
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current
          .mutateAsync({
            foodId: "food-2",
            servingSizeId: "ss-1",
            quantity: 1,
          })
          .catch(() => undefined);
      });

      expect(mockToastError).toHaveBeenCalledWith(
        expect.stringContaining("different serving size"),
      );
    });
  });

  describe("useUpdateIngredientMutation", () => {
    it("optimistically updates ingredient quantity and nutrition", async () => {
      let resolveUpdate: (value: { data: { id: string } }) => void = () => {};
      mockUpdateIngredient.mockImplementation(
        () =>
          new Promise((resolve) => {
            resolveUpdate = resolve;
          }),
      );

      const { result } = renderHook(
        () =>
          useUpdateIngredientMutation({
            recipe: baseRecipe,
            ingredient: baseRecipe.ingredients[0],
            servingSizes: oats.servingSizes,
            setError: vi.fn(),
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      let mutationPromise!: Promise<unknown>;
      await act(async () => {
        mutationPromise = result.current.mutateAsync({
          foodId: "food-1",
          servingSizeId: "ss-1",
          quantity: 3,
        });
      });

      const cached = queryClient.getQueryData<import("@/services/openapi").RecipeDto>(
        recipesQueryKeys.byId("recipe-1"),
      )!;
      expect(cached.ingredients[0].quantity).toBe(3);
      expect(cached.nutritionalContents.protein).toBe(30);

      await act(async () => {
        resolveUpdate({ data: { id: "ing-1" } });
        await mutationPromise;
      });
    });

    it("rolls back optimistic data on error", async () => {
      mockUpdateIngredient.mockRejectedValue(new Error("failed"));
      const setError = vi.fn();

      const { result } = renderHook(
        () =>
          useUpdateIngredientMutation({
            recipe: baseRecipe,
            ingredient: baseRecipe.ingredients[0],
            servingSizes: oats.servingSizes,
            setError,
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current
          .mutateAsync({
            foodId: "food-1",
            servingSizeId: "ss-1",
            quantity: 5,
          })
          .catch(() => undefined);
      });

      const cached = queryClient.getQueryData<import("@/services/openapi").RecipeDto>(
        recipesQueryKeys.byId("recipe-1"),
      )!;
      expect(cached.ingredients[0].quantity).toBe(1);
    });

    it("sets a serving size error on duplicate serving size rejection", async () => {
      mockUpdateIngredient.mockRejectedValue(differentServingSizeError());
      const setError = vi.fn();

      const { result } = renderHook(
        () =>
          useUpdateIngredientMutation({
            recipe: baseRecipe,
            ingredient: baseRecipe.ingredients[0],
            servingSizes: oats.servingSizes,
            setError,
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current
          .mutateAsync({
            foodId: "food-1",
            servingSizeId: "ss-1",
            quantity: 2,
          })
          .catch(() => undefined);
      });

      expect(setError).toHaveBeenCalledWith(
        "servingSizeId",
        expect.objectContaining({
          message: expect.stringContaining("different serving size"),
        }),
        { shouldFocus: true },
      );
    });
  });

  describe("useRemoveIngredientsMutation", () => {
    it("marks ingredients as deleting optimistically", async () => {
      let resolveRemove: (value: { data: undefined }) => void = () => {};
      mockRemoveIngredients.mockImplementation(
        () =>
          new Promise((resolve) => {
            resolveRemove = resolve;
          }),
      );

      const { result } = renderHook(() => useRemoveIngredientsMutation(), {
        wrapper: singletonQueryClientWrapper,
      });

      let mutationPromise!: Promise<unknown>;
      await act(async () => {
        mutationPromise = result.current.mutateAsync({
          recipe: baseRecipe,
          ingredients: [baseRecipe.ingredients[0]],
        });
      });

      const cached = queryClient.getQueryData<import("@/services/openapi").RecipeDto>(
        recipesQueryKeys.byId("recipe-1"),
      )!;
      expect(cached.ingredients[0].isDeleting).toBe(true);

      await act(async () => {
        resolveRemove({ data: undefined });
        await mutationPromise;
      });
    });

    it("rolls back optimistic delete state on error", async () => {
      mockRemoveIngredients.mockRejectedValue(new Error("failed"));

      const { result } = renderHook(() => useRemoveIngredientsMutation(), {
        wrapper: singletonQueryClientWrapper,
      });

      await act(async () => {
        await result.current
          .mutateAsync({
            recipe: baseRecipe,
            ingredients: [baseRecipe.ingredients[0]],
          })
          .catch(() => undefined);
      });

      const cached = queryClient.getQueryData<import("@/services/openapi").RecipeDto>(
        recipesQueryKeys.byId("recipe-1"),
      )!;
      expect(cached.ingredients[0].isDeleting).toBe(false);
    });

    it("skips optimistic updates when recipe cache is missing", async () => {
      queryClient.removeQueries({ queryKey: recipesQueryKeys.byId("recipe-1") });

      const { result } = renderHook(() => useRemoveIngredientsMutation(), {
        wrapper: singletonQueryClientWrapper,
      });

      await act(async () => {
        await result.current.mutateAsync({
          recipe: baseRecipe,
          ingredients: [baseRecipe.ingredients[0]],
        });
      });

      expect(queryClient.getQueryData(recipesQueryKeys.byId("recipe-1"))).toBeUndefined();
    });

    it("does not roll back when optimistic context is missing", async () => {
      mockRemoveIngredients.mockRejectedValue(new Error("failed"));
      queryClient.removeQueries({ queryKey: recipesQueryKeys.byId("recipe-1") });

      const { result } = renderHook(() => useRemoveIngredientsMutation(), {
        wrapper: singletonQueryClientWrapper,
      });

      await act(async () => {
        await result.current
          .mutateAsync({
            recipe: baseRecipe,
            ingredients: [baseRecipe.ingredients[0]],
          })
          .catch(() => undefined);
      });

      expect(queryClient.getQueryData(recipesQueryKeys.byId("recipe-1"))).toBeUndefined();
    });
  });

  describe("useUpdateIngredientMutation guards", () => {
    it("skips optimistic updates when recipe cache is missing", async () => {
      queryClient.removeQueries({ queryKey: recipesQueryKeys.byId("recipe-1") });

      const { result } = renderHook(
        () =>
          useUpdateIngredientMutation({
            recipe: baseRecipe,
            ingredient: baseRecipe.ingredients[0],
            servingSizes: oats.servingSizes,
            setError: vi.fn(),
          }),
        { wrapper: singletonQueryClientWrapper },
      );

      await act(async () => {
        await result.current.mutateAsync({
          foodId: "food-1",
          servingSizeId: "ss-1",
          quantity: 3,
        });
      });

      expect(queryClient.getQueryData(recipesQueryKeys.byId("recipe-1"))).toBeUndefined();
    });
  });
});

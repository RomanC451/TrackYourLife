import { HttpStatusCode } from "axios";
import { ErrorOption } from "react-hook-form";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import {
  IngredientDto,
  RecipeDto,
  RecipesApi,
  ServingSizeDto,
} from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";

import { updateIngredientInRecipe } from "../../common/utils/recipes";
import { recipesQueryKeys } from "../../recipes/queries/useRecipeQuery";
import { ingredientsApiErrors } from "../data/ingredientsApiErrors";
import { IngredientSchema } from "../data/ingredientsSchemas";

const recipesApi = new RecipesApi();

type Variables = IngredientSchema;

export default function useUpdateIngredientMutation({
  recipe,
  ingredient,
  servingSizes,
  setError,
}: {
  recipe: RecipeDto;
  ingredient: IngredientDto;
  servingSizes: ServingSizeDto[];
  setError: (
    name: keyof IngredientSchema,
    error: ErrorOption,
    options?: {
      shouldFocus: boolean;
    },
  ) => void;
}) {
  const updateIngredientMutation = useCustomMutation({
    mutationFn: (variables: Variables) =>
      recipesApi
        .updateIngredient(recipe.id, ingredient.id, variables)
        .then((resp) => resp.data),

    meta: {
      onSuccessToast: {
        message: "Ingredient updated",
        type: "success",
      },
      invalidateQueries: [recipesQueryKeys.byId(recipe.id)],
    },

    onMutate: (variables) => {
      const previousData = queryClient.getQueryData<RecipeDto>(
        recipesQueryKeys.byId(recipe.id),
      );
      if (!previousData) {
        return;
      }

      queryClient.setQueryData(
        recipesQueryKeys.byId(recipe.id),
        (oldData: RecipeDto) => {
          return updateIngredientInRecipe(oldData, {
            ...ingredient,
            quantity: variables.quantity,
            servingSize: servingSizes.find(
              (servingSize) => servingSize.id === variables.servingSizeId,
            )!,
            isLoading: true,
            isDeleting: false,
          });
        },
      );

      return { previousData };
    },

    onError: (error, _variables, context) => {
      if (!context) {
        return;
      }

      queryClient.setQueryData(
        recipesQueryKeys.byId(context.previousData.id),
        context.previousData,
      );

      handleApiError({
        error,
        errorHandlers: {
          [HttpStatusCode.BadRequest]: {
            [ingredientsApiErrors.Ingredient.DifferentServingSize]: () => {
              const servingSize = recipe.ingredients.find(
                (ingredient) => ingredient.food.id === ingredient.food.id,
              )?.servingSize;

              if (!servingSize) return;

              setError(
                "servingSizeId",
                {
                  message: `This ingredient already exists with a different serving size: ${servingSize?.value} ${servingSize?.unit}`,
                },
                {
                  shouldFocus: true,
                },
              );
            },
          },
        },
      });
    },
  });

  return updateIngredientMutation;
}

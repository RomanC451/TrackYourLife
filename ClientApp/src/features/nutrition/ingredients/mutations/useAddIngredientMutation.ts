import { HttpStatusCode } from "axios";
import { ErrorOption } from "react-hook-form";
import { toast } from "sonner";
import { v4 as uuidv4 } from "uuid";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import {
  FoodDto,
  RecipeDto,
  RecipesApi,
  ServingSizeDto,
} from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";

import { addIngredientInRecipe } from "../../common/utils/recipes";
import { recipesQueryKeys } from "../../recipes/queries/useRecipeQuery";
import { ingredientsApiErrors } from "../data/ingredientsApiErrors";
import { IngredientSchema } from "../data/ingredientsSchemas";

const recipesApi = new RecipesApi();

export type Variables = IngredientSchema;

function useAddIngredientMutation({
  recipe,
  food,
  servingSizes,
  setError,
}: {
  recipe: RecipeDto;
  food: FoodDto;
  servingSizes: ServingSizeDto[];
  setError?: (
    name: keyof IngredientSchema,
    error: ErrorOption,
    options?: {
      shouldFocus: boolean;
    },
  ) => void;
}) {
  const addIngredientMutation = useCustomMutation({
    mutationFn: (variables: Variables) =>
      recipesApi.addIngredient(recipe.id, variables).then((resp) => resp.data),

    meta: {
      noDefaultErrorToast: true,
      onSuccessToast: {
        message: "Ingredient added",
        type: "success",
      },
      invalidateQueries: [recipesQueryKeys.all],
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
        (oldData: RecipeDto) =>
          addIngredientInRecipe(oldData, {
            food,
            servingSize: servingSizes.find(
              (servingSize) => servingSize.id === variables.servingSizeId,
            )!,
            id: uuidv4(),
            quantity: variables.quantity,
            isLoading: true,
            isDeleting: false,
          }),
      );

      return { previousData };
    },

    onError: (error, variables, context) => {
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
                (ingredient) => ingredient.food.id === variables.foodId,
              )?.servingSize;

              if (!servingSize) return;

              if (setError) {
                setError(
                  "servingSizeId",
                  {
                    message: `This ingredient already exists with a different serving size: ${servingSize?.value} ${servingSize?.unit}`,
                  },
                  {
                    shouldFocus: true,
                  },
                );
              } else {
                toast.error(
                  `This ingredient already exists with a different serving size: ${servingSize?.value} ${servingSize?.unit}`,
                );
              }
            },
          },
        },
      });
    },
  });

  return addIngredientMutation;
}

export default useAddIngredientMutation;

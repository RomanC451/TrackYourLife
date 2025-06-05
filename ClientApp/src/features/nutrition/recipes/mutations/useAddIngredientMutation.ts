import { useMutation } from "@tanstack/react-query";
import { v4 as uuidv4 } from "uuid";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import {
  AddIngredientRequest,
  FoodDto,
  RecipeDto,
  RecipesApi,
  ServingSizeDto,
} from "@/services/openapi";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

import { setRecipesQueryData } from "../../common/queries/useRecipesQuery";
import { addIngredientInRecipe } from "../../common/utils/recipes";
import {
  getRecipeQueryData,
  invalidateRecipeQuery,
  setRecipeQueryData,
} from "../queries/useRecipeQuery";
import ingredientAddedToast from "../toasts/ingredientAddedToast";

const recipesApi = new RecipesApi();

type AddIngredientMutationVariables = {
  recipe: RecipeDto;
  food: FoodDto;
  servingSize: ServingSizeDto;
} & AddIngredientRequest;

function useAddIngredientMutation() {
  const addIngredientMutation = useMutation({
    mutationFn: ({ recipe, ...request }: AddIngredientMutationVariables) => {
      const promise = recipesApi
        .addIngredient(recipe.id, request)
        .then((resp) => resp.data);

      ingredientAddedToast(promise, request.food.name);

      return promise;
    },
    onMutate: ({ recipe, food, servingSize, quantity }) => {
      const previousRecipe = getRecipeQueryData(recipe.id);

      if (!previousRecipe) {
        return { previousRecipe };
      }

      setRecipeQueryData({
        recipeId: recipe.id,
        addedIngredient: { food, servingSize, id: uuidv4(), quantity, isLoading: true, isDeleting: false },
      });
      return { previousRecipe };
    },
    onSuccess: ({ id }, { recipe, food, servingSize, quantity }) => {
      setRecipesQueryData({
        updatedRecipe: addIngredientInRecipe(recipe, {
          food,
          servingSize,
          id,
          quantity,
          isLoading: true,
          isDeleting: false,
        }),
      });
    },
    onError: (error, _, context) => {
      toastDefaultServerError(error);
      if (!context) {
        return;
      }

      const { previousRecipe } = context;
      if (!previousRecipe) {
        return;
      }

      setRecipeQueryData({
        recipeId: previousRecipe.id,
        data: previousRecipe,
      });
    },
    onSettled: (_, _2, { recipe }) => {
      invalidateRecipeQuery(recipe.id);
    },
  });

  const isPending = useDelayedLoading(addIngredientMutation.isPending);

  return {
    addIngredientMutation,
    isPending,
  };
}

export default useAddIngredientMutation;

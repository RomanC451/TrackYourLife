import { useMutation } from "@tanstack/react-query";
import { toastDefaultServerError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import useStoreLoadingStateToContext from "~/hooks/useStoreLoadingStateToContext";
import {
  AddIngredientRequest,
  FoodDto,
  RecipeDto,
  RecipesApi,
  ServingSizeDto,
} from "~/services/openapi";
import {
  getRecipeQueryData,
  invalidateRecipeQuery,
  setRecipeQueryData,
} from "../../queries/recipes/useRecipeQuery";
import { setRecipesQueryData } from "../../queries/recipes/useRecipesQuery";
import ingredientAddedToast from "../../toasts/recipes/ingredientAddedToast";
import { addIngredientInRecipe } from "../../utils/recipes";

const recipesApi = new RecipesApi();

type AddIngredientMutationVariables = {
  recipe: RecipeDto;
  food: FoodDto;
  servingSize: ServingSizeDto;
} & AddIngredientRequest;

function useAddIngredientMutation() {
  const addIngredientMutation = useMutation({
    mutationFn: ({ recipe, ...request }: AddIngredientMutationVariables) =>
      recipesApi.addIngredient(recipe.id, request).then((resp) => resp.data),
    onMutate: ({ recipe, food, servingSize, quantity }) => {
      const previousRecipe = getRecipeQueryData(recipe.id);

      if (!previousRecipe) {
        return { previousRecipe };
      }

      setRecipeQueryData({
        recipeId: recipe.id,
        addedIngredient: { food, servingSize, id: "id", quantity },
      });
      return { previousRecipe };
    },
    onSuccess: ({ id }, { recipe, food, servingSize, quantity }) => {
      ingredientAddedToast(food.name);
      setRecipesQueryData({
        updatedRecipe: addIngredientInRecipe(recipe, {
          food,
          servingSize,
          id,
          quantity,
        }),
        invalidate: true,
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

      setRecipeQueryData({ recipeId: previousRecipe.id, data: previousRecipe });
    },
    onSettled: (_, _2, { recipe }) => {
      invalidateRecipeQuery(recipe.id);
    },
  });

  const isPending = useDelayedLoading(addIngredientMutation.isPending);

  console.log(addIngredientMutation.isPending);

  useStoreLoadingStateToContext("addIngredientMutation", false);

  return {
    addIngredientMutation,
    isPending,
  };
}

export default useAddIngredientMutation;

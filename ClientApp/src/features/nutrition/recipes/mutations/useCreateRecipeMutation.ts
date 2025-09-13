import { ErrorOption } from "react-hook-form";
import { v4 as uuidv4 } from "uuid";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { CreateRecipeRequest, RecipeDto, RecipesApi } from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";

import { createEmptyNutritionalContent } from "../../common/utils/nutritionalContent";
import { RecipeDetailsSchema } from "../data/recipesSchemas";
import { recipesQueryKeys } from "../queries/useRecipeQuery";

const recipesApi = new RecipesApi();

export type Variables = CreateRecipeRequest;

function useCreateRecipeMutation({
  onSuccess,
  setError,
}: {
  onSuccess?: (recipeId: string) => void;
  setError: (
    name: keyof RecipeDetailsSchema,
    error: ErrorOption,
    options?: {
      shouldFocus: boolean;
    },
  ) => void;
}) {
  const createRecipeMutation = useCustomMutation({
    mutationFn: (variables: Variables) =>
      recipesApi.createRecipe(variables).then((resp) => resp.data),

    meta: {
      onSuccessToast: {
        message: "Recipe created",
        type: "success",
      },
      invalidateQueries: [recipesQueryKeys.all],
    },
    onMutate: (variables) => {
      const previousRecipes = queryClient.getQueryData(recipesQueryKeys.all);

      if (!previousRecipes) return;

      queryClient.setQueryData(recipesQueryKeys.all, (oldData: RecipeDto[]) => [
        ...oldData,
        {
          id: uuidv4(),
          name: variables.name,
          ingredients: [],
          nutritionalContents: createEmptyNutritionalContent(),
          portions: variables.portions,
          isLoading: true,
        },
      ]);

      return { previousRecipes };
    },

    onSuccess: (data) => {
      onSuccess?.(data.id);
    },

    onError: (error, _variables, context) => {
      if (!context) return;

      queryClient.setQueryData(recipesQueryKeys.all, context.previousRecipes);

      handleApiError({
        error,
        validationErrorsHandler: setError,
      });
    },
  });

  return createRecipeMutation;
}

export default useCreateRecipeMutation;

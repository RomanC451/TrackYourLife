import { useNavigate } from "@tanstack/react-router";

import useCustomForm from "@/components/forms/useCustomForm";
import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { RecipeDto } from "@/services/openapi";

import { recipeDetailsSchema } from "../../data/recipesSchemas";
import useCreateRecipeMutation from "../../mutations/useCreateRecipeMutation";
import useUpdateRecipeMutation from "../../mutations/useUpdateRecipeMutation";
import { recipesQueryOptions } from "../../queries/useRecipeQuery";
import { dialogTabs, RecipeDialogType } from "./RecipeDialog";

export const emptyRecipe: RecipeDto = {
  name: "",
  portions: 1,
  weight: 100,
  isLoading: false,
  isDeleting: false,
  id: "",
  ingredients: [],
  nutritionalContents: createEmptyNutritionalContent(),
  servingSizes: [],
} as const;

function useRecipeDialog({
  dialogType,
  setTab,
  recipeId,
}: {
  dialogType: RecipeDialogType;
  setTab: (tab: dialogTabs) => void;
  recipeId: string | undefined;
}) {
  const navigate = useNavigate();

  const {
    query: { data: queryData, isLoading: queryIsLoading },
    isDelayedPending: queryIsDelayedLoading,
  } = useCustomQuery({
    ...recipesQueryOptions.byId(recipeId),
    onFirstFetch: () => {
      form.setDataIfNoSessionStorage(queryData);
    },
  });

  const createRecipeMutation = useCreateRecipeMutation({
    onSuccess: (recipeId) => {
      navigate({
        to: "/nutrition/recipes/edit/$recipeId",
        params: { recipeId: recipeId },
        search: { tab: "ingredients" },
        replace: true,
      });
    },
    setError: (name, error, options) => {
      form.setError(name, error, options);
    },
  });

  const updateRecipeMutation = useUpdateRecipeMutation({
    recipeId: recipeId ?? "",
    onSuccess: () => {
      setTab("ingredients");
    },
    setError: (name, error, options) => {
      form.setError(name, error, options);
    },
  });

  const pendingState =
    dialogType === "create"
      ? createRecipeMutation.pendingState
      : updateRecipeMutation.pendingState;

  const recipe = queryData ?? emptyRecipe;

  const { form, handleCustomSubmit } = useCustomForm({
    formSchema: recipeDetailsSchema,
    defaultValues: recipe,
    sessionStorageKey: recipeId
      ? `recipe-form-data-${dialogType}-${recipeId}`
      : undefined,
    queryData: queryData,
    onSubmit: async (formData) => {
      const result = await form.trigger();
      if (!result) {
        return;
      }
      if (dialogType === "create") {
        createRecipeMutation.mutate(formData);
      } else {
        updateRecipeMutation.mutate(formData);
      }
    },
  });

  return {
    recipe,
    queryIsLoading: queryIsDelayedLoading || queryIsLoading,
    form,
    handleCustomSubmit,
    pendingState,
  };
}

export default useRecipeDialog;

import { useNavigate, useSearch } from "@tanstack/react-router";

import { router } from "@/App";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import IngredientsList from "@/features/nutrition/ingredients/components/ingredientsList/IngredientsList";

import RecipeMacrosCarousel from "../common/RecipeMacrosCarousel";
import RecipeForm from "./RecipeForm";
import useRecipeDialog from "./useRecipeDialog";

export type dialogTabs = "details" | "ingredients";

export type RecipeDialogType = "create" | "edit";

const dialogText: Record<
  RecipeDialogType,
  {
    title: string;
    buttonText: string;
    description: string;
  }
> = {
  create: {
    title: "Create a new recipe",
    description: "Create a new recipe",
    buttonText: "Create",
  },
  edit: {
    title: "Edit recipe",
    description: "Edit recipe",
    buttonText: "Update",
  },
};

export default function RecipeDialog({
  dialogType,
  dialogTitle,
  dialogDescription,
  recipeId,
  onClose,
}: {
  dialogType: RecipeDialogType;
  dialogTitle?: string;
  dialogDescription?: string;
  recipeId?: string;
  onClose?: () => void;
}) {
  const navigate = useNavigate();
  const { tab } = useSearch({
    from:
      dialogType === "edit"
        ? "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes/_dialogs/edit/$recipeId"
        : "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes/_dialogs/create",
  });

  const setTab = (tab: dialogTabs) => {
    navigate({
      to: router.history.location.pathname,
      search: { tab: tab },
      replace: true,
    });
  };

  const { form, handleCustomSubmit, pendingState, recipe, queryIsLoading } =
    useRecipeDialog({
      dialogType,
      setTab,
      recipeId: recipeId,
    });

  const isLoading = recipeId ? queryIsLoading : false;

  return (
    <Dialog
      onOpenChange={(state) => {
        if (!state) {
          onClose?.();
        }
      }}
      defaultOpen={true}
    >
      <DialogContent
        className="max-h-[100dvh] max-w-[100dvw] p-6"
        withoutOverlay
      >
        <DialogHeader>
          <DialogTitle className="mb-6 text-center">
            {dialogTitle ?? dialogText[dialogType].title}
          </DialogTitle>
          <DialogDescription hidden>
            {dialogDescription ?? dialogText[dialogType].description}
          </DialogDescription>
        </DialogHeader>
        <Tabs
          value={tab}
          onValueChange={(value) => setTab(value as dialogTabs)}
          defaultValue="details"
        >
          <TabsList className="w-full">
            <TabsTrigger className="w-full" value="details">
              Details
            </TabsTrigger>
            <TabsTrigger
              className="w-full"
              value="ingredients"
              disabled={recipeId === undefined || isLoading}
            >
              Ingredients
            </TabsTrigger>
          </TabsList>
          <TabsContent value="details">
            {isLoading ? (
              <RecipeForm.Loading />
            ) : (
              <RecipeForm
                onCancel={() => {
                  form.resetSessionStorage();
                  onClose?.();
                }}
                form={form}
                handleCustomSubmit={handleCustomSubmit}
                submitButtonText={recipeId ? "Update and Next" : "Create"}
                pendingState={pendingState}
              />
            )}
          </TabsContent>
          <TabsContent value="ingredients" className="space-y-2">
            <RecipeMacrosCarousel recipe={recipe} className="my-4" />
            <IngredientsList recipe={recipe} />
          </TabsContent>
        </Tabs>
      </DialogContent>
    </Dialog>
  );
}

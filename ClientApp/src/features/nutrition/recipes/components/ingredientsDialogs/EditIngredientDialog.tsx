import { PropsWithChildren } from "react";

import { Dialog, DialogContent, DialogTrigger } from "@/components/ui/dialog";
import { getServingSizeIndex } from "@/features/nutrition/common/utils/filters";
import { IngredientDto, RecipeDto } from "@/services/openapi";

import IngredientDialogContent from "./IngredientDialogContent";
import useEditIngredientDialog from "./useEditIngredientDialog";

type EditIngredientDialogProps = {
  recipe: RecipeDto;
  ingredient: IngredientDto;
} & PropsWithChildren;

function EditIngredientDialog({
  recipe,
  ingredient,
  children,
}: EditIngredientDialogProps) {
  const { onSubmit, isPending, dialogState, toggleDialogState } =
    useEditIngredientDialog(recipe, ingredient);

  return (
    <Dialog open={dialogState} onOpenChange={toggleDialogState}>
      <DialogTrigger asChild>{children}</DialogTrigger>

      <DialogContent
        className="max-h-screen gap-2"
        onOpenAutoFocus={(e) => {
          e.preventDefault();
        }}
      >
        <IngredientDialogContent
          food={ingredient.food}
          onSubmit={onSubmit}
          isPending={isPending}
          defaultValues={{
            nrOfServings: ingredient.quantity,
            servingSizeIndex: getServingSizeIndex(
              ingredient.food.servingSizes,
              ingredient.servingSize,
            ),
          }}
          submitButtonText="Update food diary"
        />
      </DialogContent>
    </Dialog>
  );
}

export default EditIngredientDialog;

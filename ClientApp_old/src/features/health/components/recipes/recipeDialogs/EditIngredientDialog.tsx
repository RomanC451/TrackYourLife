import { PropsWithChildren } from "react";
import { Dialog, DialogContent, DialogTrigger } from "~/chadcn/ui/dialog";
import { getServingSizeIndex } from "~/features/health/data/filters";
import useEditIngredientDialog from "~/features/health/hooks/recipes/useEditIngredientDialog";
import { IngredientDto, RecipeDto } from "~/services/openapi";
import IngredientDialogContent from "./IngredientDialogContent";

type EditIngredientDialogProps = {
  recipe: RecipeDto;
  ingredient: IngredientDto;
} & PropsWithChildren;

function EditIngredientDialog({
  recipe,
  ingredient,
  children,
}: EditIngredientDialogProps): JSX.Element {
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

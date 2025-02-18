import { Dialog, DialogContent, DialogTrigger } from "@/components/ui/dialog";
import { useLoadingContext } from "@/contexts/LoadingContext";
import FoodListElementOverview from "@/features/nutrition/common/components/foodList/FoodListElementOverview";
import { MUTATION_KEYS } from "@/features/nutrition/common/data/mutationKeys";
import { FoodDto, RecipeDto } from "@/services/openapi";

import IngredientDialogContent from "./IngredientDialogContent";
import useAddIngredientDialog from "./useAddIngredientDialog";

type AddIngredientDialogProps = {
  food: FoodDto;
  recipe: RecipeDto;
  onSuccess: () => void;
};

function AddIngredientDialog({
  food,
  recipe,
  onSuccess,
}: AddIngredientDialogProps) {
  const { dialogState, toggleDialogState, onSubmit, isPending } =
    useAddIngredientDialog(food, recipe, onSuccess);

  const { loadingState } = useLoadingContext(MUTATION_KEYS.recipes);

  return (
    <Dialog open={dialogState} onOpenChange={toggleDialogState}>
      <DialogTrigger
        asChild
        className="w-full text-left"
        disabled={loadingState}
      >
        <button>
          <FoodListElementOverview
            name={food.name}
            brandName={food.brandName}
            quantity={
              food.servingSizes[0].value + " " + food.servingSizes[0].unit
            }
            nutritionalContents={food.nutritionalContents}
          />
        </button>
      </DialogTrigger>

      <DialogContent
        className="gap-2"
        onOpenAutoFocus={(e) => {
          e.preventDefault();
        }}
      >
        <IngredientDialogContent
          food={food}
          onSubmit={onSubmit}
          isPending={isPending}
          submitButtonText="Add to"
        />
      </DialogContent>
    </Dialog>
  );
}

export default AddIngredientDialog;

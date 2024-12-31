import { Dialog, DialogContent, DialogTrigger } from "~/chadcn/ui/dialog";
import useAddIngredientDialog from "~/features/health/hooks/recipes/useAddIngredientDialog";
import { FoodDto, RecipeDto } from "~/services/openapi";
import DiaryEntryOverview from "../../foodSearch/DiaryEntryOverview";
import IngredientDialogContent from "./IngredientDialogContent";

type AddIngredientDialogProps = {
  food: FoodDto;
  recipe: RecipeDto;
  onSuccess: () => void;
};

function AddIngredientDialog({
  food,
  recipe,
  onSuccess,
}: AddIngredientDialogProps): JSX.Element {
  const { dialogState, toggleDialogState, onSubmit, isPending } =
    useAddIngredientDialog(food, recipe, onSuccess);

  return (
    <Dialog open={dialogState} onOpenChange={toggleDialogState}>
      <DialogTrigger asChild className=" w-full text-left">
        <button>
          <DiaryEntryOverview
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

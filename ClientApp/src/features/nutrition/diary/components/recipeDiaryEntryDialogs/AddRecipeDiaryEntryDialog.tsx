import { Dialog, DialogContent, DialogTrigger } from "@/components/ui/dialog";
import FoodListElementOverview from "@/features/nutrition/common/components/foodList/FoodListElementOverview";
import { DateOnly } from "@/lib/date";
import { RecipeDto } from "@/services/openapi";

import RecipeDiaryEntryDialogContent from "./RecipeDiaryEntryDialogContent";
import useAddRecipeDiaryEntryDialog from "./useAddRecipeDiaryEntryDialog";

type AddRecipeDiaryEntryDialogProps = {
  recipe: RecipeDto;
  date: DateOnly;
  onSuccess: () => void;
};

function AddRecipeDiaryEntryDialog({
  recipe,
  date,
  onSuccess,
}: AddRecipeDiaryEntryDialogProps): JSX.Element {
  const { dialogState, toggleDialogState, onSubmit, isPending } =
    useAddRecipeDiaryEntryDialog(recipe, date, onSuccess);

  return (
    <Dialog open={dialogState} onOpenChange={toggleDialogState}>
      <DialogTrigger asChild className="w-full text-left">
        <button>
          <FoodListElementOverview
            name={recipe.name}
            nutritionalContents={recipe.nutritionalContents}
          />
        </button>
      </DialogTrigger>

      <DialogContent
        className="gap-2"
        onOpenAutoFocus={(e) => {
          e.preventDefault();
        }}
      >
        <RecipeDiaryEntryDialogContent
          recipe={recipe}
          onSubmit={onSubmit}
          isPending={isPending}
          submitButtonText="Add to"
        />
      </DialogContent>
    </Dialog>
  );
}

export default AddRecipeDiaryEntryDialog;

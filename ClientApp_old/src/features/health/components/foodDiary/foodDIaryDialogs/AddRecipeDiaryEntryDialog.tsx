import { Dialog, DialogContent, DialogTrigger } from "~/chadcn/ui/dialog";
import useAddRecipeDiaryEntryDialog from "~/features/health/hooks/foodDiaries/useAddRecipeDiaryEntryDialog";
import { RecipeDto } from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import DiaryEntryOverview from "../../foodSearch/DiaryEntryOverview";
import RecipeDiaryEntryDialogContent from "./RecipeDiaryEntryDialogContent";

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
      <DialogTrigger asChild className=" w-full text-left">
        <button>
          <DiaryEntryOverview
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

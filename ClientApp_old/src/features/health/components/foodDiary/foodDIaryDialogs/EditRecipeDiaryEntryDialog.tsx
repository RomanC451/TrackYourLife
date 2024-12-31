import { Dialog, DialogContent } from "~/chadcn/ui/dialog";
import useUpdateRecipeDiaryEntryDialog from "~/features/health/hooks/foodDiaries/useUpdateRecipeDiaryEntryDialog";
import RecipeDiaryEntryDialogContent from "./RecipeDiaryEntryDialogContent";

type EditRecipeDiaryEntryDialogProps = {
  diaryId: string;
  dialogState: boolean;
  toggleDialogState: () => void;
};

function EditRecipeDiaryEntryDialog({
  diaryId,
  dialogState,
  toggleDialogState,
}: EditRecipeDiaryEntryDialogProps) {
  const { recipeDiaryQuery, onSubmit, isPending } =
    useUpdateRecipeDiaryEntryDialog(diaryId, toggleDialogState);

  if (recipeDiaryQuery.data === undefined) return null;

  const diary = recipeDiaryQuery.data;

  return (
    <Dialog open={dialogState} onOpenChange={toggleDialogState}>
      <DialogContent
        className="max-h-screen gap-2"
        onOpenAutoFocus={(e) => {
          e.preventDefault();
        }}
      >
        {isPending.isLoading ? (
          //   <RecipeDiaryDialogContent.Loading />
          "Loading..."
        ) : (
          <RecipeDiaryEntryDialogContent
            recipe={diary.recipe}
            onSubmit={onSubmit}
            isPending={isPending}
            defaultValues={{
              nrOfServings: diary.quantity,
              mealType: diary.mealType,
            }}
            submitButtonText="Update food diary"
          />
        )}
      </DialogContent>
    </Dialog>
  );
}

export default EditRecipeDiaryEntryDialog;

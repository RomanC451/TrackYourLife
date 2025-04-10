import { Dialog, DialogContent } from "@/components/ui/dialog";

import RecipeDiaryEntryDialogContent from "./RecipeDiaryEntryDialogContent";
import useEditRecipeDiaryEntryDialog from "./useEditRecipeDiaryEntryDialog";

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
    useEditRecipeDiaryEntryDialog(diaryId, toggleDialogState);

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
            submitButtonText="Update"
          />
        )}
      </DialogContent>
    </Dialog>
  );
}

export default EditRecipeDiaryEntryDialog;

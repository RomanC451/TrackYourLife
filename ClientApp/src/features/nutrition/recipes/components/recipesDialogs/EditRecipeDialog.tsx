import { memo } from "react";
import { useToggle } from "usehooks-ts";

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from "@/components/ui/dialog";

import useRecipeQuery from "../../queries/useRecipeQuery";
import RecipeDialog from "./RecipeDialog";

type EditRecipeDialogProps = {
  recipeId: string;
  onClose?: () => void;
};

function EditRecipeDialog({ recipeId, onClose }: EditRecipeDialogProps) {
  const { recipeQuery, isPending } = useRecipeQuery(recipeId);

  const [dialogOpen, toggleDialogOpen] = useToggle(true);

  function onOpenChange() {
    toggleDialogOpen();
    onClose?.();
  }

  if (isPending.isStarting) return null;

  return (
    <Dialog open={dialogOpen} onOpenChange={onOpenChange}>
      <DialogContent
        onOpenAutoFocus={(e) => {
          e.preventDefault();
        }}
        className="max-w-[90vw] space-y-4"
      >
        <DialogTitle hidden>Edit recipe</DialogTitle>
        <DialogDescription hidden> Edit recipe</DialogDescription>
        {isPending.isLoading ? (
          <RecipeDialog.Loading />
        ) : recipeQuery.data === undefined ? null : (
          <RecipeDialog recipe={recipeQuery.data} isPending={isPending} />
        )}
      </DialogContent>
    </Dialog>
  );
}

export default memo(EditRecipeDialog);

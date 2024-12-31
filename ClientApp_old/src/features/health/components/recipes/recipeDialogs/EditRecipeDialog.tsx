import { useToggle } from "usehooks-ts";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from "~/chadcn/ui/dialog";
import { LoadingContextProvider } from "~/contexts/LoadingContext";
import useRecipeQuery from "~/features/health/queries/recipes/useRecipeQuery";
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
        className=" "
      >
        <DialogTitle hidden>Edit recipe</DialogTitle>
        <DialogDescription hidden> Edit recipe</DialogDescription>
        <LoadingContextProvider>
          {isPending.isLoading ? (
            <RecipeDialog.Loading />
          ) : recipeQuery.data === undefined ? null : (
            <RecipeDialog recipe={recipeQuery.data} isPending={isPending} />
          )}
        </LoadingContextProvider>
      </DialogContent>
    </Dialog>
  );
}

export default EditRecipeDialog;

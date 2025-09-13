import RecipeDialog from "./RecipeDialog";

function EditRecipeDialog({
  recipeId,
  onClose,
}: {
  recipeId: string;
  onClose?: () => void;
}) {
  return (
    <RecipeDialog dialogType="edit" recipeId={recipeId} onClose={onClose} />
  );
}

export default EditRecipeDialog;

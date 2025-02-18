import { toast } from "sonner";

const recipeCreatedToast = (recipe: string) =>
  toast(recipe, { description: `Recipe has been created.` });

export default recipeCreatedToast;

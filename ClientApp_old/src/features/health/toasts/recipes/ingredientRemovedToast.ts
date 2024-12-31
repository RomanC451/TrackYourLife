import { toast } from "sonner";

const ingredientRemovedToast = (name: string) =>
  toast(name, { description: `Ingredient has been removed.` });

export default ingredientRemovedToast;

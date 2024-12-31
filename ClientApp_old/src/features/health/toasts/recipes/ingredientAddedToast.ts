import { toast } from "sonner";

const ingredientAddedToast = (name: string) =>
  toast(name, { description: `Ingredient has been added.` });

export default ingredientAddedToast;

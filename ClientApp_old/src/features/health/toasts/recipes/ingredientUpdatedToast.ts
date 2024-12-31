import { toast } from "sonner";

const ingredientUpdatedToast = (name: string) =>
  toast(name, { description: `Ingredient has been updated.` });

export default ingredientUpdatedToast;

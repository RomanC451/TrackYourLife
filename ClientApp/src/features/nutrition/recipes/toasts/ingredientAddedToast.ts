import { toast } from "sonner";

const ingredientAddedToast = (promise: Promise<unknown>, name: string) =>
  toast.promise(promise, {
    loading: `Adding ingredient '${name}'`,
    success: () => `Ingredient '${name}' has been added`,
    error: (err) => err?.response?.data?.detail ?? `Failed to add '${name}'`,
  });

export default ingredientAddedToast;

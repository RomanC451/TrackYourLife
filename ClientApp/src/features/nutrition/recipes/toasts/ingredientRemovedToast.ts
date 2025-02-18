import { toast } from "sonner";

const ingredientRemovedToast = (promise: Promise<unknown>) =>
  toast.promise(promise, {
    loading: `Removing ingredients...`,
    success: () => `Ingredients has been removed`,
    error: (err) =>
      err?.response?.data?.detail ?? "Failed to remove ingredients",
  });
export default ingredientRemovedToast;

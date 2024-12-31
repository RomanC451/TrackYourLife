import { toast } from "sonner";

type RecipeDeletedToastProps = {
  name: string;
  action: () => void;
};

const recipeDeletedToast = ({ name, action }: RecipeDeletedToastProps) => {
  toast(name, {
    description: "Has been removed.",
    action: { label: "Undo", onClick: action },
  });
};

export default recipeDeletedToast;

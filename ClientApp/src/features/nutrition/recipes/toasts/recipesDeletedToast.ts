import { toast } from "sonner";

type RecipesDeletedToastProps = {
  count: number;
  action: () => void;
};

const recipesDeletedToast = ({ count, action }: RecipesDeletedToastProps) => {
  toast(`${count} recipes`, {
    description: "Have been removed.",
    action: { label: "Undo", onClick: action },
  });
};

export default recipesDeletedToast;

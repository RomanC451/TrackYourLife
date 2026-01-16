import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  FoodDto,
  IngredientDto,
  RecipeDto,
  ServingSizeDto,
} from "@/services/openapi";

import useAddIngredientMutation from "../../mutations/useAddIngredientMutation";
import useUpdateIngredientMutation from "../../mutations/useUpdateIngredientMutation";
import IngredientForm from "./IngredientForm";
import useIngredientDialog from "./useIngredientDialog";

type IngredientDialogType = "create" | "edit";

const dialogText: Record<
  IngredientDialogType,
  {
    title: string;
    description: string;
    buttonText: string;
  }
> = {
  create: {
    title: "Create a new ingredient",
    description: "Create a new ingredient",
    buttonText: "Create",
  },
  edit: {
    title: "Edit an ingredient",
    description: "Edit an ingredient",
    buttonText: "Update",
  },
};

const defaultValues = (food: FoodDto) => {
  return {
    foodId: food.id,
    servingSizeId: food.servingSizes[0].id,
    quantity: 1,
  };
};

type IngredientDialogProps = {
  food: FoodDto;
  recipe: RecipeDto;
  servingSizes: ServingSizeDto[];
  onClose?: () => void;
  onSuccess?: () => void;
} & (
  | {
      dialogType: "edit";
      ingredient: IngredientDto;
    }
  | {
      dialogType: "create";
      ingredient?: null;
    }
);

export default function IngredientDialog({
  dialogType,
  ingredient,
  food,
  recipe,
  servingSizes,
  onClose,
  onSuccess,
}: IngredientDialogProps) {
  const addIngredientMutation = useAddIngredientMutation({
    recipe: recipe,
    food: food,
    servingSizes: servingSizes,
    setError: (name, error, options) => {
      form.setError(name, error, options);
    },
  });

  const updateIngredientMutation = useUpdateIngredientMutation({
    recipe: recipe,
    ingredient: ingredient!,
    servingSizes: servingSizes,
    setError: (name, error, options) => {
      form.setError(name, error, options);
    },
  });

  const mutation =
    dialogType === "create" ? addIngredientMutation : updateIngredientMutation;

  const defaultVals = ingredient
    ? {
        foodId: ingredient.food.id,
        servingSizeId: ingredient.servingSize.id,
        quantity: ingredient.quantity,
      }
    : defaultValues(food);

  const { form, handleCustomSubmit } = useIngredientDialog({
    mutation: mutation,
    defaultValues: defaultVals,
    onSuccess: () => {
      onSuccess?.();
    },
  });

  function resetDialog() {
    form.reset(defaultVals);
  }

  return (
    <Dialog
      defaultOpen
      onOpenChange={(state) => {
        if (!state) {
          resetDialog();
          onClose?.();
        }
      }}
    >
      <DialogContent
        className="max-h-dvh max-w-dvw p-6"
        withoutOverlay
      >
        <DialogHeader>
          <DialogTitle className="mb-6 text-center">
            {dialogText[dialogType].title}
          </DialogTitle>
          <DialogDescription hidden>
            {dialogText[dialogType].description}
          </DialogDescription>
        </DialogHeader>

        <IngredientForm
          food={food}
          form={form}
          handleCustomSubmit={handleCustomSubmit}
          pendingState={mutation.pendingState}
          submitButtonText={dialogText[dialogType].buttonText}
        />
      </DialogContent>
    </Dialog>
  );
}

import { PlusIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import Spinner from "@/components/ui/spinner";
import { colors } from "@/constants/tailwindColors";
import { changeSvgColor } from "@/lib/changeSvg";
import { cn } from "@/lib/utils";
import { FoodDto, RecipeDto } from "@/services/openapi";

import useAddIngredientMutation from "../mutations/useAddIngredientMutation";

type AddIngredientButtonProps = {
  food: FoodDto;
  recipe: RecipeDto;
  className?: string;
};

function AddIngredientButton({
  recipe,
  food,
  className,
}: AddIngredientButtonProps): React.JSX.Element {
  const plusSvg = changeSvgColor(
    <PlusIcon className="scale-75 transform" />,
    colors.violet,
  );

  const addIngredientMutation = useAddIngredientMutation({
    recipe,
    food,
    servingSizes: Object.values(food.servingSizes),
  });

  return (
    <>
      <Button
        variant="outline"
        size="icon"
        className={cn(className, "")}
        disabled={addIngredientMutation.isPending}
        onClick={() => {
          addIngredientMutation.mutate({
            foodId: food.id,
            servingSizeId: food.servingSizes[0].id,
            quantity: 1,
          });
        }}
      >
        {addIngredientMutation.isPending ? (
          <Spinner color="fill-primary" />
        ) : (
          plusSvg
        )}
      </Button>
    </>
  );
}

export default AddIngredientButton;

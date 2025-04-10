import { PlusIcon } from "lucide-react";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import { colors } from "@/constants/tailwindColors";
import { useLoadingContext } from "@/contexts/LoadingContext";
import { changeSvgColor } from "@/lib/changeSvg";
import { cn } from "@/lib/utils";
import { FoodDto, RecipeDto } from "@/services/openapi";

import { MUTATION_KEYS } from "../../common/data/mutationKeys";
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
}: AddIngredientButtonProps): JSX.Element {
  const plusSvg = changeSvgColor(
    <PlusIcon className="scale-75 transform" />,
    colors.violet,
  );

  const { addIngredientMutation } = useAddIngredientMutation();

  const { loadingState, updateLoadingState } = useLoadingContext(
    MUTATION_KEYS.recipes,
  );

  return (
    <>
      <ButtonWithLoading
        variant="outline"
        size="icon"
        className={cn(className, "")}
        disabled={loadingState || addIngredientMutation.isPending}
        isLoading={addIngredientMutation.isPending}
        onClick={() => {
          updateLoadingState(true);

          addIngredientMutation.mutate(
            {
              recipe,
              food,
              servingSize: food.servingSizes[0],
              foodId: food.id,
              quantity: 1,
              servingSizeId: food.servingSizes[0].id,
            },
            {
              onSettled: () => updateLoadingState(false),
            },
          );
        }}
      >
        {plusSvg}
      </ButtonWithLoading>
    </>
  );
}

export default AddIngredientButton;

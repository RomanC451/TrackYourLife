import { FoodPlusSvg } from "~/assets/health";
import { Button } from "~/chadcn/ui/button";
import { colors } from "~/constants/tailwindColors";
import { FoodDto, RecipeDto } from "~/services/openapi";
import { cn } from "~/utils";
import { changeSvgColor } from "~/utils/changeSvg";
import useAddIngredientMutation from "../../mutations/recipes/useAddIngredientMutation";

type AddIngredientButtonProps = {
  food: FoodDto;
  recipe: RecipeDto;
  className?: string;
  mutation: ReturnType<
    typeof useAddIngredientMutation
  >["addIngredientMutation"];
};

function AddIngredientButton({
  recipe,
  food,
  className,
  mutation,
}: AddIngredientButtonProps): JSX.Element {
  const plusSvg = changeSvgColor(
    <FoodPlusSvg className="scale-75 transform" />,
    colors.violet,
  );
  return (
    <>
      <Button
        variant="outline"
        size="icon"
        className={cn(className, "")}
        // disabled={!isPending.isLoaded}
        onClick={() =>
          mutation.mutateAsync({
            recipe,
            food,
            servingSize: food.servingSizes[0],
            foodId: food.id,
            quantity: 1,
            servingSizeId: food.servingSizes[0].id,
          })
        }
      >
        {/* {isPending.isLoading ? (
          <CircularProgress size={21} className="size-[21px]" />
        ) : ( */}
        plusSvg
        {/* )} */}
      </Button>
    </>
  );
}

export default AddIngredientButton;

import { FoodPlusSvg } from "~/assets/health";
import { Button } from "~/chadcn/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "~/chadcn/ui/dropdown-menu";
import { colors } from "~/constants/tailwindColors";
import { MealTypes, TMealTypes } from "~/features/health/data/enums";
import useAddFoodDiaryMutation from "~/features/health/hooks/useAddFoodDiaryMutation";
import { AddFoodDiaryEntryRequest, FoodResponse } from "~/services/openapi";
import { changeSvgColor } from "~/utils/changeSvg";
import { getDateOnly } from "~/utils/date";
import { cn } from "~/utils/utils";

type SelectMealToAddButtonProps = {
  food: FoodResponse;
  className?: string;
  date: Date;
};

const AddFoodToMealSelectButton: React.FC<SelectMealToAddButtonProps> = ({
  food,
  className,
  date,
}) => {
  const { addFoodDiaryMutation } = useAddFoodDiaryMutation(food);

  async function addFoodToMeal(meal: TMealTypes) {
    const requestBody: AddFoodDiaryEntryRequest = {
      foodId: food.id,
      mealType: meal,
      servingSizeId: food.servingSizes[0].id,
      quantity: 1,
      entryDate: getDateOnly(date),
    };

    addFoodDiaryMutation.mutate(requestBody);
  }

  const plusSvg = changeSvgColor(
    <FoodPlusSvg className="scale-75 transform" />,
    colors.violet,
  );

  return (
    <div className={cn(className, "")}>
      <DropdownMenu>
        <DropdownMenuTrigger
          asChild
          className="bg-secondary hover:bg-background"
        >
          <Button variant="outline" size="icon" className="">
            {plusSvg}
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent className="w-56">
          <DropdownMenuLabel>Select meal</DropdownMenuLabel>
          <DropdownMenuSeparator />

          {Object.values(MealTypes).map((meal, index) => (
            <DropdownMenuItem
              key={index}
              onClick={(e) => {
                e.stopPropagation();
                addFoodToMeal(meal);
              }}
            >
              <span>{meal}</span>
            </DropdownMenuItem>
          ))}
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  );
};

export default AddFoodToMealSelectButton;

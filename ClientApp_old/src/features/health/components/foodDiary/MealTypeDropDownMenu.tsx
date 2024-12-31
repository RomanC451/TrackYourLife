import { CircularProgress } from "@mui/material";
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
import { LoadingState } from "~/hooks/useDelayedLoading";
import { MealTypes } from "~/services/openapi";
import { changeSvgColor } from "~/utils/changeSvg";
import { DateOnly } from "~/utils/date";
import { cn } from "~/utils/utils";

type MealTypeDropDownMenuProps = {
  selectCallback: (mealType: MealTypes) => void;
  className?: string;
  date: DateOnly;
  isPending: LoadingState;
};

const MealTypeDropDownMenu = ({
  selectCallback,
  className,
  isPending,
}: MealTypeDropDownMenuProps) => {
  const plusSvg = changeSvgColor(
    <FoodPlusSvg className="scale-75 transform" />,
    colors.violet,
  );

  return (
    <div
      className={cn(className, "")}
      onBlur={(e) => {
        e.stopPropagation();
        e.preventDefault();
      }}
      onMouseDown={(e) => {
        e.stopPropagation();
        e.preventDefault();
      }}
    >
      <DropdownMenu>
        <DropdownMenuTrigger
          asChild
          className="bg-secondary hover:bg-background"
        >
          <Button
            variant="outline"
            size="icon"
            className=""
            disabled={!isPending.isLoaded}
          >
            {isPending.isLoading ? (
              <CircularProgress size={21} className="size-[21px]" />
            ) : (
              plusSvg
            )}
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
                // addFoodToMeal(meal);
                selectCallback(meal);
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

export default MealTypeDropDownMenu;

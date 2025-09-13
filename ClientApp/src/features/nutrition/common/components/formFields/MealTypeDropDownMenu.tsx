import { PlusIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import Spinner from "@/components/ui/spinner";
import { colors } from "@/constants/tailwindColors";
import { PendingState } from "@/hooks/useCustomQuery";
import { changeSvgColor } from "@/lib/changeSvg";
import { DateOnly } from "@/lib/date";
import { cn } from "@/lib/utils";
import { MealTypes } from "@/services/openapi";

type MealTypeDropDownMenuProps = {
  selectCallback: (mealType: MealTypes) => void;
  className?: string;
  date: DateOnly;
  pendingState: PendingState;
};

const MealTypeDropDownMenu = ({
  selectCallback,
  className,
  pendingState,
}: MealTypeDropDownMenuProps) => {
  const plusSvg = changeSvgColor(
    <PlusIcon className="scale-75 transform" />,
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
            disabled={pendingState.isPending}
          >
            {pendingState.isDelayedPending ? (
              <Spinner className="size-5" />
            ) : (
              plusSvg
            )}
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent className="w-56">
          <DropdownMenuLabel>Select meal</DropdownMenuLabel>
          <DropdownMenuSeparator />

          {Object.values(MealTypes).map((meal) => (
            <DropdownMenuItem
              key={meal}
              onClick={(e) => {
                e.stopPropagation();
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

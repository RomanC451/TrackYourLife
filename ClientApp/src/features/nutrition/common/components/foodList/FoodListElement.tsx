import React, { memo } from "react";

import { Skeleton } from "@/components/ui/skeleton";
import { FoodDto } from "@/services/openapi";

type FoodListElementProps = {
  food: FoodDto;
  AddFoodButton: React.ComponentType<{
    food: FoodDto;
    className?: string;
  }>;
  AddFoodDialog: React.ComponentType<{ food: FoodDto }>;
};

const FoodListElement = memo(function FoodListElement({
  food,
  AddFoodDialog,
  AddFoodButton,
}: FoodListElementProps) {
  return (
    <div className="relative">
      <AddFoodDialog food={food} />
      <AddFoodButton
        food={food}
        className="absolute right-2 top-[50%] translate-y-[-50%]"
      />
    </div>
  );
});

export const LoadingFoodListElement = memo(function LoadingFoodListElement() {
  return (
    <div className="relative h-16 p-2">
      <div className="w-full">
        <Skeleton className="h-5 w-[100px]" />
        <Skeleton className="mt-2 h-4 w-[200px]" />
      </div>
      <Skeleton className="absolute right-2 top-[50%] h-[40px] w-[40px] translate-y-[-50%]" />
    </div>
  );
});

export default memo(FoodListElement);

import { useEffect, useRef } from "react";
import { useNavigate } from "@tanstack/react-router";

import { router } from "@/App";
import { Skeleton } from "@/components/ui/skeleton";
import { queryClient } from "@/queryClient";
import { FoodDto } from "@/services/openapi";

import { foodQueryOptions } from "../../queries/useFoodQuery";
import FoodListElementOverview from "./FoodListElementOverview";
import { useFoodSearchContext } from "./useFoodSearchContext";

type FoodListElementProps = {
  food: FoodDto;
};

function FoodListElement({ food }: FoodListElementProps) {
  const { AddFoodButtonComponent, onSelectedFoodToOptions } =
    useFoodSearchContext();

  const navigate = useNavigate();
  const preloadTimeoutRef = useRef<NodeJS.Timeout | null>(null);

  const cancelPreload = () => {
    if (preloadTimeoutRef.current) {
      clearTimeout(preloadTimeoutRef.current);
      preloadTimeoutRef.current = null;
    }
  };

  const schedulePreload = () => {
    queryClient.setQueryData(foodQueryOptions.byId(food.id).queryKey, food);
    // cancelPreload();
    // preloadTimeoutRef.current = setTimeout(() => {
    //   router.preloadRoute({
    //     ...onSelectedFoodToOptions,
    //     search: { foodId: food.id },
    //   });
    //   preloadTimeoutRef.current = null;
    // }, 500);
  };

  useEffect(() => {
    return () => {
      cancelPreload();
    };
  }, []);

  return (
    <div className="relative">
      <button
        className="w-full"
        onClick={() =>
          navigate({ ...onSelectedFoodToOptions, search: { foodId: food.id } })
        }
        onMouseEnter={schedulePreload}
        onMouseLeave={cancelPreload}
        onTouchStart={schedulePreload}
        onTouchEnd={cancelPreload}
      >
        <FoodListElementOverview
          name={food.name}
          nutritionalContents={food.nutritionalContents}
          brandName={food.brandName}
          quantity={
            food.servingSizes[0].value + " " + food.servingSizes[0].unit
          }
        />
      </button>
      <AddFoodButtonComponent
        className="absolute right-2 top-[50%] translate-y-[-50%]"
        food={food}
      />
    </div>
  );
}

export const LoadingFoodListElement = function LoadingFoodListElement() {
  return (
    <div className="relative h-16 p-2">
      <div className="w-full">
        <Skeleton className="h-5 w-[100px]" />
        <Skeleton className="mt-2 h-4 w-[200px]" />
      </div>
      <Skeleton className="absolute right-2 top-[50%] h-[40px] w-[40px] translate-y-[-50%]" />
    </div>
  );
};

export default FoodListElement;

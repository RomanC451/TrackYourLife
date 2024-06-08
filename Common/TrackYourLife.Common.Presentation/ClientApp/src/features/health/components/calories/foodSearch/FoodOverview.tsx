import React from "react";

import { FoodElement } from "../../../requests/getFoodListRequest";

type FoodOverviewProps = {
  food: FoodElement;
};

const FoodOverview: React.FC<FoodOverviewProps> = ({ food }) => {
  return (
    <div className=" w-full snap-start rounded-lg p-2 text-left hover:bg-accent/50 ">
      <p className="font-bold">{food.name}</p>

      <p className="">
        {food.nutritionalContents.energy.value +
          " " +
          food.nutritionalContents.energy.unit +
          ", "}
        {food.brandName ? food.brandName + ", " : ""}
        {food.servingSizes[0].value + " " + food.servingSizes[0].unit}
      </p>
    </div>
  );
};

export default FoodOverview;

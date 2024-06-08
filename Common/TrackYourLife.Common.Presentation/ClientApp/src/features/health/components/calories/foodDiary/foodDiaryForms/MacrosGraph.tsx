import React from "react";
import DoughnutChart from "~/components/charts/DoughnutChart";
import { colors } from "~/constants/tailwindColors";
import { FoodElement } from "~/features/health/requests/getFoodListRequest";
import AbsoluteCenterChildrenLayout from "~/layouts/AbsoluteCenterChildrenLayout";

type MacrosGraphProps = {
  food: FoodElement;
  nutritionMultiplier: number;
  nutritionalPercentages: number[];
};

const MacrosGraph: React.FC<MacrosGraphProps> = ({
  food,
  nutritionMultiplier,
  nutritionalPercentages
}) => {
  return (
    <div className="relative w-[80px] h-[80px]">
      <AbsoluteCenterChildrenLayout className=" w-auto h-full items-center">
        <div
          className={
            "w-[80px] h-[80px]  rounded-full flex items-center flex-col justify-center"
          }
        >
          <p className="text-lg font font-semibold">
            {(
              food.nutritionalContents.energy.value * nutritionMultiplier
            ).toFixed(1)}
          </p>
          <p className="text-sm">Cal</p>
        </div>
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="w-auto h-full items-center">
        <DoughnutChart
          radius={80}
          userData={nutritionalPercentages}
          colors={[colors.violet, colors.green, colors.yellow]}
          labels={["Carbohydrates", "Fat", "Protein"]}
        />
      </AbsoluteCenterChildrenLayout>
    </div>
  );
};

export default MacrosGraph;

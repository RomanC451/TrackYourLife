import React from "react";

import DoughnutChart from "@/components/charts/DoughnutChart";
import { colors } from "@/constants/tailwindColors";
import AbsoluteCenterChildrenLayout from "@/layouts/AbsoluteCenterChildrenLayout";
import { NutritionalContent } from "@/services/openapi";

type MacrosGraphProps = {
  nutritionalContents: NutritionalContent;
  nutritionMultiplier?: number;
  nutritionalPercentages: number[];
};

const MacrosGraph: React.FC<MacrosGraphProps> = ({
  nutritionalContents,
  nutritionalPercentages,
  nutritionMultiplier = 1,
}) => {
  return (
    <div className="relative size-[80px]">
      <AbsoluteCenterChildrenLayout className="h-full w-auto items-center">
        <div
          className={
            "flex h-[80px] w-[80px] flex-col items-center justify-center rounded-full"
          }
        >
          <p className="font text-lg font-semibold">
            {(nutritionalContents.energy.value * nutritionMultiplier).toFixed(
              1,
            )}
          </p>
          <p className="text-sm">Cal</p>
        </div>
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="h-full w-auto items-center">
        <DoughnutChart
          radius={80}
          userData={nutritionalPercentages}
          colors={[colors.violet, colors.green, colors.yellow]}
          labels={["Protein", "Carbohydrates", "Fat"]}
        />
      </AbsoluteCenterChildrenLayout>
    </div>
  );
};

export default MacrosGraph;

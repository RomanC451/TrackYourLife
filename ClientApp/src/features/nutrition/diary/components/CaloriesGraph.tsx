import React from "react";

import DottedSemiCircleBorderSvg from "@/assets/nutrition/DottedSemiCircleBorder.svg?react";
import CircleProgressBar from "@/components/charts/CircleProgressBar";
import {
  HybridTooltip,
  HybridTooltipContent,
  HybridTooltipTrigger,
} from "@/components/ui/hybrid-tooltip";
import { Skeleton } from "@/components/ui/skeleton";
import { colors } from "@/constants/tailwindColors";
import AbsoluteCenterChildrenLayout from "@/layouts/AbsoluteCenterChildrenLayout";

import useActiveNutritionGoalsQuery from "../queries/useCaloriesGoalQuery";
import useNutritionOverviewQuery from "../queries/useNutritionOverviewQuery";

const CaloriesGraph: React.FC = () => {
  const {
    activeNutritionGoalsQuery,
    goals,
    isPending: caloriesGoalQueryIsPending,
  } = useActiveNutritionGoalsQuery();

  const {
    nutritionOverviewQuery,

    isPending: nutritionOverviewQueryPending,
  } = useNutritionOverviewQuery();

  const completionPercentage =
    nutritionOverviewQuery.data === undefined ||
    activeNutritionGoalsQuery.data === undefined ||
    goals === undefined
      ? 0
      : (nutritionOverviewQuery.data.energy?.value * 100) /
        goals.calories.value;

  return (
    <div className="relative h-[195px] w-[310px] flex-shrink-0">
      <AbsoluteCenterChildrenLayout className="pt-[10px]">
        <CircleProgressBar
          color={colors["violet"]}
          darkColor={colors["dark-violet"]}
          completionPercentage={completionPercentage}
        />
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="pt-[54.5px]">
        <DottedSemiCircleBorderSvg />
      </AbsoluteCenterChildrenLayout>

      <AbsoluteCenterChildrenLayout className="pt-[120px]">
        <div className="flex flex-col items-center gap-[10px] font-semibold">
          {caloriesGoalQueryIsPending.isStarting ||
          nutritionOverviewQueryPending.isStarting ? (
            <div className="h-[26px] w-24" />
          ) : caloriesGoalQueryIsPending.isLoading ||
            nutritionOverviewQueryPending.isLoading ? (
            <Skeleton className="h-[26px] w-24" />
          ) : activeNutritionGoalsQuery.isError ? (
            <p className="text-[24px] leading-[26.4px]">-</p>
          ) : (
            <HybridTooltip>
              <HybridTooltipTrigger>
                <div className="inline-flex text-[24px] leading-[26.4px]">
                  <p
                    className={
                      (goals?.calories.value ?? 0) <
                      (nutritionOverviewQuery.data?.energy?.value ?? 0)
                        ? "text-red-800"
                        : ""
                    }
                  >
                    {nutritionOverviewQuery.data?.energy?.value.toFixed(2)}
                  </p>
                  <p>/{goals?.calories.value}</p>
                </div>
              </HybridTooltipTrigger>
              <HybridTooltipContent side="right">
                <p
                  className={
                    (goals?.calories.value ?? 0) <
                    (nutritionOverviewQuery.data?.energy?.value ?? 0)
                      ? "text-red-800"
                      : ""
                  }
                >
                  {completionPercentage.toFixed(0)} %
                </p>
              </HybridTooltipContent>
            </HybridTooltip>
          )}
          <p className="text-gray text-[14px] leading-[15.4px]">
            Today's calories
          </p>
        </div>
      </AbsoluteCenterChildrenLayout>
    </div>
  );
};

export default CaloriesGraph;

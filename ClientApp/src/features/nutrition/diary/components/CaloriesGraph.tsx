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

import useCaloriesGoalQuery from "../queries/useCaloriesGoalQuery";
import useTotalCaloriesQuery from "../queries/useTotalCaloriesQuery";
import { SetCaloriesGoalDrawer } from "./SetCaloriesGoalDrawer";

const CaloriesGraph: React.FC = () => {
  const { caloriesGoalQuery, isPending: caloriesGoalQueryIsPending } =
    useCaloriesGoalQuery();

  const { totalCaloriesQuery, isPending: totalCaloriesQueryPending } =
    useTotalCaloriesQuery();

  const completionPercentage =
    totalCaloriesQuery.data === undefined ||
    caloriesGoalQuery.data === undefined
      ? 0
      : (totalCaloriesQuery.data * 100) / caloriesGoalQuery.data?.value;

  return (
    <div className="relative h-[195px] w-full flex-shrink-0">
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
      <AbsoluteCenterChildrenLayout className="z-10 pt-[81.5px]">
        <SetCaloriesGoalDrawer />
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="pt-[137px]">
        <div className="flex flex-col items-center gap-[10px] font-semibold">
          {caloriesGoalQueryIsPending.isStarting ||
          totalCaloriesQueryPending.isStarting ? (
            <div className="h-[26px] w-24" />
          ) : caloriesGoalQueryIsPending.isLoading ||
            totalCaloriesQueryPending.isLoading ? (
            <Skeleton className="h-[26px] w-24" />
          ) : caloriesGoalQuery.isError ? (
            <p className="text-[24px] leading-[26.4px]">-</p>
          ) : (
            <HybridTooltip>
              <HybridTooltipTrigger>
                <div className="inline-flex text-[24px] leading-[26.4px]">
                  <p
                    className={
                      (caloriesGoalQuery.data?.value ??
                      0 < (totalCaloriesQuery.data ?? 0))
                        ? "text-red-800"
                        : ""
                    }
                  >
                    {totalCaloriesQuery.data}
                  </p>
                  <p>/{caloriesGoalQuery.data?.value}</p>
                </div>
              </HybridTooltipTrigger>
              <HybridTooltipContent side="right">
                <p
                  className={
                    (caloriesGoalQuery.data?.value ??
                    0 < (totalCaloriesQuery.data ?? 0))
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

import DottedSemiCircleBorderSvg from "@/assets/nutrition/DottedSemiCircleBorder.svg?react";
import CircleProgressBar from "@/components/charts/CircleProgressBar";
import {
  HybridTooltip,
  HybridTooltipContent,
  HybridTooltipTrigger,
} from "@/components/ui/hybrid-tooltip";
import { colors } from "@/constants/tailwindColors";
import AbsoluteCenterChildrenLayout from "@/layouts/AbsoluteCenterChildrenLayout";
import { GoalDto, NutritionalContent } from "@/services/openapi";

type CaloriesGraphProps = {
  goals:
    | {
        calories: GoalDto;
        proteins: GoalDto;
        carbs: GoalDto;
        fat: GoalDto;
      }
    | undefined;
  nutritionOverview: NutritionalContent | undefined;
};

function CaloriesGraph({ goals, nutritionOverview }: CaloriesGraphProps) {
  const completionPercentage =
    nutritionOverview === undefined || goals === undefined
      ? 0
      : (nutritionOverview.energy.value * 100) / goals.calories.value;

  return (
    // <C className="p-4">
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
        <CaloriesDisplay
          goals={goals}
          completionPercentage={completionPercentage}
          calories={nutritionOverview?.energy?.value ?? 0}
        />
      </AbsoluteCenterChildrenLayout>
    </div>
    // </C ard>
  );
}

function CaloriesDisplay({
  goals,
  completionPercentage,
  calories,
}: {
  goals:
    | {
        calories: GoalDto;
        proteins: GoalDto;
        carbs: GoalDto;
        fat: GoalDto;
      }
    | undefined;
  completionPercentage: number;
  calories: number;
}) {
  const renderCaloriesContent = () => {
    return (
      <HybridTooltip>
        <HybridTooltipTrigger>
          <div className="inline-flex text-[24px] leading-[26.4px]">
            <p
              className={
                (goals?.calories.value ?? 0) < (calories ?? 0)
                  ? "text-red-800"
                  : ""
              }
            >
              {calories.toFixed()}
            </p>
            <p>/{goals?.calories.value}</p>
          </div>
        </HybridTooltipTrigger>
        <HybridTooltipContent side="right">
          <p
            className={
              (goals?.calories.value ?? 0) < calories ? "text-red-800" : ""
            }
          >
            {completionPercentage.toFixed()} %
          </p>
        </HybridTooltipContent>
      </HybridTooltip>
    );
  };

  return (
    <div className="flex flex-col items-center gap-[10px] font-semibold">
      {renderCaloriesContent()}
      <p className="text-gray text-[14px] leading-[15.4px]">Today's calories</p>
    </div>
  );
}

export default CaloriesGraph;

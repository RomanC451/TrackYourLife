"use client";

import { Card } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { colors } from "@/constants/tailwindColors";
import { cn } from "@/lib/utils";
import { GoalType } from "@/services/openapi";

import useActiveNutritionGoalsQuery from "../queries/useCaloriesGoalQuery";
import useNutritionOverviewQuery from "../queries/useNutritionOverviewQuery";

export default function MacroProgress() {
  const { nutritionOverviewQuery, isPending: isPendingNutritionOverview } =
    useNutritionOverviewQuery();

  const {
    activeNutritionGoalsQuery,
    isPending: isPendingActiveNutritionGoals,
  } = useActiveNutritionGoalsQuery();

  const calculatePercentage = (current: number, target: number) => {
    return Math.min((current / target) * 100, 100);
  };

  if (
    isPendingActiveNutritionGoals.isLoading ||
    isPendingNutritionOverview.isLoading
  ) {
    return <MacroProgress.Loading />;
  }

  if (
    nutritionOverviewQuery.data === undefined ||
    activeNutritionGoalsQuery.data === undefined
  ) {
    return <MacroProgress.Empty />;
  }

  const gals = {
    carbs: activeNutritionGoalsQuery.data.find(
      (goal) => goal.type === GoalType.Carbohydrates,
    )!.value,
    protein: activeNutritionGoalsQuery.data.find(
      (goal) => goal.type === GoalType.Protein,
    )!.value,
    fat: activeNutritionGoalsQuery.data.find(
      (goal) => goal.type === GoalType.Fats,
    )!.value,
  };

  const percentages = {
    carbs: calculatePercentage(
      nutritionOverviewQuery.data.carbohydrates,
      gals.carbs,
    ).toFixed(0),
    protein: calculatePercentage(
      nutritionOverviewQuery.data.protein,
      gals.protein,
    ).toFixed(0),
    fat: calculatePercentage(nutritionOverviewQuery.data.fat, gals.fat).toFixed(
      0,
    ),
  };

  return (
    <Card className="w-full max-w-[310px] border-0 bg-background p-6 text-white">
      <div className="space-y-4">
        {/* Protein */}
        <div className="space-y-2">
          <div className="flex justify-between text-sm">
            <div className="flex gap-2">
              <span className="text-purple-400">
                {nutritionOverviewQuery.data.protein}g
              </span>
              <span className="text-gray-500">/ {gals.protein}g Protein</span>
            </div>

            <span className="text-purple-400">{percentages.protein}%</span>
          </div>
          <div className="h-2 overflow-hidden rounded-full bg-gray-800">
            <div
              className="h-full rounded-full bg-purple-400 transition-all duration-500"
              style={{
                width: `${percentages.protein}%`,
                boxShadow: "0 0 10px rgba(250, 204, 21, 0.5)",
              }}
            />
          </div>
        </div>

        {/* Carbs */}
        <div className="space-y-2">
          <div className="flex justify-between text-sm">
            <div className="flex gap-2">
              <span className="text-green-400">
                {nutritionOverviewQuery.data.carbohydrates}g
              </span>
              <span className="text-gray-500">/ {gals.carbs}g Carbs</span>
            </div>

            <span className="text-green-400">{percentages.carbs}%</span>
          </div>
          <div className="h-2 overflow-hidden rounded-full bg-gray-800">
            <div
              className="h-full rounded-full bg-green-500 transition-all duration-500"
              style={{
                width: `${percentages.carbs}%`,
                boxShadow: "0 0 10px rgba(147, 51, 234, 0.5)",
              }}
            />
          </div>
        </div>

        {/* Fat */}
        <div className="space-y-2">
          <div className="flex justify-between text-sm">
            <div className="flex gap-2">
              <span className="text-yellow-400">
                {nutritionOverviewQuery.data.fat}g
              </span>
              <span className="text-gray-500">/ {gals.fat}g Fat</span>
            </div>

            <span className="text-yellow-400">{percentages.fat}%</span>
          </div>
          <div className="h-2 overflow-hidden rounded-full bg-gray-800">
            <div
              className="h-full rounded-full bg-yellow-400 transition-all duration-500"
              style={{
                width: `${percentages.fat}%`,
                boxShadow: colors.green,
              }}
            />
          </div>
        </div>
      </div>
    </Card>
  );
}

MacroProgress.Loading = function loading() {
  return (
    <Card className="w-full max-w-[310px] border-0 bg-background p-6 text-white">
      <div className="space-y-4">
        {[
          { name: "Protein", color: "bg-purple-400" },
          { name: "Carbs", color: "bg-green-400" },
          { name: "Fat", color: "bg-yellow-400" },
        ].map((item) => (
          <div className="space-y-2" key={item.name}>
            <div className="flex justify-between text-sm">
              <div className="flex gap-2">
                <Skeleton className={cn("mt-[2px] h-4 w-4", item.color)} />
                <span className="text-gray-500">/ </span>

                <Skeleton className="mt-[2px] h-4 w-4 bg-gray-500" />

                <span className="text-gray-500"> {item.name}</span>
              </div>
              <Skeleton className={cn("mt-[2px] h-4 w-4", item.color)} />
            </div>
            <Skeleton className={cn("h-2", item.color)} />
          </div>
        ))}
      </div>
    </Card>
  );
};

MacroProgress.Empty = function Empty() {
  return <div className="w-full max-w-[310]"></div>;
};

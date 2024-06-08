import { useQuery } from "@tanstack/react-query";
import React from "react";
import { DottedSemiCircleBorderSvg } from "~/assets/health";
import { CircleProgressBar } from "~/components/progressBars";
import { colors } from "~/constants/tailwindColors";
import AbsoluteCenterChildrenLayout from "~/layouts/AbsoluteCenterChildrenLayout";
import { FoodDiaryApi } from "~/services/openapi";
import { getDateOnly } from "~/utils/date";
import useUserCaloriesGoalQuery from "../../hooks/useUserCaloriesGoalQuery";
import { SetCaloriesGoalDrawer } from "./foodDiary/SetCaloriesGoalDrawer";

const foodDiaryApi = new FoodDiaryApi();

const CaloriesGraph: React.FC = () => {
  const totalCaloriesQuery = useQuery({
    queryKey: ["totalCalories"],
    queryFn: () =>
      foodDiaryApi
        .getTotalCalories(getDateOnly(), getDateOnly())
        .then((res) => res.data),
    // getTotalCaloriesRequest({
    //   fetchRequest,
    //   startDate: getDateOnly(),
    //   endDate: getDateOnly(),
    // }),
  });

  const { userCaloriesGoalQuery } = useUserCaloriesGoalQuery();

  return (
    <div className="relative h-[195px] w-full flex-shrink-0">
      <AbsoluteCenterChildrenLayout className="pt-[10px]">
        <CircleProgressBar
          color={colors["violet"]}
          darkColor={colors["dark-violet"]}
          completionPercentage={30}
        />
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="pt-[54.5px]">
        <DottedSemiCircleBorderSvg />
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="z-10 pt-[81.5px]">
        <SetCaloriesGoalDrawer />
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="pt-[137px] ">
        <div className="flex flex-col items-center gap-[10px] font-semibold">
          <p className="text-[24px]  leading-[26.4px]  text-white">
            {totalCaloriesQuery.data?.totalCalories}/
            {userCaloriesGoalQuery.data?.value}
          </p>
          <p className="text-gray  text-[14px]  leading-[15.4px]">
            Today's calories
          </p>
        </div>
      </AbsoluteCenterChildrenLayout>
    </div>
  );
};

export default CaloriesGraph;

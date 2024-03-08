import React, { useState } from "react";
import { CaloriesPlusSvg, DottedSemiCircleBorderSvg } from "~/assets/health";
import { CircleProgressBar } from "~/components/progressBars";
import { colors } from "~/constants/tailwindColors";
import AbsoluteCenterChildrenLayout from "~/layouts/AbsoluteCenterChildrenLayout";

const CaloriesGraph: React.FC = () => {
  const [completition, setCompletition] = useState(34);
  return (
    <div className="relative h-[195px] w-full flex-shrink-0">
      <AbsoluteCenterChildrenLayout className="pt-[10px]">
        <CircleProgressBar
          color={colors["violet"]}
          darkColor={colors["dark-violet"]}
          completitionPercentage={completition}
        />
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="pt-[54.5px]">
        <DottedSemiCircleBorderSvg />
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="z-10 pt-[81.5px]">
        <button onClick={() => setCompletition(completition + 10)}>
          <CaloriesPlusSvg />
        </button>
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="pt-[137px] ">
        <div className="flex flex-col items-center gap-[10px] font-semibold">
          <p className="text-[24px]  leading-[26.4px]  text-white">
            {Math.min(completition, 100)}%
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

import React, { useState } from "react";
import { CaloriesPlusSvg, DottedSemiCircleBorderSvg } from "~/assets/health";
import BoxStyledComponent from "~/components/BoxStyledComponent";
import { CircleProgressBar } from "~/components/progressBars";
import { tailwindColors } from "~/constants/tailwindColors";
import AbsoluteCenterChildrenLayout from "~/layouts/AbsoluteCenterChildrenLayout";

const CaloriesComponent: React.FC = (): JSX.Element => {
  const [completition, setCompletition] = useState(34);

  return (
    <BoxStyledComponent
      width={322}
      height={195}
      title="Calories"
      className="flex-grow"
    >
      <AbsoluteCenterChildrenLayout className="pt-[10px]">
        <CircleProgressBar
          color={tailwindColors["violet"]}
          darkColor={tailwindColors["dark-violet"]}
          completitionPercentage={completition}
        />
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="pt-[54.5px]">
        <DottedSemiCircleBorderSvg />
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="pt-[81.5px] z-10">
        <button onClick={() => setCompletition(completition + 10)}>
          <CaloriesPlusSvg />
        </button>
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout className="pt-[137px] ">
        <div className="font-semibold flex flex-col items-center gap-[10px]">
          <p className="text-white  text-[24px]  leading-[26.4px]">
            {completition}%
          </p>
          <p className="text-gray  text-[14px]  leading-[15.4px]">
            Today's calories
          </p>
        </div>
      </AbsoluteCenterChildrenLayout>
    </BoxStyledComponent>
  );
};

export default CaloriesComponent;

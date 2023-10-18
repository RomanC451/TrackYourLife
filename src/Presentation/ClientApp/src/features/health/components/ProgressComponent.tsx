import clsx from "clsx";
import React from "react";
import { BurgerSvg, ScaleSvg } from "~/assets/health";
import LineChart from "~/components/charts/LineChart";
import {
  tailwindBackgroundColors,
  tailwindColors
} from "~/constants/tailwindColors";
import ComponentTopBarMenuLayout from "~/layouts/ComponentTopBarMenuLayout";

const fontStyle = "font-[Nunito_Sans] font-semibold";

interface ICommonComponentProps {
  color: string;
  title: string;
  progressValue: string;
  chartLineData: number[];
  svg: React.ReactElement;
  svgHorizontalOffset?: number;
}
const CommonPart: React.FC<ICommonComponentProps> = ({
  color,
  title,
  progressValue,
  chartLineData,
  svg,
  svgHorizontalOffset = 0
}): JSX.Element => {
  return (
    <div className="w-[315px] h-[85px] rounded-[10px] border-gray border-2 p-[17px]">
      <div className="flex justify-between font items-center">
        <div
          style={{ backgroundColor: color }}
          className="rounded-full w-[45px] h-[45px] shadow-black/25 shadow-[0_4px_4px_0_rgba(0_0_0_0.25)_inset]"
        >
          <div
            style={{ paddingLeft: svgHorizontalOffset }}
            className="flex justify-center items-center h-full w-full"
          >
            {svg}
          </div>
        </div>
        <div className={clsx(fontStyle, "flex flex-col")}>
          <p className="text-gray text-[14px]">{title}</p>
          <p className="text-[12px]">{progressValue}</p>
        </div>
        <div className="w-[107px] h-[30px] ">
          <LineChart
            color={color}
            userData={chartLineData}
            bgColor={tailwindBackgroundColors["main-dark-bg"]}
          />
        </div>
      </div>
    </div>
  );
};

const ProgressComponent: React.FC = (): JSX.Element => {
  return (
    <ComponentTopBarMenuLayout title="Progress">
      <div className="flex flex-col h-full justify-between min-h-[195px]">
        <CommonPart
          color={tailwindColors.yellow}
          title="Calories deficite"
          progressValue="-1267 kcal"
          chartLineData={[90, 93, 94, 92, 96, 94, 99, 97, 98, 99]}
          svg={<BurgerSvg />}
        />
        <CommonPart
          color={tailwindColors.green}
          title="Weight loss"
          progressValue="-9.78 kg"
          chartLineData={[99, 98, 97, 100, 94, 93, 90, 92, 91, 90]}
          svg={<ScaleSvg />}
          svgHorizontalOffset={4}
        />
      </div>
    </ComponentTopBarMenuLayout>
  );
};

export default ProgressComponent;

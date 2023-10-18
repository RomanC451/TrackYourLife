import clsx from "clsx";
import React from "react";
import BoxStyledComponent from "~/components/BoxStyledComponent";
import DoughnutChart from "~/components/charts/DoughnutChart";
import { tailwindColors } from "~/constants/tailwindColors";
import AbsoluteCenterChildrenLayout from "~/layouts/AbsoluteCenterChildrenLayout";
import ComponentTopBarMenuLayout from "~/layouts/ComponentTopBarMenuLayout";

const fontStyle = "font-[Nunito_Sans] font-semibold ";

interface IProps {
  color: string;
  name: string;
  percentage: string;
  overflowPercentage: string;
}

const ListComponent: React.FC<IProps> = ({
  color,
  name,
  percentage,
  overflowPercentage
}): JSX.Element => {
  return (
    <div className={fontStyle}>
      <div className="flex items-center gap-1">
        <div
          style={{ backgroundColor: color }}
          className="rounded-full w-[9px] h-[9px] mt-[2px]"
        />
        <p className="text-gray text-[16px]">{name}</p>
      </div>
      <div className="flex justify-between">
        <p className="text-[14px]">{percentage}</p>
        <p style={{ color: color }} className="text-[12px]">
          {overflowPercentage}
        </p>
      </div>
    </div>
  );
};

const MacrosComponent: React.FC = (): JSX.Element => {
  const userData = [54.3, 31.7, 14];

  return (
    <BoxStyledComponent
      width={359}
      height={195}
      title="Macros"
      className="flex"
    >
      <div className="relative w-[150px] h-[195px]">
        <AbsoluteCenterChildrenLayout className=" w-auto h-full items-center">
          <div
            className={clsx(
              fontStyle,
              "w-[97px] h-[97px] bg-[#323131] rounded-full flex items-center flex-col justify-center"
            )}
          >
            <p className="text-[20px]">+23%</p>
            <p className="text-[14px]">Total</p>
          </div>
        </AbsoluteCenterChildrenLayout>
        <AbsoluteCenterChildrenLayout className="w-auto h-full items-center">
          <DoughnutChart
            radius={128}
            userData={userData}
            colors={[
              tailwindColors.violet,
              tailwindColors.green,
              tailwindColors.yellow
            ]}
            labels={["Carbohydrates", "Fat", "Protein"]}
          />
        </AbsoluteCenterChildrenLayout>
      </div>
      <div className="w-[209px] h-full flex justify-center items-center flex-col gap-[16px]">
        <div>
          <ListComponent
            color={tailwindColors.violet}
            name="Carbohydrates"
            percentage="54.3%"
            overflowPercentage="+4.3%"
          />
          <ListComponent
            color={tailwindColors.green}
            name="Fat"
            percentage="31.7%"
            overflowPercentage="+1.7%"
          />
          <ListComponent
            color={tailwindColors.yellow}
            name="Protein"
            percentage="14%"
            overflowPercentage="-6%"
          />
        </div>
      </div>
    </BoxStyledComponent>
  );
};

export default MacrosComponent;

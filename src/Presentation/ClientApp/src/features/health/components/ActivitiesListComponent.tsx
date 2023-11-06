import clsx from "clsx";
import React from "react";
import GrowingModal from "~/animations/GrowingModal";
import {
  CyclingSvg,
  GimWorkoutSvg,
  RunningSvg,
  WalkingSvg
} from "~/assets/health/activities";
import BoxStyledComponent from "~/components/BoxStyledComponent";
import { tailwindColors } from "~/constants/tailwindColors";
import ComponentTopBarMenuLayout from "~/layouts/ComponentTopBarMenuLayout";

const fontStyle = "font-[Nunito_Sans] font-semibold ";

interface IProps {
  type: string;
  date: string;
  svg: React.ReactElement;
  color: string;
  progress: string;
  kcals: number;
}
const ListComponent: React.FC<IProps> = ({
  type,
  date,
  svg,
  color,
  progress,
  kcals
}): JSX.Element => {
  return (
    <div className={clsx(fontStyle, "flex justify-around items-center")}>
      <div
        style={{ backgroundColor: color }}
        className="w-[33px] h-[33px] rounded-[10px] flex justify-center items-center shadow-[0_4px_4px_0_rgba(0,0,0,0.25)_inset]"
      >
        {svg}
      </div>
      <div className="flex flex-col justify-start">
        <p className="text-[16px]">{type}</p>
        <p className="text-[12px] text-gray">{date}</p>
      </div>
      <p className="w-[90px] text-[16px] text-gray">{progress}</p>
      <p className="text-[16px] text-gray">{kcals} kcal</p>
    </div>
  );
};

const ActivitiesListComponent: React.FC = (): JSX.Element => {
  return (
    <GrowingModal maxWidth={500} maxHeight={500} minWidth={300} minHeight={272}>
      <BoxStyledComponent
        className="flex flex-col gap-[20px] justify-center flex-grow"
        minWidth={300}
        height={272}
        title="Activities"
      >
        <ListComponent
          svg={<WalkingSvg />}
          date="21 Jul, 2023 at 1:30 PM"
          type="Walking"
          color={tailwindColors.green}
          progress="3 km"
          kcals={134}
        />
        <ListComponent
          svg={<CyclingSvg />}
          date="21 Jul, 2023 at 7:15 AM"
          type="Cycling"
          color={tailwindColors.violet}
          progress="8 km"
          kcals={254}
        />{" "}
        <ListComponent
          svg={<GimWorkoutSvg />}
          date="21 Jul, 2023 at 9:05 PM"
          type="Gim workout"
          color={tailwindColors.yellow}
          progress="1h 15min"
          kcals={215}
        />{" "}
        <ListComponent
          svg={<RunningSvg />}
          date="20 Jul, 2023 at 6:30 PM"
          type="Running"
          color={tailwindColors.turquoise}
          progress="3 km"
          kcals={176}
        />
      </BoxStyledComponent>
    </GrowingModal>
  );
};

export default ActivitiesListComponent;

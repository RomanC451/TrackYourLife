import React from "react";
import GrowingModal from "~/animations/GrowingModal";
import { ReactComponent as CyclingSvg } from "~/assets/health/activities/Cycling.svg";
import { ReactComponent as GimWorkoutSvg } from "~/assets/health/activities/GimWorkout.svg";
import { ReactComponent as RunningSvg } from "~/assets/health/activities/Running.svg";
import { ReactComponent as WalkingSvg } from "~/assets/health/activities/Walking.svg";

import BoxStyledComponent from "~/components/BoxStyledComponent";
import { colors } from "~/constants/tailwindColors";
import { cn } from "../../../utils/utils";

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
  kcals,
}): JSX.Element => {
  return (
    <div className={cn(fontStyle, "flex items-center justify-around")}>
      <div
        style={{ backgroundColor: color }}
        className="flex h-[33px] w-[33px] items-center justify-center rounded-[10px] shadow-[0_4px_4px_0_rgba(0,0,0,0.25)_inset]"
      >
        {svg}
      </div>
      <div className="flex flex-col justify-start">
        <p className="text-[16px]">{type}</p>
        <p className="text-gray text-[12px]">{date}</p>
      </div>
      <p className="text-gray w-[90px] text-[16px]">{progress}</p>
      <p className="text-gray text-[16px]">{kcals} kcal</p>
    </div>
  );
};

const ActivitiesListComponent: React.FC = (): JSX.Element => {
  return (
    <GrowingModal maxWidth={500} maxHeight={500} minWidth={300} minHeight={272}>
      <BoxStyledComponent
        className="flex flex-grow flex-col justify-center gap-[20px]"
        title="Activities"
      >
        <ListComponent
          svg={<WalkingSvg />}
          date="21 Jul, 2023 at 1:30 PM"
          type="Walking"
          color={colors.green}
          progress="3 km"
          kcals={134}
        />
        <ListComponent
          svg={<CyclingSvg />}
          date="21 Jul, 2023 at 7:15 AM"
          type="Cycling"
          color={colors.violet}
          progress="8 km"
          kcals={254}
        />
        <ListComponent
          svg={<GimWorkoutSvg />}
          date="21 Jul, 2023 at 9:05 PM"
          type="Gim workout"
          color={colors.yellow}
          progress="1h 15min"
          kcals={215}
        />
        <ListComponent
          svg={<RunningSvg />}
          date="20 Jul, 2023 at 6:30 PM"
          type="Running"
          color={colors.turquoise}
          progress="3 km"
          kcals={176}
        />
      </BoxStyledComponent>
    </GrowingModal>
  );
};

export default ActivitiesListComponent;

import React from "react";
import { TColors } from "~/constants/tailwindColors";

type MacroOverviewProps = {
  name: string;
  color: TColors;
  percentage: number;
  mass: string;
};

const MacroOverview: React.FC<MacroOverviewProps> = ({
  name,
  color,
  percentage,
  mass
}) => {
  return (
    <div className="flex flex-col items-center">
      <span style={{ color }}>{percentage}%</span>
      <span className="font-semibold text-lg">{mass}g</span>
      <span>{name}</span>
    </div>
  );
};

export default MacroOverview;

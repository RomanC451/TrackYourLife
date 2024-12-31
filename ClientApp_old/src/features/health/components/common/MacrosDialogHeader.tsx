import { Skeleton } from "@mui/material";
import { colors } from "~/constants/tailwindColors";
import { NutritionalContent } from "~/services/openapi";
import getMacrosData from "../../utils/getMacrosData";
import MacroOverview from "./MacroOverview";
import MacrosGraph from "./MacrosGraph";

type MacrosOverviewHeaderProps = {
  nutritionalContents: NutritionalContent;
  nutritionMultiplier?: number;
};

function MacrosDialogHeader({
  nutritionalContents,
  nutritionMultiplier = 1,
}: MacrosOverviewHeaderProps): JSX.Element {
  const macrosOverviewData = getMacrosData(
    nutritionalContents,
    nutritionMultiplier,
  );

  const nutritionalPercentages = [
    macrosOverviewData.carbohydrates.percentage,
    macrosOverviewData.fat.percentage,
    macrosOverviewData.protein.percentage,
  ];

  return (
    <div className="flex items-center justify-between">
      <MacrosGraph
        nutritionalContents={nutritionalContents}
        nutritionMultiplier={nutritionMultiplier}
        nutritionalPercentages={nutritionalPercentages}
      />
      <MacroOverview {...macrosOverviewData.carbohydrates} />
      <MacroOverview {...macrosOverviewData.fat} />
      <MacroOverview {...macrosOverviewData.protein} />
    </div>
  );
}

MacrosDialogHeader.Loading = function () {
  return (
    <div className="flex justify-between">
      <Skeleton className="h-[80px] w-[80px] rounded-full" />
      {[colors.violet, colors.green, colors.yellow].map((color, index) => (
        <div key={index} className="flex flex-col items-center gap-1">
          <Skeleton
            className="h-[20px] w-[31px] rounded-full"
            style={{ backgroundColor: color }}
          />
          <Skeleton className="h-[24px] w-[45px] rounded-full" />
          <Skeleton className="h-[20px] w-[40px] rounded-full" />
        </div>
      ))}
    </div>
  );
};

export default MacrosDialogHeader;

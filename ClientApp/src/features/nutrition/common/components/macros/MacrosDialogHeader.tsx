import { Card } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { colors } from "@/constants/tailwindColors";
import { NutritionalContent } from "@/services/openapi";

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
}: MacrosOverviewHeaderProps): React.JSX.Element {
  const macrosOverviewData = getMacrosData(
    nutritionalContents,
    nutritionMultiplier,
  );

  const nutritionalPercentages = [
    macrosOverviewData.protein.percentage,
    macrosOverviewData.carbohydrates.percentage,
    macrosOverviewData.fat.percentage,
  ];

  return (
    <Card className="grid grid-cols-4 p-1">
      <MacrosGraph
        nutritionalContents={nutritionalContents}
        nutritionMultiplier={nutritionMultiplier}
        nutritionalPercentages={nutritionalPercentages}
      />
      <MacroOverview {...macrosOverviewData.protein} />
      <MacroOverview {...macrosOverviewData.carbohydrates} />
      <MacroOverview {...macrosOverviewData.fat} />
    </Card>
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

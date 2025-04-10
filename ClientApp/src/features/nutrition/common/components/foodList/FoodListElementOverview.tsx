import React from "react";

import { NutritionalContent } from "@/services/openapi";

type DiaryEntryOverviewProps = {
  name: string;
  nutritionalContents: NutritionalContent;
  brandName?: string;
  quantity?: string;
};

const FoodListElementOverview: React.FC<DiaryEntryOverviewProps> = ({
  name,
  nutritionalContents,
  brandName,
  quantity,
}) => {
  return (
    <div className="w-full snap-start rounded-lg p-2 text-left hover:bg-accent/50">
      <p className="font-bold">{name}</p>
      <p className="">
        {nutritionalContents.energy.value.toFixed() + " cal"}
        {brandName ? ", " + brandName : ""}
        {quantity ? ", " + quantity : ""}
      </p>
    </div>
  );
};

export default FoodListElementOverview;

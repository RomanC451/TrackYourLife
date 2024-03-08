import React from "react";
import { DialogTitle } from "~/chadcn/ui/dialog";
import { Separator } from "~/chadcn/ui/separator";
import { Skeleton } from "~/chadcn/ui/skeleton";
import { colors } from "~/constants/tailwindColors";
import { screensEnum } from "~/constants/tailwindSizes";
import { useAppGeneralStateContext } from "~/contexts/AppGeneralContextProvider";

// type FoodDiaryEntryDialogLoadingProps = {};

const FoodDiaryEntryDialogLoading: React.FC = () => {
  const { screenSize } = useAppGeneralStateContext();

  return (
    <>
      <DialogTitle>
        <Skeleton className="h-[18px] w-[250px]" />
      </DialogTitle>
      <Separator />
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
      <Separator />
      {screenSize.width >= screensEnum.sm ? (
        <div className="flex gap-3">
          <div className="w-[180px]">
            <Skeleton className="mb-1 h-[19px] w-[125px]" />
            <Skeleton className="h-[40px] w-full" />
          </div>
          <div className="grow">
            <Skeleton className="mb-1 h-[19px] w-[100px]" />
            <Skeleton className="h-[40px] w-full" />
          </div>
        </div>
      ) : (
        <>
          <div className="w-full">
            <Skeleton className="mb-1 h-[19px] w-[125px]" />
            <Skeleton className="h-[40px] w-full" />
          </div>
          <Separator />
          <div className="full">
            <Skeleton className="mb-1 h-[19px] w-[100px]" />
            <Skeleton className="h-[40px] w-full" />
          </div>
        </>
      )}
      <Separator />
      <div className="flex w-full justify-end">
        <div className="w-full min-w-[130px]  sm:w-[50%]">
          <Skeleton className="mb-1 h-[19px] w-[100px]" />
          <Skeleton className="h-[40px] w-full" />
        </div>
      </div>
      <Separator />
      <div className="flex items-center justify-between">
        <Skeleton className="h-[24px] w-[175px]" />
        <Skeleton className="h-[20px] w-[20px]" />
      </div>
      <Separator />
      <div className="flex w-full justify-end">
        <Skeleton className="h-[40px] w-[150px]" />
      </div>
    </>
  );
};

export default FoodDiaryEntryDialogLoading;

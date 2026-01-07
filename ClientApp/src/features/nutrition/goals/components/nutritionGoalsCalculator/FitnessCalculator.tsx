"use client";

import { useState } from "react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import { screensEnum } from "@/constants/tailwindSizes";
import { useAppGeneralStateContext } from "@/contexts/AppGeneralContextProvider";

import CalculateNutritionGoalsForm from "./CalculateNutritionGoalsForm";
import CalculateNutritionGoalsFormResults from "./CalculateNutritionGoalsFormResults";

export default function FitnessCalculator({
  buttonText,
  onSuccess,
}: {
  buttonText?: string;
  onSuccess?: () => void;
}) {
  const [open, setOpen] = useState(false);
  const [showResults, setShowResults] = useState(false);
  const { screenSize } = useAppGeneralStateContext();
  const isSmallScreen = screenSize.width < screensEnum.sm;

  const content = (
    <div className="-z-20">
      {showResults ? (
        <CalculateNutritionGoalsFormResults />
      ) : (
        <CalculateNutritionGoalsForm
          onSuccess={() => {
            setShowResults(true);
            onSuccess?.();
          }}
        />
      )}
    </div>
  );

  if (isSmallScreen) {
    return (
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogTrigger asChild>
          <Button>Fitness calculator</Button>
        </DialogTrigger>
        <DialogContent className="space-y-4 overflow-hidden">
          <DialogHeader>
            <DialogTitle>Nutrition goals calculator</DialogTitle>
            <DialogDescription hidden>
              Nutrition goals calculator
            </DialogDescription>
          </DialogHeader>
          {content}
          <Button
            variant="outline"
            className="w-full"
            type="button"
            onClick={() => setShowResults((prev) => !prev)}
          >
            {showResults ? "Return to Calculator" : "Results"}
          </Button>
        </DialogContent>
      </Dialog>
    );
  }

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button>{buttonText ?? "Fitness calculator"}</Button>
      </SheetTrigger>
      <SheetContent className="space-y-4 overflow-hidden">
        <SheetDescription hidden>Nutrition goals calculator</SheetDescription>
        <SheetHeader className="mb-4 font-extrabold">
          <SheetTitle>Nutrition goals calculator</SheetTitle>
        </SheetHeader>
        {content}
        <Button
          variant="outline"
          className="w-full"
          type="button"
          onClick={() => setShowResults((prev) => !prev)}
        >
          {showResults ? "Return to Calculator" : "Results"}
        </Button>
      </SheetContent>
    </Sheet>
  );
}

"use client";

import { useState } from "react";
import { useAppGeneralStateContext } from "@/contexts/AppGeneralContextProvider";
import { screensEnum } from "@/constants/tailwindSizes";

import { Button } from "@/components/ui/button";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

import CalculateNutritionGoalsForm from "./CalculateNutritionGoalsForm";
import CalculateNutritionGoalsFormResults from "./CalculateNutritionGoalsFormResults";

export default function FitnessCalculator() {
  const [open, setOpen] = useState(false);
  const [showResults, setShowResults] = useState(false);
  const { screenSize } = useAppGeneralStateContext();
  const isSmallScreen = screenSize.width < screensEnum.sm;

  const content = (
      <div className="-z-20">
        {showResults ? (
          <CalculateNutritionGoalsFormResults 
            onEdit={() => setShowResults(false)} 
          />
        ) : (
          <CalculateNutritionGoalsForm
            onSuccess={() => setShowResults(true)}
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
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Nutrition goals calculator</DialogTitle>
            <DialogDescription hidden>Nutrition goals calculator</DialogDescription>
          </DialogHeader>
          {content}
        </DialogContent>
      </Dialog>
    );
  }

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button>Fitness calculator</Button>
      </SheetTrigger>
      <SheetContent className="overflow-hidden">
            <SheetDescription hidden>Nutrition goals calculator</SheetDescription>
      <SheetHeader className="mb-4 font-extrabold">
        <SheetTitle>Nutrition goals calculator</SheetTitle>
      </SheetHeader>
        {content}
      </SheetContent>
    </Sheet>
  );
}

import React from "react";

import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { Separator } from "@/components/ui/separator";
import { NutritionalContent } from "@/services/openapi";

type NutritionalInfoAccordionProps = {
  nutritionalContents: NutritionalContent;
  nutritionalMultiplier: number;
};

const NutritionalInfoAccordion: React.FC<NutritionalInfoAccordionProps> = ({
  nutritionalContents,
  nutritionalMultiplier,
}) => {
  const renderNutritionalInfo = (
    label: string,
    value: number,
    unit: string,
    highlighted: boolean = false,
  ) => (
    <div
      className={`flex justify-between ${highlighted ? "font-semibold" : ""}`}
    >
      <span style={{ marginLeft: highlighted ? "" : "4px" }}>{label}</span>
      <span>
        {value != 0
          ? `${(value * nutritionalMultiplier).toFixed(1)} ${unit} `
          : "--"}
      </span>
    </div>
  );

  return (
    <Accordion type="single" collapsible className="w-full">
      <AccordionItem value="item-1">
        <AccordionTrigger>Nutritional info</AccordionTrigger>
        <AccordionContent>
          <div className="flex w-full gap-2">
            <div className="flex-grow">
              <div className="h-[140px]">
                {renderNutritionalInfo(
                  "Calories",
                  nutritionalContents.energy.value,
                  "",
                  true,
                )}
                {renderNutritionalInfo(
                  "Total Fat",
                  nutritionalContents.fat,
                  "g",
                  true,
                )}
                {renderNutritionalInfo(
                  "Saturated",
                  nutritionalContents.saturatedFat,
                  "g",
                )}
                {renderNutritionalInfo(
                  "Polyunsaturated",
                  nutritionalContents.polyunsaturatedFat,
                  "g",
                )}
                {renderNutritionalInfo(
                  "Monounsaturated",
                  nutritionalContents.monounsaturatedFat,
                  "g",
                )}
                {renderNutritionalInfo(
                  "Trans",
                  nutritionalContents.transFat,
                  "g",
                )}
                {renderNutritionalInfo(
                  "Cholesterol",
                  nutritionalContents.cholesterol,
                  "mg",
                  true,
                )}
              </div>
              <Separator className="mb-1 mt-1" />
              <div>
                {renderNutritionalInfo(
                  "Vitamin A",
                  nutritionalContents.vitaminA,
                  "%",
                  true,
                )}
                {renderNutritionalInfo(
                  "Vitamin C",
                  nutritionalContents.vitaminC,
                  "%",
                  true,
                )}
              </div>
            </div>
            <div className="flex-grow">
              <div className="h-[140px]">
                {renderNutritionalInfo(
                  "Sodium",
                  nutritionalContents.sodium,
                  "%",
                  true,
                )}
                {renderNutritionalInfo(
                  "Potassium",
                  nutritionalContents.potassium,
                  "mg",
                  true,
                )}
                {renderNutritionalInfo(
                  "Total Carbohydrate",
                  nutritionalContents.carbohydrates,
                  "g",
                  true,
                )}
                {renderNutritionalInfo(
                  "Dietary Fiber",
                  nutritionalContents.fiber,
                  "g",
                )}
                {renderNutritionalInfo(
                  "Sugars",
                  nutritionalContents.sugar,
                  "g",
                )}
                {renderNutritionalInfo(
                  "Protein",
                  nutritionalContents.protein,
                  "g",
                  true,
                )}
              </div>
              <Separator className="mb-1 mt-1" />
              <div>
                {renderNutritionalInfo(
                  "Calcium",
                  nutritionalContents.calcium,
                  "%",
                  true,
                )}
                {renderNutritionalInfo(
                  "Iron",
                  nutritionalContents.iron,
                  "%",
                  true,
                )}
              </div>
            </div>
          </div>
          <p className="mt-3">
            *Percent Daily Values are based on a 2000 calorie diet. Your daily
            values may be higher or lower depending on your calorie needs.
          </p>
        </AccordionContent>
      </AccordionItem>
    </Accordion>
  );
};

export default NutritionalInfoAccordion;

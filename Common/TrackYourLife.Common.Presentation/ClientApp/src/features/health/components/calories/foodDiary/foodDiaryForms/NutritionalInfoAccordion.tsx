import React from "react";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger
} from "~/chadcn/ui/accordion";
import { Separator } from "~/chadcn/ui/separator";
import { FoodElement } from "~/features/health/requests/getFoodListRequest";

type NutritionalInfoAccordionProps = {
  food: FoodElement;
  nutritionalMultiplier: number;
};

const NutritionalInfoAccordion: React.FC<NutritionalInfoAccordionProps> = ({
  food,
  nutritionalMultiplier
}) => {
  const renderNutritionalInfo = (
    label: string,
    value: number,
    unit: string,
    highlithed: boolean = false
  ) => (
    <div
      className={`flex justify-between ${highlithed ? "font-semibold" : ""}`}
    >
      <span style={{ marginLeft: highlithed ? "" : "4px" }}>{label}</span>
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
          <div className="w-full flex gap-2">
            <div className="flex-grow">
              <div className="h-[140px]">
                {renderNutritionalInfo(
                  "Calories",
                  food.nutritionalContents.energy.value,
                  "",
                  true
                )}
                {renderNutritionalInfo(
                  "Total Fat",
                  food.nutritionalContents.fat,
                  "g",
                  true
                )}
                {renderNutritionalInfo(
                  "Saturated",
                  food.nutritionalContents.saturatedFat,
                  "g"
                )}
                {renderNutritionalInfo(
                  "Polyunsaturated",
                  food.nutritionalContents.polyunsaturatedFat,
                  "g"
                )}
                {renderNutritionalInfo(
                  "Monounsaturated",
                  food.nutritionalContents.monounsaturatedFat,
                  "g"
                )}
                {renderNutritionalInfo(
                  "Trans",
                  food.nutritionalContents.transFat,
                  "g"
                )}
                {renderNutritionalInfo(
                  "Cholesterol",
                  food.nutritionalContents.cholesterol,
                  "mg",
                  true
                )}
              </div>
              <Separator className="mt-1 mb-1" />
              <div>
                {renderNutritionalInfo(
                  "Vitamin A",
                  food.nutritionalContents.vitaminA,
                  "%",
                  true
                )}
                {renderNutritionalInfo(
                  "Vitamin B",
                  food.nutritionalContents.vitaminC,
                  "%",
                  true
                )}
              </div>
            </div>
            <div className="flex-grow">
              <div className="h-[140px]">
                {renderNutritionalInfo(
                  "Sodium",
                  food.nutritionalContents.sodium,
                  "%",
                  true
                )}
                {renderNutritionalInfo(
                  "Potassium",
                  food.nutritionalContents.potassium,
                  "mg",
                  true
                )}
                {renderNutritionalInfo(
                  "Total Carbohydrate",
                  food.nutritionalContents.carbohydrates,
                  "g",
                  true
                )}
                {renderNutritionalInfo(
                  "Dietary Fiber",
                  food.nutritionalContents.fiber,
                  "g"
                )}
                {renderNutritionalInfo(
                  "Sugars",
                  food.nutritionalContents.sugar,
                  "g"
                )}
                {renderNutritionalInfo(
                  "Protein",
                  food.nutritionalContents.protein,
                  "g",
                  true
                )}
              </div>
              <Separator className="mt-1 mb-1" />
              <div>
                {renderNutritionalInfo(
                  "Calcium",
                  food.nutritionalContents.calcium,
                  "%",
                  true
                )}
                {renderNutritionalInfo(
                  "Iron",
                  food.nutritionalContents.iron,
                  "%",
                  true
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

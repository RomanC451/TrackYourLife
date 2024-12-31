import { Energy, NutritionalContent } from "~/services/openapi";

function multiplyEnergy(energy: Energy, factor: number): Energy {
  return { ...energy, value: energy.value * factor };
}

function addEnergy(energy1: Energy, energy2: Energy): Energy {
  return { ...energy1, value: energy1.value + energy2.value };
}
function subtractEnergy(energy1: Energy, energy2: Energy): Energy {
  return { ...energy1, value: Math.max(0, energy1.value - energy2.value) };
}

export function multiplyNutritionalContent(
  content: NutritionalContent,
  factor: number,
): NutritionalContent {
  return {
    calcium: content.calcium * factor,
    carbohydrates: content.carbohydrates * factor,
    cholesterol: content.cholesterol * factor,
    fat: content.fat * factor,
    fiber: content.fiber * factor,
    iron: content.iron * factor,
    monounsaturatedFat: content.monounsaturatedFat * factor,
    netCarbs: content.netCarbs * factor,
    polyunsaturatedFat: content.polyunsaturatedFat * factor,
    potassium: content.potassium * factor,
    protein: content.protein * factor,
    saturatedFat: content.saturatedFat * factor,
    sodium: content.sodium * factor,
    sugar: content.sugar * factor,
    transFat: content.transFat * factor,
    vitaminA: content.vitaminA * factor,
    vitaminC: content.vitaminC * factor,
    energy: multiplyEnergy(content.energy, factor),
  };
}

export function addNutritionalContent(
  content1: NutritionalContent,
  content2: NutritionalContent,
): NutritionalContent {
  return {
    calcium: content1.calcium + content2.calcium,
    carbohydrates: content1.carbohydrates + content2.carbohydrates,
    cholesterol: content1.cholesterol + content2.cholesterol,
    fat: content1.fat + content2.fat,
    fiber: content1.fiber + content2.fiber,
    iron: content1.iron + content2.iron,
    monounsaturatedFat:
      content1.monounsaturatedFat + content2.monounsaturatedFat,
    netCarbs: content1.netCarbs + content2.netCarbs,
    polyunsaturatedFat:
      content1.polyunsaturatedFat + content2.polyunsaturatedFat,
    potassium: content1.potassium + content2.potassium,
    protein: content1.protein + content2.protein,
    saturatedFat: content1.saturatedFat + content2.saturatedFat,
    sodium: content1.sodium + content2.sodium,
    sugar: content1.sugar + content2.sugar,
    transFat: content1.transFat + content2.transFat,
    vitaminA: content1.vitaminA + content2.vitaminA,
    vitaminC: content1.vitaminC + content2.vitaminC,
    energy: addEnergy(content1.energy, content2.energy),
  };
}

export function subtractNutritionalContent(
  content1: NutritionalContent,
  content2: NutritionalContent,
): NutritionalContent {
  return {
    calcium: Math.max(0, content1.calcium - content2.calcium),
    carbohydrates: Math.max(0, content1.carbohydrates - content2.carbohydrates),
    cholesterol: Math.max(0, content1.cholesterol - content2.cholesterol),
    fat: Math.max(0, content1.fat - content2.fat),
    fiber: Math.max(0, content1.fiber - content2.fiber),
    iron: Math.max(0, content1.iron - content2.iron),
    monounsaturatedFat: Math.max(
      0,
      content1.monounsaturatedFat - content2.monounsaturatedFat,
    ),
    netCarbs: Math.max(0, content1.netCarbs - content2.netCarbs),
    polyunsaturatedFat: Math.max(
      0,
      content1.polyunsaturatedFat - content2.polyunsaturatedFat,
    ),
    potassium: Math.max(0, content1.potassium - content2.potassium),
    protein: Math.max(0, content1.protein - content2.protein),
    saturatedFat: Math.max(0, content1.saturatedFat - content2.saturatedFat),
    sodium: Math.max(0, content1.sodium - content2.sodium),
    sugar: Math.max(0, content1.sugar - content2.sugar),
    transFat: Math.max(0, content1.transFat - content2.transFat),
    vitaminA: Math.max(0, content1.vitaminA - content2.vitaminA),
    vitaminC: Math.max(0, content1.vitaminC - content2.vitaminC),
    energy: subtractEnergy(content1.energy, content2.energy),
  };
}

export function createEmptyNutritionalContent(): NutritionalContent {
  return {
    calcium: 0,
    carbohydrates: 0,
    cholesterol: 0,
    fat: 0,
    fiber: 0,
    iron: 0,
    monounsaturatedFat: 0,
    netCarbs: 0,
    polyunsaturatedFat: 0,
    potassium: 0,
    protein: 0,
    saturatedFat: 0,
    sodium: 0,
    sugar: 0,
    transFat: 0,
    vitaminA: 0,
    vitaminC: 0,
    energy: { unit: "calories", value: 0 },
  };
}

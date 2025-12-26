import { v4 as uuidv4 } from "uuid";

import { ExerciseSet, ExerciseSetType } from "@/services/openapi";

import { ExerciseSetSchema } from "../data/exercisesSchemas";

export function exerciseSetSchemaToApiExerciseSet(
  exerciseSet: ExerciseSetSchema,
): ExerciseSet {
  switch (exerciseSet.type) {
    case ExerciseSetType.Weight:
      return {
        id: exerciseSet.id ?? uuidv4(),
        name: exerciseSet.name,
        orderIndex: exerciseSet.orderIndex,
        restTimeSeconds: exerciseSet.restTimeSeconds,
        type: ExerciseSetType.Weight,
        reps: exerciseSet.reps,
        weight: exerciseSet.weight,
        durationSeconds: undefined,
        distance: undefined,
      };
    case ExerciseSetType.Time:
      return {
        id: exerciseSet.id ?? uuidv4(),
        name: exerciseSet.name,
        orderIndex: exerciseSet.orderIndex,
        restTimeSeconds: exerciseSet.restTimeSeconds,
        type: ExerciseSetType.Time,
        durationSeconds: exerciseSet.durationSeconds,
        reps: undefined,
        weight: undefined,
        distance: undefined,
      };
    case ExerciseSetType.Bodyweight:
      return {
        id: exerciseSet.id ?? uuidv4(),
        name: exerciseSet.name,
        orderIndex: exerciseSet.orderIndex,
        restTimeSeconds: exerciseSet.restTimeSeconds,
        type: ExerciseSetType.Bodyweight,
        reps: exerciseSet.reps,
        weight: undefined,
        durationSeconds: undefined,
        distance: undefined,
      };
    case ExerciseSetType.Distance:
      return {
        id: exerciseSet.id ?? uuidv4(),
        name: exerciseSet.name,
        orderIndex: exerciseSet.orderIndex,
        restTimeSeconds: exerciseSet.restTimeSeconds,
        type: ExerciseSetType.Distance,
        distance: exerciseSet.distance,
        reps: undefined,
        weight: undefined,
        durationSeconds: undefined,
      };
    default:
      throw new Error("Invalid set type");
  }
}

export function apiExerciseSetToExerciseSetSchema(
  exerciseSet: ExerciseSet,
): ExerciseSetSchema {
  switch (exerciseSet.type) {
    case ExerciseSetType.Weight:
      return {
        id: exerciseSet.id,
        name: exerciseSet.name,
        orderIndex: exerciseSet.orderIndex,
        restTimeSeconds: exerciseSet.restTimeSeconds,
        type: ExerciseSetType.Weight,
        reps: exerciseSet.reps ?? 0,
        weight: exerciseSet.weight ?? 0,
      };
    case ExerciseSetType.Time:
      return {
        id: exerciseSet.id,
        name: exerciseSet.name,
        orderIndex: exerciseSet.orderIndex,
        restTimeSeconds: exerciseSet.restTimeSeconds,
        type: ExerciseSetType.Time,
        durationSeconds: exerciseSet.durationSeconds ?? 0,
      };
    case ExerciseSetType.Bodyweight:
      return {
        id: exerciseSet.id,
        name: exerciseSet.name,
        orderIndex: exerciseSet.orderIndex,
        restTimeSeconds: exerciseSet.restTimeSeconds,
        type: ExerciseSetType.Bodyweight,
        reps: exerciseSet.reps ?? 0,
      };
    case ExerciseSetType.Distance:
      return {
        id: exerciseSet.id,
        name: exerciseSet.name,
        orderIndex: exerciseSet.orderIndex,
        restTimeSeconds: exerciseSet.restTimeSeconds,
        type: ExerciseSetType.Distance,
        distance: exerciseSet.distance ?? 0,
        distanceUnit: "km",
      };
    default:
      throw new Error("Invalid set type");
  }
}

export function createDefaultExerciseSet(
  type: ExerciseSetType,
  orderIndex: number,
): ExerciseSetSchema {
  switch (type) {
    case ExerciseSetType.Weight:
      return {
        name: "Set " + (orderIndex + 1),
        orderIndex: orderIndex,
        restTimeSeconds: 0,
        type: type,
        reps: 0,
        weight: undefined!,
      };
    case ExerciseSetType.Time:
      return {
        name: "Set " + (orderIndex + 1),
        orderIndex: orderIndex,
        restTimeSeconds: 0,
        type: type,
        durationSeconds: 0,
      };
    case ExerciseSetType.Bodyweight:
      return {
        name: "Set " + (orderIndex + 1),
        orderIndex: orderIndex,
        restTimeSeconds: 0,
        type: type,
        reps: 0,
      };
    case ExerciseSetType.Distance:
      return {
        name: "Set " + (orderIndex + 1),
        orderIndex: orderIndex,
        restTimeSeconds: 0,
        type: type,
        distance: 0,
        distanceUnit: "km",
      };
    default:
      throw new Error("Invalid set type");
  }
}
